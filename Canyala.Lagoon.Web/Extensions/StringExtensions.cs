//-------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace Canyala.Lagoon.Web.Extensions
{
    public static class StringExtensions
    {
        public static MvcHtmlString ToMvcHtmlString(this string text)
        {
            return new MvcHtmlString(text);
        }
    }
}
