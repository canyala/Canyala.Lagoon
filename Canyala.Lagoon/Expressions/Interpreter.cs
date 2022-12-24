//
// Copyright (c) 2013 Canyala Innovation AB
//
// All rights reserved.
//

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Canyala.Lagoon.Expressions
{
    public class Interpreter
    {
        public static double Evaluate(string expression, Symbols symbols = null)
        {
            if (symbols == null)
                symbols = new Symbols();

            Interpreter interpreter = new Interpreter(expression, symbols);
            return interpreter.Value;
        }

        public Interpreter(string expression, Symbols symbols = null)
        {
            Symbols = symbols ?? new Symbols();
            Tokens = new Tokenizer(expression);
        }

        public double Value
        {
            get
            {
                Tokens.Initialize();
                return InterpretExpression();
            }
        }

        public Symbols Symbols { get; set; }
        private Tokenizer Tokens;

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
                        value = value + InterpretTerm();
                        break;
                    case "-":
                        value = value - InterpretTerm();
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
                        value = value * InterpretFactor();
                        break;
                    case "/":
                        value = value / InterpretFactor();
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
}
