using RestlessDb.Common.Types;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RestlessDb.Common.Types
{
    public class QueryItemExt
    {
        public string Name { get; set; }
        public string Label { get; set; }
        public string Description { get; set; }
        public string Sql { get; set; }

        public List<QueryColumn> Columns { get; set; }
        public List<QueryItemExt> Children { get; set; }

        public QueryMetaData AsQueryMetaData()
        {
            var ret = new QueryMetaData();
            ret.Name = Name;
            ret.Label = Label;
            ret.Description = Description;
            ret.Columns = Columns;
            if (Children != null)
            {
                ret.Children = (from a in Children select a.AsQueryMetaData()).ToList();
            }

            return ret;
        }
    }
}
