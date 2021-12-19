using RestlessDb.Managers;
using RestlessDb.Common.Types;
using RestlessDb.Formatters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace RestlessDb.Controllers
{
    [ApiController]
    [Route("/dbapi/{query}")]
    public class QueryController : ControllerBase
    {
        private readonly ILogger<QueryController> logger;
        private readonly GenericQueryManager manager;
        private readonly IEnumerable<IQueryFormatter> queryFormatters;

        public QueryController(ILogger<QueryController> logger, GenericQueryManager manager,
            IEnumerable<IQueryFormatter> queryFormatters)
        {
            this.logger = logger;
            this.manager = manager;
            this.queryFormatters = queryFormatters;
        }

        [HttpGet]
        public IActionResult Get(string query, [FromQuery(Name = "offset")] int? offset,
            [FromQuery(Name = "maxrows")] int? maxRows,
            [FromQuery(Name = "outputformat")] string outputFormat)
        {
            int startTickCount = Environment.TickCount;

            Dictionary<string, object> parameters = new Dictionary<string, object>(StringComparer.InvariantCultureIgnoreCase);

            var paramsToRemove = new HashSet<string>() { "offset", "maxrows", "outputformat" };
            foreach (var tuple in HttpContext.Request.Query)
            {
                if (!paramsToRemove.Contains(tuple.Key.ToLowerInvariant()))
                {
                    parameters.Add(tuple.Key.ToUpperInvariant(), tuple.Value);
                }
            }

            try
            {
                var queryResult = manager.GetQueryResults(query, offset, maxRows, parameters);

                logger.LogInformation($"query {query} executed OK; {queryResult.Data.Count()} rows");
                outputFormat = outputFormat ?? "json";

                var formatter = queryFormatters.FirstOrDefault(f => outputFormat.Equals(f.OutputFormat, StringComparison.InvariantCultureIgnoreCase));

                if (formatter == null)
                {
                    throw new GenericDbQueryException(GenericDbQueryExceptionCode.FORMATTER_NOTFOUND,
                        $"Unknown output format '{outputFormat}'");
                }

                return formatter.GetActionResult(queryResult);
            }
            catch (Exception e)
            {
                return ControllerHelper.HandleException(e, logger);
            }
            finally
            {
                logger.LogInformation("Get: {0}, offset:{1}, maxRows: {2}, params: [{3}] ({4} ms)",
                    query, offset, maxRows, string.Join(", ", parameters), Environment.TickCount - startTickCount);
            }
        }
    }
}
