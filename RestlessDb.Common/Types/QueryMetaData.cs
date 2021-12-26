using System;
using System.Linq;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace RestlessDb.Common.Types
{
    public enum QueryColumnType { STRING, INT, DOUBLE, DATETIME, DECIMAL, SHORT }

    public class QueryColumn
    {
        public string Label { get; set; }
        public QueryColumnType ColumnType { get; set; }
    }

    [DataContract]
    public class QueryMetaData
    {
        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public string Label { get; set; }

        [DataMember]
        public string Description { get; set; }

        [DataMember]
        public List<QueryColumn> Columns { get; set; }

        [DataMember]
        public HashSet<string> Parameters { get; set; }

        [DataMember]
        public List<QueryMetaData> Children { get; set; }

        public static QueryMetaData BuildFromQueryItemExt(QueryItemExt queryItemExt)
        {
            var ret = new QueryMetaData();
            ret.Name = queryItemExt.QueryItem.Name;
            ret.Label = queryItemExt.QueryItem.Label;
            ret.Description = queryItemExt.QueryItem.Description;
            ret.Columns = queryItemExt.Columns;
            ret.Parameters = queryItemExt.Parameters;
            if (queryItemExt.Children != null)
            {
                ret.Children = (from a in queryItemExt.Children select BuildFromQueryItemExt(a)).ToList();
            }

            return ret;
        }

    }

}
