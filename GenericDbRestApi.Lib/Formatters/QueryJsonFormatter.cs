using GenericDbRestApi.Lib.Types;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;

namespace GenericDBRestApi.Lib.Formatters
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
