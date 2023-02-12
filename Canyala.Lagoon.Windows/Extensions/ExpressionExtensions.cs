//-------------------------------------------------------------------------------
//
//  MIT License
//
//  Copyright (c) 2012-2022 Canyala Innovation (Martin Fredriksson)
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


using System.Diagnostics.Contracts;
using System.Linq.Expressions;

using Canyala.Lagoon.Core.Extensions;
namespace Canyala.Lagoon.Windows.Extensions;

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

                Contract.Assume(methodCallExpression.Arguments.Count > 0);
                Contract.Assume(memberExpression != null);

                return methodCallExpression.Method.Name switch
                {
                    "StartsWith" => "[{0}] LIKE '{1}%'".Args(memberExpression.Member.Name, ToSql(methodCallExpression.Arguments[0])),
                    "Contains" => "[{0}] LIKE '%{1}%'".Args(memberExpression.Member.Name, ToSql(methodCallExpression.Arguments[0])),
                    "EndsWith" => "[{0}] LIKE '%{1}'".Args(memberExpression.Member.Name, ToSql(methodCallExpression.Arguments[0])),

                    _ => throw new ArgumentException("ToSql - Not supported expression: {0}".Args(expression.ToString())),
                };
            }

            case ExpressionType.Constant :
            {
                var constantExpression = expression as ConstantExpression;
                Contract.Assume(constantExpression?.Value != null);

                string valueAsString = constantExpression.Value.ToString() ?? string.Empty;
                    if (constantExpression.Type == typeof(string))
                        return "'{0}'".Args(valueAsString.Replace("'", "''"));
                    else
                        return valueAsString;
            }

            default :
                throw new ArgumentException("ToSql - Not supported expression: {0}".Args(expression.ToString()));
        }
    }
}
