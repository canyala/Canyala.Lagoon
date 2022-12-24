//
// Copyright (c) 2013 Canyala Innovation AB
//
// All rights reserved.
//

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Canyala.Lagoon.Expressions
{
    public class Symbols
    {
        private Dictionary<string, double> dictionary = null;

        public Symbols()
        {
            dictionary = new Dictionary<string, double>();
        }

        public Symbols(string name, double value) : this()
        {
            Assign(name, value);
        }

        public double this[string name]
        {
            get { return ValueOf(name); }
            set { Assign(name, value); }
        }

        public double ValueOf(string name)
        {
            double value;
            if (dictionary.TryGetValue(name, out value))
                return value;
            return 0.0;
        }

        public void Assign(string name, double value)
        {
            dictionary[name] = value;
        }
    }
}
