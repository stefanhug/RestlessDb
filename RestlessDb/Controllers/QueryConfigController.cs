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
                var queryResult = manager.GetQueryResults();

                return new JsonResult(queryResult);
            }
            catch (GenericDbQueryException e)
            {
                var msg = $"Exception: {e.ExceptionCode.ToString()}\r\n{e.Message}";
                return new ContentResult()
                {
                    Content = msg,
                    ContentType = "text/plain",
                    StatusCode = (int)ExceptionStatusToHttpStatusCodeMapper.GetHttpStatusCode(e)
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
