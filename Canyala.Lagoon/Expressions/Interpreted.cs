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

using System.Globalization;

namespace Canyala.Lagoon.Expressions;

public class Interpreted : Expression
{
    public static double Evaluate(string expression, Symbols? symbols = null)
    {
        Interpreted interpreted = new(expression, symbols ?? new Symbols());
        return interpreted.Evaluate();
    }

    public Interpreted(string expression, Symbols? symbols = null) : base(symbols)
    {
        Tokens = new Tokenizer(expression);
    }

    public override double Evaluate()
    {
        Tokens.Initialize();
        return InterpretExpression();
    }

    private readonly Tokenizer Tokens;

    private double InterpretExpression()
    {
        double value;

        if (Tokens.Accept(token => token == "+"))
            value = InterpretTerm();
        else if (Tokens.Accept(token => token == "-"))
            value = -InterpretTerm();
        else
            value = InterpretTerm();

        while (Tokens.Allow(token => token == "+" || token == "-"))
        {
            switch (Tokens.Read())
            {
                case "+":
                    value += InterpretTerm();
                    break;
                case "-":
                    value -= InterpretTerm();
                    break;
            }
        }

        return value;
    }

    private double InterpretTerm()
    {
        double value = InterpretFactor();

        while (Tokens.Allow(token => token == "*" || token == "/"))
        {
            switch (Tokens.Read())
            {
                case "*":
                    value *= InterpretFactor();
                    break;
                case "/":
                    value /= InterpretFactor();
                    break;
            }
        }

        return value;
    }

    private double InterpretFactor()
    {
        double value;

        if (Tokens.Accept(token => token == "("))
        {
            value = InterpretExpression();
            Tokens.Expect(token => token == ")");
        }
        else
        {
            if (Tokens.Allow(token => char.IsDigit(token[0])))
            {
                value = Double.Parse(Tokens.Read(), CultureInfo.InvariantCulture.NumberFormat);
            }
            else if (Tokens.Allow(token => char.IsLetter(token[0]) || token[0] == '?'))
            {
                value = Symbols.ValueOf(Tokens.Read());
            }
            else
            {
                value = InterpretExpression();
            }
        }

        return value;
    }
}
