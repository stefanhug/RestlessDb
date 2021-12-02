using System;
using System.Collections.Generic;
using System.Text;

namespace RestlessDb.Types
{
    public class QueryConfigResult
    {
        public List<QueryConfigItem> QueryConfigItems { get; set; }
    }

    public class QueryConfigItem
    {
        public string Name { get; set; }
        public string Label { get; set; }
        public string Description { get; set; }
        public HashSet<string> Parameters { get; set; }
    }

}
