using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RestlessDb.Common.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace RestlessDb.Controllers
{
    public static class ControllerHelper
    {
        public static ContentResult HandleException(Exception e, ILogger logger)
        {
            logger.LogError(e.Message);
            return new ContentResult()
            {
                Content = e.Message,
                ContentType = "text/plain",
                StatusCode = GetHttpStatusCode(e)
            };
        }

        private static int GetHttpStatusCode(Exception e)
        {
            return e is GenericDbQueryException ?
                    (int)ExceptionStatusToHttpStatusCodeMapper.GetHttpStatusCode((GenericDbQueryException)e) :
                    (int)HttpStatusCode.InternalServerError;
        }
    }
}
