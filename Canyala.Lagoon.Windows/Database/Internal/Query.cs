//-------------------------------------------------------------------------------
//
//  MIT License
//
//  Copyright (c) 2012-2022 Canyala Innovation
//
//  Permission is hereby granted, free of charge, to any person obtaining a copy
//  of this software and associated documentation files (the "Software"), to deal
//  in the Software without restriction, including without limitation the rights
//  to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//  copies of the Software, and to permit persons to whom the Software is
//  furnished to do so, subject to the following conditions:
//
//  The above copyright notice and this permission notice shall be included in all
//  copies or substantial portions of the Software.
//
//  THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//  IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//  FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//  AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//  LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//  OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
//  SOFTWARE.
//
//-------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using System.Globalization;
using System.Reflection;

using Canyala.Lagoon.Extensions;
using Canyala.Lagoon.Functional;
using System.Collections;


namespace Canyala.Lagoon.Database.Internal
{
    /// <summary>
    /// Provides logic for generating sql.
    /// </summary>
    internal static class Query
    {
        public static readonly string IdPropertyName = "Id";
        public static readonly Type IdPropertyType = typeof(Guid);
        private static readonly string[] SortDirections = { "ASC", "DESC" };

        internal static string TableName(Type type)
        {
            var nameAttribute = type.GetCustomAttributes(false).OfType<NameAttribute>().SingleOrDefault();

            if (nameAttribute != null) 
                return nameAttribute.Name;

            int length = 1;
            while (length < type.Name.Length && char.IsLower(type.Name[length]))
                length++;

            return type.Name.Substring(0, length);
        }

        internal static string ColumnName(PropertyInfo property)
        {
            var nameAttribute = property.GetCustomAttributes(false).OfType<NameAttribute>().SingleOrDefault();

            if (nameAttribute != null) 
                return nameAttribute.Name;

            return property.Name;
        }

        private static readonly Dictionary<Type,string> SqlTypes = new Dictionary<Type,string>
        {
            { typeof(Guid), "uniqueidentifier" },
            { typeof(byte), "tinyint" },
            { typeof(int), "smallint" },
            { typeof(long), "int" },
            { typeof(bool), "bit" },
            { typeof(string), "nvarchar(256)" },
            { typeof(float), "float(24)" },
            { typeof(double), "float(53)" },
            { typeof(decimal), "money" },
            { typeof(DateTime), "datetime" },
            { typeof(DateTimeOffset), "datetimeoffset" },
            { typeof(byte[]), "image" },
            { typeof(char), "nchar(1)" }
        };

        internal static string VerifyTable<T>()
        {
            var type = typeof(T);
            var table = TableName(type);

            /*
            var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);

            var columnDefinitions = properties
                .Select(property => "[{0}] {1} {2}".Args(ColumnName(property), SqlTypes[property.PropertyType], SqlConstraint(property)))
                .Join(',');
            */

            return "SELECT OBJECT_ID('{0}', 'U') AS Result".Args(table);
        }

        internal static string CreateTable<T>()
        {
            var type = typeof(T);
            var table = TableName(type);
            var properties = type.GetProperties(BindingFlags.Public|BindingFlags.Instance);

            var columnDefinitions = properties
                .Select(property => "[{0}] {1} {2}".Args(ColumnName(property), SqlTypes[property.PropertyType], SqlConstraint(property)))
                .Join(',');

            return "CREATE TABLE [{0}] ({1})".Args(table, columnDefinitions);
        }

        private static string SqlConstraint(PropertyInfo property)
        {
            if (property.PropertyType == IdPropertyType && ColumnName(property) == IdPropertyName)
                return "NOT NULL PRIMARY KEY";
            else
                return "NOT NULL";
        }

        internal static string DropTable<T>()
        {
            return "DROP TABLE [{0}]".Args(TableName(typeof(T)));
        }

        internal static string Insert<T>(IEnumerable<T> records)
        {
            var type = typeof(T);
            var properties = type.GetProperties(BindingFlags.Public|BindingFlags.Instance);
            var idProperty = type.GetProperty(IdPropertyName, typeof(Guid));

            var columnNames = properties
                .Select(property => "[{0}]".Args(ColumnName(property)))
                .Join(',');

            var queryBuilder = new StringBuilder();

            foreach (var record in records)
            {
                if (queryBuilder.Length > 0)
                    queryBuilder.Append(Environment.NewLine);

                if (idProperty != null && idProperty.GetValue(record).Equals(Guid.Empty))
                    idProperty.SetValue(record, Guid.NewGuid());

                var columnValues = properties
                    .Select(property => "{0}".Args(ToSql(property.GetValue(record))))
                    .Join(',');

                queryBuilder.Append("INSERT INTO [{0}] ({1}) VALUES ({2})".Args(TableName(type), columnNames, columnValues));
            }

            return queryBuilder.ToString();
        }

