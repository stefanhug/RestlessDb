using GenericDBRestApi.Managers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;


namespace testwebapi.Controllers
{
    [Microsoft.AspNetCore.Mvc.ApiController]
    [Route("index.html")]
    public class MarkdownRenderController : ControllerBase
    {
        private readonly ILogger<MarkdownRenderController> logger;
        private readonly MarkdownRenderManager manager;
       
        public MarkdownRenderController(ILogger<MarkdownRenderController> logger, MarkdownRenderManager manager)
        {
            this.logger = logger;
            this.manager = manager;
        }

        [HttpGet]
        public IActionResult Get()
        {
            return new ContentResult()
            {
                Content = manager.GetMdFileAsHtml("README.md"),
                ContentType = "text/html",
                StatusCode = (int)HttpStatusCode.OK
            };
        }
    }
}
