using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Canyala.Lagoon.Database;
using Canyala.Lagoon.Extensions;
using Canyala.Lagoon.Functional;

namespace Canyala.Lagoon.Database.Internal
{
    [Name("information_schema.tables")]
    internal class Table
    {
        [Name("TABLE_CATALOG")]
        public string TableCatalog { get; set; }
        [Name("TABLE_NAME")]
        public string TableName { get; set; }
        [Name("COLUMN_NAME")]
        public string ColumnName { get; set; }
        [Name("DATA_TYPE")]
        public string DataType { get; set; }
    }
}
