using RestlessDb.Types;
using System.Collections.Generic;

namespace RestlessDb.DataLayer
{
    public interface IGenericSqlHelper
    {
        (List<Dictionary<string, object>> data, bool hasMoreRows) QueryAsDictList(string sqlStatement, int offset, int maxRows, Dictionary<string, object> parameters = null);
        List<QueryColumn> QueryResultColumns(string sqlStatement);
    }
}