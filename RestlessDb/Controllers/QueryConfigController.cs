using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RestlessDb.Common.Types;
using RestlessDb.Managers;
using System;
using System.Net;

namespace RestlessDb.Controllers
{
    [ApiController]
    public class QueryConfigController : ControllerBase
    {
        private readonly ILogger<QueryConfigController> logger;
        private readonly QueryConfigManager manager;

        public QueryConfigController(ILogger<QueryConfigController> logger, QueryConfigManager manager)
        {
            this.logger = logger;
            this.manager = manager;
        }

        [Route("/dbapiconfig/allqueries")]
        [HttpGet]
        public IActionResult Get()
        {
            try
            {
                var queryResult = manager.GetAllQueryMetaData();
                return new JsonResult(queryResult);
            }
            catch (Exception e)
            {
                return ControllerHelper.HandleException(e, logger);
            };
        }
    }
}