        internal static string Update<T>(IEnumerable<T> records)
            where T : class, new()
        {
            var type = typeof(T);
            var table = TableName(type);
            var idProperty = type.GetProperty(IdPropertyName, IdPropertyType);

            if (idProperty == null)
                throw new ArgumentException("Query.Update<T> requires that 'T' declares a property '{0}' of type '{1}"
                    .Args(IdPropertyName, IdPropertyType.Name));

            var properties = type.GetProperties(BindingFlags.Public|BindingFlags.Instance);
            var updateProperties = properties.Except(Seq.Of(idProperty));

            var queryBuilder = new StringBuilder();

            foreach (var record in records)
            {
                if (queryBuilder.Length > 0)
                    queryBuilder.Append(Environment.NewLine);

                var setClause = updateProperties
                    .Select(property => "[{0}]={1}".Args(ColumnName(property), ToSql(property.GetValue(record))))
                    .Join(',');

                var whereClause = "WHERE [{0}]={1}"
                    .Args(idProperty.Name, ToSql(idProperty.GetValue(record)));

                queryBuilder.Append("UPDATE [{0}] SET {1} {2}".Args(table, setClause, whereClause));
            }

            return queryBuilder.ToString();
        }

        internal static string Delete<T>(Expression<Predicate<T>> wherePredicate)
            where T : new()
        {
            return "DELETE FROM [{0}]{1}"
                .Args(TableName(typeof(T)), wherePredicate.ToSqlWhereClause());
        }

        internal static string Select<T,TResult>(Expression<Predicate<T>> where=null, Expression<Func<TResult,object>> orderBy=null, Sort sort = Sort.Ascending,  int skip=0, int take=0)
            where T : new()
        {
            var type = typeof(T);
            var properties = type.GetProperties(BindingFlags.Public|BindingFlags.Instance);
            var table = TableName(type);

            var columns = properties
                .Select(property => "[{0}]".Args(ColumnName(property)))
                .Join(',');

            var sortDirectionClause = orderBy != null 
                ? String.Concat(" ", SortDirections[(int)sort]) 
                : String.Empty;

            int top = skip + take;

            if (top > 0)
            {
                var orderByClause = orderBy == null ? " ORDERBY [{0}]".Args(IdPropertyName) : orderBy.ToSqlOrderByClause();
                return "SELECT {0} FROM (SELECT TOP {1} ROW_NUMBER() OVER ({2}{3}) AS [_Row],{0} FROM [{4}]{5}) AS [_WithRows] WHERE [_Row]>{6} AND [_Row]<={1}".
                    Args(columns, top, orderByClause, sortDirectionClause, table, where.ToSqlWhereClause(), skip);
            }
            else
                return "SELECT {0} FROM [{1}]{2}{3}{4}"
                    .Args(columns, table, where.ToSqlWhereClause(), orderBy.ToSqlOrderByClause(), sortDirectionClause);
        }

        internal static string Count<T>(Expression<Predicate<T>> expression = null)
        {
            return "SELECT COUNT(*) FROM [{0}]{1}"
                .Args(TableName(typeof(T)), expression.ToSqlWhereClause());
        }

        internal static string ToSqlWhereClause<T>(this Expression<Predicate<T>> expression)
        {
            if (expression != null)
                return String.Concat(" WHERE ", ToSql(expression.Reduce()));
            else
                return String.Empty;
        }

        internal static string ToSqlOrderByClause<T>(this Expression<Func<T, object>> expression)
        {
            if (expression != null)
                return String.Concat(" ORDER BY ", ToSql(expression));
            else
                return String.Empty;
        }

