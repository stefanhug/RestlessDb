using GenericDbRestApi.Managers;
using GenericDbRestApi.Types;
using GenericDBRestApi.Formatters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace testwebapi.Controllers
{
    [ApiController]
    [Route("/dbapi/{query}")]
    public class GenericQueryController : ControllerBase
    {
        private readonly ILogger<GenericQueryController> logger;
        private readonly GenericQueryManager manager;
        private readonly IEnumerable<IQueryFormatter> queryFormatters;

        public GenericQueryController(ILogger<GenericQueryController> logger, GenericQueryManager manager,
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
            Dictionary<string, string> parameters = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase);
            foreach (var tuple in HttpContext.Request.Query)
            {
                if (tuple.Key.ToLowerInvariant() != "offset" && tuple.Key.ToLowerInvariant() != "maxrows")
                {
                    parameters.Add(tuple.Key, tuple.Value);
                }
            }
            var queryResult = manager.GetQueryResults(query, offset, maxRows, parameters);

            if (queryResult.Status == GenericQueryResultStatus.OK)
            {
                logger.LogInformation($"query {query} executed OK; {queryResult.Data.Count()} rows");
                outputFormat = outputFormat ?? "json";

                var formatter = queryFormatters.FirstOrDefault(f => outputFormat.Equals(f.OutputFormat, StringComparison.InvariantCultureIgnoreCase));
                if (formatter != null)
                {
                    return formatter.GetActionResult(queryResult);
                }
                else
                {
                    return new ContentResult()
                    {
                        Content = $"Unknown output format '{outputFormat}'",
                        ContentType = "text/plain",
                        StatusCode = (int)HttpStatusCode.NotImplemented
                    };
                }
            }
            else
            {
                var ret = new JsonResult(queryResult);
                switch (queryResult.Status)
                {
                    case GenericQueryResultStatus.QRY_NOTFOUND:
                        ret.StatusCode = (int)HttpStatusCode.NotFound;
                        break;
                    case GenericQueryResultStatus.QRY_ERROR:
                        ret.StatusCode = (int)HttpStatusCode.InternalServerError;
                        break;
                    default:
                        ret.StatusCode = (int)HttpStatusCode.OK;
                        break;
                }
                return ret;
            }
        }
    }
}
