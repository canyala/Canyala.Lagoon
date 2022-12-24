//
// Copyright (c) 2013 Canyala Innovation AB
//
// All rights reserved.
//

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Text.RegularExpressions;

namespace Canyala.Lagoon.Expressions
{
    public class Compiler
    {
        static public double Evaluate(string expression, Symbols symbols = null)
        {
            if (symbols == null)
                symbols = new Symbols();

            Parser parser = new Parser(expression, symbols);
            Expression<Func<double>> lambdaExpression = Expression.Lambda<Func<double>>(parser.ExpressionTree);
            Func<double> compiledExpression = lambdaExpression.Compile();
            return compiledExpression();
        }

        public Compiler(string expression, Symbols symbols = null)
        {
            if (symbols == null)
                symbols = new Symbols();

            Symbols = symbols;
            Parser parser = new Parser(expression, symbols);
            Expression<Func<double>> lambdaExpression = Expression.Lambda<Func<double>>(parser.ExpressionTree);
            compiledExpression = lambdaExpression.Compile();
        }

        public double Value { get { return compiledExpression(); } }
        public Symbols Symbols { get; set; }

        private Func<double> compiledExpression;
    }
}

