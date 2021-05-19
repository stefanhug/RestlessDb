using System.Collections.Generic;
using RestlessDb.Repositories;
using RestlessDb.Types;
using Microsoft.Extensions.Logging;

namespace RestlessDb.Managers
{
    public class GenericQueryManager
    {
        const int MAXROWS = 8000;
        private readonly ILogger<GenericQueryManager> logger;
        private readonly QueryRepository queryRepository;

        public GenericQueryManager(ILogger<GenericQueryManager> logger, QueryRepository queryRepository)
        {
            this.logger = logger;
            this.queryRepository = queryRepository;
        }

        public QueryResult GetQueryResults(string query, int? offset, int? maxRows, Dictionary<string, object> parameters)
        {
            offset ??= 0;
            maxRows ??= MAXROWS;
            return queryRepository.GetQueryResults(query, offset.Value, maxRows.Value, parameters);
        }
    }
}
