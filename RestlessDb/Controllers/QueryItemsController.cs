using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RestlessDb.Managers;
using RestlessDb.Common.Types;
using System;
using System.Net;

namespace RestlessDb.Controllers
{
    [Route("/dbapi/admin/[controller]")]
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
                var queryResult = manager.GetAll();
                return new JsonResult(queryResult);
            }
            catch (Exception e)
            {
                return ControllerHelper.HandleException(e, logger);
            };
        }

        [HttpGet("{name}")]
        public IActionResult Get(string name)
        {
            try
            {
                var queryResult = manager.Get(name);
                return new JsonResult(queryResult);
            }
            catch (Exception e)
            {
                return ControllerHelper.HandleException(e, logger);
            };
        }

        [HttpPut]
        public IActionResult Put([Bind("Id,Name,Label,Description,Sql,Parent,Pos")] QueryItem queryItem)
        {
            try
            {
                var itm = manager.Update(queryItem);
                return Ok(itm);
                
            }
            catch (Exception e)
            {
                return ControllerHelper.HandleException(e, logger);
            };
        }

        [HttpPost]
        public IActionResult Post([Bind("Id,Name,Label,Description,Sql,Parent,Pos")] QueryItem queryItem)
        {
            try
            {
                var itm = manager.Insert(queryItem);
                return Created($"/dbapi/admin/queryitems/{itm.Name}", itm);
            }
            catch (Exception e)
            {
                return ControllerHelper.HandleException(e, logger);
            };
        }

        [HttpDelete("{name}")]
        public IActionResult Delete(string name)
        {
            try
            {
                manager.Delete(name);
                return Ok();
            }
            catch (Exception e)
            {
                return ControllerHelper.HandleException(e, logger);
            };
        }
    }
}
