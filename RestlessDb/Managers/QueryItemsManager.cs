using Microsoft.Extensions.Logging;
using RestlessDb.Repositories;
using RestlessDb.Common.Types;
using System;
using System.Collections.Generic;
using System.Text;

namespace RestlessDb.Managers
{
    public class QueryItemsManager
    {
        private readonly ILogger<QueryItemsManager> logger;
        private readonly QueryConfigRepository queryConfigRepository;

        public QueryItemsManager(QueryConfigRepository queryConfigRepository, ILogger<QueryItemsManager> logger)
        {
            this.logger = logger;
            this.queryConfigRepository = queryConfigRepository;
        }

        public QueryAdminResult GetAllQueryItems()
        {
            return queryConfigRepository.GetAllQueryItems();
        }
    }
}
