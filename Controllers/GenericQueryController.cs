using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using GenericDbRestApi.DataLayer;
using GenericDbRestApi.Managers;
using System.Net;
using GenericDBRestApi.Formatters;

namespace testwebapi.Controllers
{
    [ApiController]
    [Route("/dbapi/{query}")]
    public class GenericQueryController : ControllerBase
    {
        private readonly ILogger<GenericQueryController> logger;
        private readonly GenericQueryManager manager;
        
        public GenericQueryController(ILogger<GenericQueryController> logger, GenericQueryManager manager)
        {
            this.logger = logger;
            this.manager = manager;
        }

        [HttpGet]
        public IActionResult Get(string query, [FromQuery(Name = "offset")] int? offset, 
            [FromQuery(Name = "maxrows")] int? maxRows, [FromQuery(Name = "outputformat")] string outputFormat)
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
            logger.LogInformation($"query {query} executed; {queryResult.Data.Count()} rows");


            outputFormat = outputFormat ?? "json";

            if (outputFormat == "json")
            {
                var ret = new JsonResult(queryResult);
                return ret;
            }
            else if (outputFormat == "excel")
            {
                var ret = new GenericQueryResultExcelConverter(queryResult).GetAsFileResult();
                return ret;
            }
            else
            {
                var ret = new ContentResult();
                ret.Content = $"Unknown output format '{outputFormat}";
                ret.ContentType = "text/plain";
                ret.StatusCode = (int)HttpStatusCode.NotAcceptable;

                return ret;
            }
            
           
        }
    }
}
