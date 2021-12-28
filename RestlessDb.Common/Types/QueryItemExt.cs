using System.Collections.Generic;
using System.Linq;

namespace RestlessDb.Common.Types
{
    public class QueryItemExt
    {
        public QueryItem QueryItem { get; set; }

        public HashSet<string> Parameters { get; set; }

        public List<QueryColumn> Columns { get; set; }

        public List<QueryItemExt> Children { get; set; }
    }
}
