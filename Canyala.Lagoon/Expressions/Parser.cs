//
// Copyright (c) 2013 Canyala Innovation AB
//
// All rights reserved.
//

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace Canyala.Lagoon.Expressions
{
    internal class Parser
    {
        internal Expression ExpressionTree { get; private set; }

        internal Parser(string expression, Symbols symbols)
        {
            Symbols = symbols;
            Tokens = new Tokenizer(expression);
            ExpressionTree = ParseExpression();
        }

        private Tokenizer Tokens;
        private Symbols Symbols;

        private Expression ParseExpression()
        {
            Expression expression = null;

            if (Tokens.Accept(token => token == "+"))
                expression = ParseTerm();
            else if (Tokens.Accept(token => token == "-"))
                expression = Expression.Negate(ParseTerm());
            else
                expression = ParseTerm();

            while (Tokens.Allow(token => token == "+" || token == "-"))
            {
                switch (Tokens.Read())
                {
                    case "+":
                        expression = Expression.Add(expression, ParseTerm());
                        break;
                    case "-":
                        expression = Expression.Subtract(expression, ParseTerm());
                        break;
                }
            }

            return expression;
        }

        private Expression ParseTerm()
        {
            Expression expression = ParseFactor();

            while (Tokens.Allow(token => token == "*" || token == "/"))
            {
                switch (Tokens.Read())
                {
                    case "*":
                        expression = Expression.Multiply(expression, ParseFactor());
                        break;
                    case "/":
                        expression = Expression.Divide(expression, ParseFactor());
                        break;
                }
            }

            return expression;
        }

        private Expression ParseFactor()
        {
            Expression expression;

            if (Tokens.Accept(token => token == "("))
            {
                expression = ParseExpression();
                Tokens.Expect(token => token == ")");
            }
            else
            {
                if (Tokens.Allow(token => char.IsDigit(token[0])))
                {
                    expression = Expression.Constant(Double.Parse(Tokens.Read(), CultureInfo.InvariantCulture.NumberFormat));
                }
                else if (Tokens.Allow(token => char.IsLetter(token[0]) || token[0] == '?'))
                {
                    Expression[] argumentValues = { Expression.Constant(Tokens.Read()) };
                    expression = Expression.Call(Expression.Constant(Symbols), typeof(Symbols).GetMethod("ValueOf"), argumentValues);
                }
                else
                {
                    expression = ParseExpression();
                }
            }

            return expression;
        }
    }
}
