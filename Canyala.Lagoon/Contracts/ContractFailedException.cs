//
// Copyright (c) 2013 Canyala Innovation AB
//
// All rights reserved.
//

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Canyala.Lagoon.Contracts
{
    public class ContractFailedException : Exception
    {
        public ContractFailedException(string message) : base(message) { }
    }
}
