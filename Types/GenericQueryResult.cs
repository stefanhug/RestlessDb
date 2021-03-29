using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GenericDbRestApi.Types
{
    public enum QueryColumnType { STRING, INT, DOUBLE, DATETIME }

    public class QueryColumn
    {
        public string Label { get; set; }
        public QueryColumnType ColumnType { get; set; }
    }

    public enum GenericQueryResultStatus { OK, QRY_NOTFOUND, QRY_ERROR, SERVER_ERROR }
        
    public class GenericQueryResult
    {
        public GenericQueryResult()
        {
        }

        public GenericQueryResultStatus Status { get; set; } = GenericQueryResultStatus.OK;
        public string ErrorMessage { get; set; }
        
        public string Name { get; set; }
        public string Label { get; set; }
        public string Description { get; set; }
        public int Offset { get; set; }
        public int MaxRows { get; set; }
        public IEnumerable<QueryColumn> Columns { get; set; }
        public IEnumerable<Dictionary<string, object>> Data { get; set; }
    }
}
