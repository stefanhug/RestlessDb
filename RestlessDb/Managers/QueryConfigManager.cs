using Microsoft.Extensions.Logging;
using RestlessDb.Repositories;
using RestlessDb.Common.Types;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace RestlessDb.Managers
{
    //Todo: remove this class, consolidate with QueryItemsManager 
    public class QueryConfigManager
    {
        private readonly ILogger<QueryConfigManager> logger;
        private readonly QueryItemsRepository queryItemsRepository;

        public QueryConfigManager(QueryItemsRepository queryConfigRepository, ILogger<QueryConfigManager> logger)
        {
            this.logger = logger;
            this.queryItemsRepository = queryConfigRepository;
        }

        public List<QueryMetaData> GetAllQueryMetaData()
        {
            var queryItemsExt = queryItemsRepository.GetAllQueryItemsExt();
            var ret = (from queryItemExt in queryItemsExt select QueryMetaData.BuildFromQueryItemExt(queryItemExt)).ToList();
            return ret;
        }
    }
}
