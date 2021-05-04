using GenericDbRestApi.Lib.Types;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GenericDbRestApi.Lib.DataLayer
{
    public class QueryItem
    {
        public string Name { get; set; }
        public string Label { get; set; }
        public string Description { get; set; }
        public string Sql { get; set; }

        public List<QueryColumn> Columns { get; set; }
        public List<QueryItem> ChildItems { get; set; }

        public QueryMetaData AsQueryMetaData()
        {
            var ret = new QueryMetaData();
            ret.Name = Name;
            ret.Label = Label;
            ret.Description = Description;
            ret.Columns = Columns;
            if (ChildItems != null)
            {
                ret.Children = (from a in ChildItems select a.AsQueryMetaData()).ToList();
            }

            return ret;
        }
    }
}
