using Microsoft.Extensions.Logging;
using RestlessDb.DataLayer;
using RestlessDb.Common.Types;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RestlessDb.Repositories
{
    public class QueryConfigRepository
    {
        private const int MAX_CONFIG_ITEMS = 8000;
        private const string QRYITEM_SQL = @"
            select Name, Label, Description, Sql 
            from GQuery.QueryItem
            where Parent is null
            order by Label
        ";

        private readonly IGenericSqlHelper genericSqlHelper;
        private readonly ILogger<QueryConfigRepository> logger;
        private readonly QueryItemProvider queryItemProvider;

        public QueryConfigRepository(IGenericSqlHelper genericSqlHelper, QueryItemProvider queryItemProvider, ILogger<QueryConfigRepository> logger)
        {
            this.genericSqlHelper = genericSqlHelper ?? throw new Exception("not found");
            this.logger = logger;
            this.queryItemProvider = queryItemProvider;
        }

        public QueryAdminResult GetAllQueryItems()
        {
            var ret = new QueryAdminResult();

            ret.QueryAdminItems = queryItemProvider.LoadQueryItems();

            return ret;
        }

        public QueryConfigResult GetAllQueries()
        {
            var ret = new QueryConfigResult();

            ret.QueryConfigItems = GetTopLevelQueryItems();

            return ret;
        }

        private List<QueryConfigItem> GetTopLevelQueryItems()
        {
            // reasonable to use dapper or EF?
            var (queryItems, hasMoreRows) = genericSqlHelper.QueryAsDictList(QRYITEM_SQL, 0, MAX_CONFIG_ITEMS);

            var ret =
                from a
                in queryItems
                select new QueryConfigItem()
                {
                    Name = ((string)a["Name"]).ToLowerInvariant(),
                    Label = (string)a["Label"],
                    Description = (string)a["Description"],
                    Parameters = QueryParamsParser.GetQueryParams((string)a["Sql"])
                };
            return ret.ToList();
        }
    }
}
