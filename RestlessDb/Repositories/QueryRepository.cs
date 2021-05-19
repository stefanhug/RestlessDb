using RestlessDb.Types;
using RestlessDb.DataLayer;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Collections.Generic;

namespace RestlessDb.Repositories
{
    public class QueryRepository
    {
        // Max number of rows to retrieve for a child query
        const int MAX_CHILD_ROWS = 10000;

        public QueryRepository(IGenericSqlHelper genericSqlHelper, QueryItemProvider queryItemProvider, 
            QueryParamsProvider queryParamsProvider,ILogger<QueryRepository> logger) 
        {
            this.GenericSqlHelper = genericSqlHelper;
            this.QueryItemProvider = queryItemProvider;
            this.QueryParamsProvider = queryParamsProvider;
            this.logger = logger;
        }

        public QueryResult GetQueryResults(string queryName, int offset, int maxRows, 
            Dictionary<string, object> queryParameters)
        {
            var ret = new QueryResult();
            ret.Offset = offset;
            ret.MaxRows = maxRows;
            ret.QueryParameters = queryParameters;

            var queryItem = QueryItemProvider.LoadQueryItem(queryName);
                
            // check the query parameters with the QueryParamsProvider
            var checkedParmeters = QueryParamsProvider.CollectParamsForQuery(queryItem.Sql, queryParameters, new List<Dictionary<string, object>>());

            (ret.Data, ret.HasMoreRows) = GenericSqlHelper.QueryAsDictList(queryItem.Sql, offset, maxRows, checkedParmeters);

            // ret.MetaData.Children will be filled in the recursive query
            ret.MetaData = queryItem.AsQueryMetaData();

            if (queryItem.ChildItems != null)
            {
                foreach (var dataRow in ret.Data)
                {
                    RecurseDataChildren(new List<Dictionary<string, object>>(){ dataRow }, queryItem.ChildItems, queryParameters);
                }
            }
                
            ret.RetrievedRows = ret.Data.Count;

            return ret;
        }

        private void RecurseDataChildren(List<Dictionary<string, object>> parentRowStack, List<QueryItem> childItems, Dictionary<string, object> queryParameters)
        {
            if (childItems != null)
            {
                foreach (var childItem in childItems)
                {
                    RecurseDataChild(parentRowStack, childItem, queryParameters);
                }
            }
        }

        private void RecurseDataChild(List<Dictionary<string, object>> parentRowStack, QueryItem queryItem, Dictionary<string, object> queryParameters)
        {
            string sqlStmt = queryItem.Sql;
            string sql = $"{sqlStmt} offset 0 rows fetch next {MAX_CHILD_ROWS + 1} rows only";

            logger.LogInformation("GetQueryResults-Child SQL: {0}", sql);

            List<Dictionary<string, object>> childRows;
            bool hasMoreRows;

            var effectiveQueryParams = QueryParamsProvider.CollectParamsForQuery(sql, queryParameters, parentRowStack);
            (childRows, hasMoreRows) = GenericSqlHelper.QueryAsDictList(sql, 0, MAX_CHILD_ROWS, effectiveQueryParams);

            // the last item is the current parent
            parentRowStack.Last().Add(queryItem.Name, childRows);

            if (queryItem.ChildItems != null)
            {
                foreach (var childRow in childRows)
                {
                    var newParentRowStack = new List<Dictionary<string, object>>(parentRowStack);
                    newParentRowStack.Add(childRow);
                    RecurseDataChildren(newParentRowStack, queryItem.ChildItems, queryParameters);
                }
            }
        }

        public IGenericSqlHelper GenericSqlHelper { get; }
        internal QueryItemProvider QueryItemProvider { get; }
        public QueryParamsProvider QueryParamsProvider { get; }

        private readonly ILogger<QueryRepository> logger;
    }

}