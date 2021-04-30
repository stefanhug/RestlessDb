using System.Collections.Generic;
using System.Runtime.Serialization;

namespace GenericDbRestApi.Lib.Types
{
    public enum GenericQueryResultStatus { OK, QRY_NOTFOUND, QRY_ERROR, SERVER_ERROR }

    [DataContract]
    public class QueryResult
    {
        [DataMember]
        public GenericQueryResultStatus Status { get; set; } = GenericQueryResultStatus.OK;
        [DataMember]
        public string ErrorMessage { get; set; }
        [DataMember]
        public int Offset { get; set; }
        [DataMember]
        public int MaxRows { get; set; }
        [DataMember]
        public Dictionary<string, object> QueryParameters { get; set; }
        [DataMember]
        public int RetrievedRows { get; set; }
        [DataMember]
        public bool HasMoreRows { get; set; }



        [DataMember]
        public QueryMetaData MetaData { get; set; }
        [DataMember]
        public List<Dictionary<string, object>> Data { get; set; }
    }
}
