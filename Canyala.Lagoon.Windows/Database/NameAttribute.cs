using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Canyala.Lagoon.Database
{
    /// <summary>
    /// Provides a custom naming attribute for records and fields.
    /// </summary>
    public class NameAttribute : Attribute
    {
        public string Name { get; private set; }

        public NameAttribute(string name) 
        { 
            Name = name; 
        }
    }
}
