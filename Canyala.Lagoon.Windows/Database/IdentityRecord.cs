using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Canyala.Lagoon.Database
{
    /// <summary>
    /// Provides a base class for updateable records.
    /// </summary>
    public class IdentityRecord
    {
        /// <summary>
        /// Provides an identity field.
        /// </summary>
        public Guid Id { get; set; }
    }
}
