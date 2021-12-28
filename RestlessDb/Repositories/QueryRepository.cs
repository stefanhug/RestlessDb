using RestlessDb.Common.Types;
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

        private readonly IGenericSqlHelper genericSqlHelper;
        private readonly ILogger<QueryRepository> logger;

        public QueryRepository(IGenericSqlHelper genericSqlHelper, QueryItemsRepository queryItemsRepository,
            QueryParamsProvider queryParamsProvider, ILogger<QueryRepository> logger)
        {
            this.genericSqlHelper = genericSqlHelper;
            this.logger = logger;
        }

        public (List<Dictionary<string, object>> data, bool hasMoreRows) GetQueryItemData(QueryItemExt queryItemExt, Dictionary<string, object> effectiveQueryParams)
        {
            return genericSqlHelper.QueryAsDictList(queryItemExt.QueryItem.Sql, 0, MAX_CHILD_ROWS, effectiveQueryParams);
        }

        public (List<Dictionary<string, object>> data, bool hasMoreRows) GetQueryItemData(QueryItemExt queryItemExt, int offset, int maxRows, Dictionary<string, object> effectiveQueryParams)
        {
            return genericSqlHelper.QueryAsDictList(queryItemExt.QueryItem.Sql, 0, maxRows, effectiveQueryParams);
        }
    }
}