        private static string ToSql(Expression expression, bool addStringPadding=true, bool evaluate=false)
        {
            switch (expression.NodeType)
            {
                case ExpressionType.Lambda:
                    {
                        var lambdaExpression = (LambdaExpression) expression;
                        return ToSql(lambdaExpression.Body);
                    }

                case ExpressionType.Convert:
                    {
                        var convertExpression = (UnaryExpression) expression;
                        return ToSql(convertExpression.Operand);
                    }

                case ExpressionType.Equal:
                    {
                        var binaryExpression = (BinaryExpression) expression;
                        return String.Concat(ToSql(binaryExpression.Left), "=", ToSql(binaryExpression.Right, addStringPadding, true));
                    }

                case ExpressionType.NotEqual:
                    {
                        var binaryExpression = (BinaryExpression) expression;
                        return String.Concat("NOT ", ToSql(binaryExpression.Left), "=", ToSql(binaryExpression.Right, true));
                    }

                case ExpressionType.AndAlso:
                    {
                        var binaryExpression = (BinaryExpression) expression;
                        return String.Concat(ToSql(binaryExpression.Left), " AND ", ToSql(binaryExpression.Right));
                    }

                case ExpressionType.OrElse:
                    {
                        var binaryExpression = (BinaryExpression) expression;
                        return String.Concat(ToSql(binaryExpression.Left), " OR ", ToSql(binaryExpression.Right));
                    }

                case ExpressionType.LessThan:
                    {
                        var binaryExpression = (BinaryExpression) expression;
                        return String.Concat(ToSql(binaryExpression.Left), "<", ToSql(binaryExpression.Right));
                    }

                case ExpressionType.LessThanOrEqual:
                    {
                        var binaryExpression = (BinaryExpression) expression;
                        return String.Concat(ToSql(binaryExpression.Left), "<=", ToSql(binaryExpression.Right));
                    }

                case ExpressionType.GreaterThan:
                    {
                        var binaryExpression = (BinaryExpression) expression;
                        return String.Concat(ToSql(binaryExpression.Left), ">", ToSql(binaryExpression.Right));
                    }

                case ExpressionType.GreaterThanOrEqual:
                    {
                        var binaryExpression = (BinaryExpression) expression;
                        return String.Concat(ToSql(binaryExpression.Left), ">=", ToSql(binaryExpression.Right));
                    }

                case ExpressionType.MemberAccess:
                    {
                        var memberExpression = (MemberExpression)expression;

                        if (memberExpression.Expression.NodeType == ExpressionType.Parameter)
                            return String.Concat('[', memberExpression.Member.Name, ']');

                        var value = Expression
                            .Lambda(memberExpression)
                            .Compile()
                            .DynamicInvoke();

                        return ToSql(value, addStringPadding);
                    }

                case ExpressionType.Call:
                    {
                        var methodCallExpression = (MethodCallExpression) expression;
                        var memberExpression = methodCallExpression.Object as MemberExpression;

                        if (memberExpression == null)
                        {
                            switch (methodCallExpression.Method.Name)
                            {
                                case "Contains" :
                                    var sequence = Expression
                                        .Lambda(methodCallExpression.Arguments[0])
                                        .Compile()
                                        .DynamicInvoke()
                                        as IEnumerable;

                                    var inList = sequence.Map(item => ToSql(item)).Join(',');

                                    var column = ToSql(methodCallExpression.Arguments[1]);

                                    return "{0} IN ({1})".Args(column, inList);

                                default:
                                    throw new ArgumentException("Query - Not supported expression: {0}".Args(expression.ToString()));
                            }
                        }
                        else if (memberExpression.Type != typeof(String))
                        {
                            switch (methodCallExpression.Method.Name)
                            {
                                case "Contains":

                                    var sequence = Expression
                                        .Lambda(memberExpression)
                                        .Compile()
                                        .DynamicInvoke()
                                        as IEnumerable;

                                    var inList = sequence.Map(item => ToSql(item)).Join(',');

                                    var column = ToSql(methodCallExpression.Arguments[0]);

                                    return "{0} IN ({1})".Args(column, inList);

                                default:
                                    throw new ArgumentException("Query - Not supported expression: {0}".Args(expression.ToString()));
                            }
                        }
                        else
                        {
                            switch (methodCallExpression.Method.Name)
                            {
                                case "StartsWith":
                                    return "[{0}] LIKE '{1}%'".Args(memberExpression.Member.Name, ToSql(methodCallExpression.Arguments[0], false));

                                case "EndsWith":
                                    return "[{0}] LIKE '%{1}'".Args(memberExpression.Member.Name, ToSql(methodCallExpression.Arguments[0], false));

                                case "Contains":
                                    return "[{0}] LIKE '%{1}%'".Args(memberExpression.Member.Name, ToSql(methodCallExpression.Arguments[0], false));

                                default:
                                    throw new ArgumentException("Query - Not supported expression: {0}".Args(expression.ToString()));
                            }
                        }
                    }

                case ExpressionType.Constant:
                    {
                        var constantExpression = (ConstantExpression) expression;
                        return ToSql(constantExpression.Value, addStringPadding);
                    }

                default:
                    throw new ArgumentException("Query - Not supported expression: {0}".Args(expression.ToString()));
            }
        }

        private static string ToSql(object instance, bool addStringPadding = true)
        {
            var type = instance.GetType();

            if (type == typeof(string))
                if (addStringPadding)
                    return String.Concat("'", instance.ToString().Replace("'", "''"), "'");
                else
                    return instance.ToString().Replace("'", "''");

            else if (type == typeof(decimal) || type == typeof(float) || type == typeof(double))
                return Convert.ToString(instance, CultureInfo.InvariantCulture);

            else if (type == typeof(DateTime))
                return String.Concat("'", instance.ToString(), "'");

            else if (type == typeof(Guid))
                return String.Concat("'", instance.ToString(), "'");

            else
                return instance.ToString();
        }
    }
}
