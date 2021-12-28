using System.Collections.Generic;
using RestlessDb.Repositories;
using RestlessDb.Common.Types;
using Microsoft.Extensions.Logging;
using RestlessDb.DataLayer;
using System.Linq;

namespace RestlessDb.Managers
{
    public class GenericQueryManager
    {
        const int MAXROWS = 8000;
        private readonly ILogger<GenericQueryManager> logger;
        private readonly QueryRepository queryRepository;
        private readonly QueryItemsRepository queryItemsRepository;
        private readonly QueryParamsProvider queryParamsProvider;

        public GenericQueryManager(ILogger<GenericQueryManager> logger, QueryRepository queryRepository, 
                                   QueryItemsRepository queryItemsRepository, QueryParamsProvider queryParamsProvider)
        {
            this.logger = logger;
            this.queryRepository = queryRepository;
            this.queryItemsRepository = queryItemsRepository;
            this.queryParamsProvider = queryParamsProvider;
        }

        public QueryResult GetQueryResults(string queryName, int? offset, int? maxRows, Dictionary<string, object> parameters)
        {
            offset ??= 0;
            maxRows ??= MAXROWS;
            return GetQueryResults(queryName, offset.Value, maxRows.Value, parameters);
        }

        public QueryResult GetQueryResults(string queryName, int offset, int maxRows,
            Dictionary<string, object> queryParameters)
        {
            var ret = new QueryResult
            {
                Offset = offset,
                MaxRows = maxRows,
                QueryParameters = queryParameters
            };

            var queryItemExt = queryItemsRepository.GetQueryItemExt(queryName);

            // check the query parameters with the QueryParamsProvider
            var checkedParmeters = queryParamsProvider.CollectParamsForQuery(queryItemExt.QueryItem.Sql, queryParameters, new List<Dictionary<string, object>>());

            (ret.Data, ret.HasMoreRows) = queryRepository.GetQueryItemData(queryItemExt, offset, maxRows, checkedParmeters);


            // ret.MetaData.Children will be filled in the recursive query
            ret.MetaData = QueryMetaData.BuildFromQueryItemExt(queryItemExt);

            if (queryItemExt.Children != null)
            {
                foreach (var dataRow in ret.Data)
                {
                    RecurseDataChildren(new List<Dictionary<string, object>>() { dataRow }, queryItemExt.Children, queryParameters);
                }
            }

            ret.RetrievedRows = ret.Data.Count;

            return ret;
        }

        private void RecurseDataChildren(List<Dictionary<string, object>> parentRowStack, List<QueryItemExt> childItems, Dictionary<string, object> queryParameters)
        {
            if (childItems != null)
            {
                foreach (var childItem in childItems)
                {
                    RecurseDataChild(parentRowStack, childItem, queryParameters);
                }
            }
        }

        private void RecurseDataChild(List<Dictionary<string, object>> parentRowStack, QueryItemExt queryItemExt, Dictionary<string, object> queryParameters)
        {
            logger.LogInformation("GetQueryResults-Child SQL: {0}", queryItemExt.QueryItem.Sql);

            List<Dictionary<string, object>> childRows;
            bool hasMoreRows;

            var effectiveQueryParams = queryParamsProvider.CollectParamsForQuery(queryItemExt.QueryItem.Sql, queryParameters, parentRowStack);
            (childRows, hasMoreRows) = queryRepository.GetQueryItemData(queryItemExt, effectiveQueryParams);


            // the last item is the current parent
            parentRowStack.Last().Add(queryItemExt.QueryItem.Name, childRows);

            if (queryItemExt.Children != null)
            {
                foreach (var childRow in childRows)
                {
                    var newParentRowStack = new List<Dictionary<string, object>>(parentRowStack);
                    newParentRowStack.Add(childRow);
                    RecurseDataChildren(newParentRowStack, queryItemExt.Children, queryParameters);
                }
            }
        }
    }
}

