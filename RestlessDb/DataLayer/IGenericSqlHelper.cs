using RestlessDb.Common.Types;
using System.Collections.Generic;

namespace RestlessDb.DataLayer
{
    public interface IGenericSqlHelper
    {
        public const int MAX_QRY_ROWS = 10000;

        (List<Dictionary<string, object>> data, bool hasMoreRows) QueryAsDictList(string sqlStatement, int offset = 0 , int maxRows = MAX_QRY_ROWS, Dictionary<string, object> parameters = null);
        List<QueryColumn> QueryResultColumns(string sqlStatement);
    }
}