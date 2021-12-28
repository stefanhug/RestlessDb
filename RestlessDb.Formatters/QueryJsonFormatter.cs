using RestlessDb.Common.Types;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;

namespace RestlessDb.Formatters
{
    public class QueryJsonFormatter : IQueryFormatter
    {
        public string ContentType => "application/json";
        public string OutputFormat => "json";
        public string Label => "JSON file";
        public string Description => "Data export in JSON value format";
        public string FileExtension => "json";
        public Disposition Disposition => Disposition.STANDALONE;

        public ActionResult GetActionResult(QueryResult queryResult)
        {
            return new JsonResult(queryResult);
        }
    }
}
