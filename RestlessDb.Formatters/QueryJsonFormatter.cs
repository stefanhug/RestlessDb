using RestlessDb.Common.Types;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;

namespace RestlessDb.Formatters
{
    public class QueryJsonFormatter : IQueryFormatter
    {
        public virtual string ContentType => "application/json";
        public virtual string OutputFormat => "json";
        public virtual string Label => "JSON file";
        public virtual string Description => "Data export in JSON value format";
        public virtual string FileExtension => "json";
        public virtual Disposition Disposition => Disposition.STANDALONE;

        public ActionResult GetActionResult(QueryResult queryResult)
        {
            return new JsonResult(queryResult);
        }
    }
}
