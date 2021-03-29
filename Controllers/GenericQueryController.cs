using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using testwebapi.DataLayer;
using testwebapi.Managers;

namespace testwebapi.Controllers
{
    [ApiController]
    [Route("/dbapi/{query}")]
    public class GenericQueryController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "BLAAAH"
        };

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

            var result = manager.GetQueryResults(query, offset, maxRows, parameters);

            outputFormat = outputFormat ?? "json";

            if (outputFormat == "json")
            {
                var ret = new JsonResult(result);
                return ret;
            }
            else if (outputFormat == "excel")
            {
                var ret = new OkObjectResult(result);
                return ret;
            }
            else
            {
                var ret = new NotFoundObjectResult($"Unknown output format '{outputFormat}'");

                return ret;
            }
            
           
        }
    }
}
