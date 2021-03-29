using System.Collections.Generic;
using GenericDbRestApi.DataLayer;
using GenericDbRestApi.Types;
using Microsoft.Extensions.Logging;

namespace GenericDbRestApi.Managers
{
    public class GenericQueryManager
    {
        const int MAXROWS = 8000;
        private readonly ILogger<GenericQueryManager> logger;
        private readonly GenericQueryRepository queryRepository;

        public GenericQueryManager(ILogger<GenericQueryManager> logger, GenericQueryRepository queryRepository)
        {
            this.logger = logger;
            this.queryRepository = queryRepository;
        }

        public GenericQueryResult GetQueryResults(string query, int? offset, int? maxRows, Dictionary<string, string> parameters)
        {
            offset ??= 0;
            maxRows ??= MAXROWS;
            return queryRepository.GetQueryResults(query, offset.Value, maxRows.Value, parameters);
        }
    }
}
