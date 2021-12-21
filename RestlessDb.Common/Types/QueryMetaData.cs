using System;
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
        public List<QueryMetaData> Children { get; set; }
    }

}
