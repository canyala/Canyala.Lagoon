//
// Copyright (c) 2013 Canyala Innovation AB
//
// All rights reserved.
//

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Canyala.Lagoon.Contracts
{
    public static class Contract
    {
        [Conditional("DEBUG")]
        public static void Assume(bool expression, string message)
        {
            if (!expression)
                throw new ContractFailedException(message);
        }

        [Conditional("DEBUG")]
        public static void Requires(bool expression, string message)
        {
            Assume(expression, "PRE: " + message);
        }

        [Conditional("DEBUG")]
        public static void Ensures(bool expression, string message)
        {
            Assume(expression, "POST: " + message);
        }
    }
}
