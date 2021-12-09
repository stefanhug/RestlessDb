using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RestlessDb.Managers;
using RestlessDb.Common.Types;
using System;
using System.Net;

namespace RestlessDb.Controllers
{
    [Route("/dbapi/admin/queryitems")]
    [ApiController]
    public class QueryItemsController : ControllerBase
    {

        private readonly ILogger<QueryItemsController> logger;
        private readonly QueryItemsManager manager;


        public QueryItemsController(ILogger<QueryItemsController> logger, QueryItemsManager manager)
        {
            this.logger = logger;
            this.manager = manager;
        }

        [HttpGet]
        public IActionResult Get()
        {
            try
            {
                var queryResult = manager.GetAllQueryItems();

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
