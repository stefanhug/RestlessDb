using GenericDbRestApi.Lib.Types;
using System.Collections.Generic;

namespace GenericDbRestApi.Lib.DataLayer
{
    public interface IGenericSqlHelper
    {
        (List<Dictionary<string, object>> data, bool hasMoreRows) QueryAsDictList(string sqlStatement, int maxRows, Dictionary<string, object> parameters = null);
        Dictionary<string, object> QuerySingleRow(string sqlStatement, Dictionary<string, object> parameters = null);
        List<QueryColumn> QueryResultColumns(string sqlStatement);
    }
}