using Microsoft.Extensions.Logging;
using RestlessDb.Repositories;
using RestlessDb.Common.Types;
using System;
using System.Collections.Generic;
using System.Text;

namespace RestlessDb.Managers
{
    public class QueryConfigManager
    {
        private readonly ILogger<QueryConfigManager> logger;
        private readonly QueryConfigRepository queryConfigRepository;

        public QueryConfigManager(QueryConfigRepository queryConfigRepository, ILogger<QueryConfigManager> logger)
        {
            this.logger = logger;
            this.queryConfigRepository = queryConfigRepository;
        }

        public QueryConfigResult GetQueryResults()
        {
            return queryConfigRepository.GetAllQueries();
        }
    }
}
