using GenericDbRestApi.Lib.Managers;
using GenericDbRestApi.Lib.Types;
using GenericDBRestApi.Lib.Formatters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace GenericDbRestApi.Lib.Controllers
{
    [ApiController]
    [Route("/dbapi/{query}")]
    public class QueryController : ControllerBase
    {
        public static readonly Dictionary<GenericDbQueryExceptionCode, HttpStatusCode> ExceptionToStatusCodeMap =
            new Dictionary<GenericDbQueryExceptionCode, HttpStatusCode>()
            {
                {GenericDbQueryExceptionCode.DBQUERY, HttpStatusCode.InternalServerError},
                {GenericDbQueryExceptionCode.FORMATTER_NOTFOUND, HttpStatusCode.NotImplemented},
                {GenericDbQueryExceptionCode.QUERY_NOTFOUND, HttpStatusCode.NotFound},
                {GenericDbQueryExceptionCode.RECURSION, HttpStatusCode.InternalServerError},
                {GenericDbQueryExceptionCode.PARAMS_MISSING, HttpStatusCode.BadRequest},
                {GenericDbQueryExceptionCode.PARAMS_NOTNEEDED, HttpStatusCode.BadRequest},
            };

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
            catch (GenericDbQueryException e)
            {
                var msg = $"Exception: {e.ExceptionCode.ToString()}\r\n{e.Message}";
                return new ContentResult()
                {
                    Content = msg,
                    ContentType = "text/plain",
                    StatusCode = (int)ExceptionToStatusCodeMap[e.ExceptionCode]
                };
            }
            catch (Exception e)
            {
                return new ContentResult()
                {
                    Content = $"Exception: {e.Message}",
                    ContentType = "text/plain",
                    StatusCode = (int)HttpStatusCode.InternalServerError
                };
            };
        }
    }
}
