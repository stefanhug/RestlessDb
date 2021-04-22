using System.Collections.Generic;
using System.Runtime.Serialization;

namespace GenericDbRestApi.Types
{
    public enum QueryColumnType { STRING, INT, DOUBLE, DATETIME }

    public class QueryColumn
    {
        public string Label { get; set; }
        public QueryColumnType ColumnType { get; set; }
    }

    public enum GenericQueryResultStatus { OK, QRY_NOTFOUND, QRY_ERROR, SERVER_ERROR }
        
    [DataContract]
    public class GenericQueryResult
    {
        [DataMember]
        public GenericQueryResultStatus Status { get; set; } = GenericQueryResultStatus.OK;
        [DataMember]
        public string ErrorMessage { get; set; }
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public string Label { get; set; }
        [DataMember]
        public string Description { get; set; }
        [DataMember]
        public int Offset { get; set; }
        [DataMember]
        public int MaxRows { get; set; }
        [DataMember]
        public List<QueryColumn> Columns { get; set; }
        [DataMember]
        public List<Dictionary<string, object>> Data { get; set; }
    }
}
