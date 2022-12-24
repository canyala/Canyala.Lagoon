/*
 
  MIT License

  Copyright (c) 2012-2022 Canyala Innovation

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

using System.Globalization;
using System.Linq.Expressions;

namespace Canyala.Lagoon.Expressions;

internal class Parser
{
    internal Expression ExpressionTree { get; private set; }

    internal Parser(string expression, Symbols symbols)
    {
        Symbols = symbols;
        Tokens = new Tokenizer(expression);
        ExpressionTree = ParseExpression();
    }

    private readonly Tokenizer Tokens;
    private readonly Symbols Symbols;

    private Expression ParseExpression()
    {
        Expression? expression = null;

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
