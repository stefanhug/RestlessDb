using Microsoft.Extensions.Logging;
using RestlessDb.Repositories;
using RestlessDb.Common.Types;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using RestlessDb.Formatters;

namespace RestlessDb.Managers
{
    //Todo: remove this class, consolidate with QueryItemsManager 
    public class QueryConfigManager
    {
        private readonly ILogger<QueryConfigManager> logger;
        private readonly QueryItemsRepository queryItemsRepository;
        private readonly IEnumerable<IQueryFormatter> queryFormatters;

        public QueryConfigManager(QueryItemsRepository queryItemsRepository, 
                                  ILogger<QueryConfigManager> logger,
                                  IEnumerable<IQueryFormatter> queryFormatters)
        {
            this.logger = logger;
            this.queryItemsRepository = queryItemsRepository;
            this.queryFormatters = queryFormatters;
        }

        public List<QueryMetaData> GetAllQueryMetaData()
        {
            var queryItemsExt = queryItemsRepository.GetAllQueryItemsExt();
            var ret = (from queryItemExt in queryItemsExt select QueryMetaData.BuildFromQueryItemExt(queryItemExt)).ToList();
            return ret;
        }

        public List<FormatterInfo> GetAllFormatters()
        {
            var ret = (from formatter 
                       in queryFormatters 
                       select new FormatterInfo() 
                       { 
                           OutputFormat = formatter.OutputFormat,
                           Name = formatter.Label,
                           Description = formatter.Description,
                           ContentType = formatter.ContentType,
                           FileExtension = formatter.FileExtension,
                           Disposition = formatter.Disposition

                       }).ToList();
            return ret;
        }
    }
}
