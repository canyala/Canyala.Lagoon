/*
 
  MIT License

  Copyright (c) 2022 Canyala Innovation

  Permission is hereby granted, free of charge, to any person obtaining a copy
  of this software and associated documentation files (the "Software"), to deal
  in the Software without restriction, including without limitation the rights
  to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
  copies of the Software, and to permit persons to whom the Software is
  furnished to do so, subject to the following conditions:

  The above copyright notice and this permission notice shall be included in all
  copies or substantial portions of the Software.

  THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
  IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
  FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
  AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
  LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
  OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
  SOFTWARE.

*/

using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Canyala.Lagoon.Extensions
{
    public static class ExpressionExtensions
    {
        public static string ToSqlWhereClause<T>(this Expression<Predicate<T>> expression)
        {
            if (expression != null)
                return String.Concat(" WHERE ", ToSql(expression.Reduce()));
            else
                return String.Empty;
        }

        public static string ToSqlOrderByClause<T>(this Expression<Func<T,object>> expression)
        {
            if (expression != null)
                return String.Concat(" ORDER BY [", ToSql(expression), "]");
            else
                return String.Empty;
        }

        private static string ToSql(Expression expression)
        {
            Contract.Assume(expression != null);

            switch (expression.NodeType)
            {
                case ExpressionType.Lambda :
                {
                    var lambdaExpression = (LambdaExpression) expression;
                    return ToSql(lambdaExpression.Body);
                }

                case ExpressionType.Equal :
                {
                    var binaryExpression = (BinaryExpression) expression;
                    return String.Concat(ToSql(binaryExpression.Left), " = ", ToSql(binaryExpression.Right));
                }

                case ExpressionType.NotEqual:
                {
                    var binaryExpression = (BinaryExpression) expression;
                    return String.Concat("NOT ", ToSql(binaryExpression.Left), " = ", ToSql(binaryExpression.Right));
                }

                case ExpressionType.AndAlso :
                {
                    var binaryExpression = (BinaryExpression) expression;
                    return String.Concat(ToSql(binaryExpression.Left), " AND ", ToSql(binaryExpression.Right));
                }

                case ExpressionType.OrElse :
                {
                    var binaryExpression = (BinaryExpression) expression;
                    return String.Concat(ToSql(binaryExpression.Left), " OR ", ToSql(binaryExpression.Right));
                }

                case ExpressionType.LessThan:
                {
                    var binaryExpression = (BinaryExpression) expression;
                    return String.Concat(ToSql(binaryExpression.Left), " < ", ToSql(binaryExpression.Right));
                }

                case ExpressionType.LessThanOrEqual:
                {
                    var binaryExpression = (BinaryExpression) expression;
                    return String.Concat(ToSql(binaryExpression.Left), " <= ", ToSql(binaryExpression.Right));
                }

                case ExpressionType.GreaterThan:
                {
                    var binaryExpression = (BinaryExpression) expression;
                    return String.Concat(ToSql(binaryExpression.Left), " > ", ToSql(binaryExpression.Right));
                }

                case ExpressionType.GreaterThanOrEqual:
                {
                    var binaryExpression = (BinaryExpression) expression;
                    return String.Concat(ToSql(binaryExpression.Left), " >= ", ToSql(binaryExpression.Right));
                }

                case ExpressionType.MemberAccess:
                {
                    var memberExpression = (MemberExpression) expression;
                    return memberExpression.Member.Name;
                }

                case ExpressionType.Call:
                {
                    var methodCallExpression = (MethodCallExpression) expression;
                    var memberExpression = methodCallExpression.Object as MemberExpression;

                    #region Assumptions
                    Contract.Assume(methodCallExpression.Arguments.Count > 0);
                    Contract.Assume(memberExpression != null);
                    #endregion

                    switch (methodCallExpression.Method.Name)
                    {
                        case "StartsWith" :
                            return "[{0}] LIKE '{1}%'".Args(memberExpression.Member.Name, ToSql(methodCallExpression.Arguments[0]));

                        case "EndsWith":
                            return "[{0}] LIKE '%{1}'".Args(memberExpression.Member.Name, ToSql(methodCallExpression.Arguments[0]));

                        case "Contains":
                            return "[{0}] LIKE '%{1}%'".Args(memberExpression.Member.Name, ToSql(methodCallExpression.Arguments[0]));

                        default :
                            throw new ArgumentException("ToSql - Not supported expression: {0}".Args(expression.ToString()));
                    }
                }

                case ExpressionType.Constant :
                {
                    var constantExpression = (ConstantExpression) expression;
                    Contract.Assume(constantExpression.Value != null);

                    if (constantExpression.Type == typeof(string))
                        return "'{0}'".Args(constantExpression.Value.ToString().Replace("'", "''"));
                    else
                        return constantExpression.Value.ToString();
                }

                default :
                    throw new ArgumentException("ToSql - Not supported expression: {0}".Args(expression.ToString()));
            }
        }
    }
}
