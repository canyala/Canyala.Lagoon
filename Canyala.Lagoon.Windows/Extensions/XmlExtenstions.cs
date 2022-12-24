using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Canyala.Lagoon.Extensions
{
    public static class XmlExtenstions
    {
        public static string ElementValue(this XElement parent, string name)
        {
            return parent.Element(name).Value.Trim();
        }
    }
}
