using GenericDbRestApi.Types;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;

namespace GenericDBRestApi.Formatters
{
    public class QueryJsonFormatter : IQueryFormatter
    {
        public string ContentType => "application/json";
        public string OutputFormat => "json";

        public ActionResult GetActionResult(GenericQueryResult queryResult)
        {
            return new JsonResult(queryResult);
        }
    }
}
