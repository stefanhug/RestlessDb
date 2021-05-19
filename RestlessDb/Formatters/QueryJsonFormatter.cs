using RestlessDb.Types;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;

namespace RestlessDb.Formatters
{
    public class QueryJsonFormatter : IQueryFormatter
    {
        public string ContentType => "application/json";
        public string OutputFormat => "json";

        public ActionResult GetActionResult(QueryResult queryResult)
        {
            return new JsonResult(queryResult);
        }
    }
}
