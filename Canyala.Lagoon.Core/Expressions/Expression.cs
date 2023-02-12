using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Canyala.Lagoon.Core.Expressions
{
    public abstract class Expression
    {
        public Symbols Symbols { get; set; }

        public Expression(Symbols? symbols)
        {
            Symbols = symbols ?? new Symbols();
        }

        public abstract double Evaluate();
    }
}
