using GenericDbRestApi.Lib.Formatters;
using GenericDbRestApi.Lib.Types;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Net;
using System.Text;

namespace GenericDBRestApi.Lib.Formatters
{
    public class QueryHtmlFormatter : IQueryFormatter
    {
        public string ContentType => "text/html";
        public string OutputFormat => "html";

        public ActionResult GetActionResult(QueryResult queryResult)
        {
            var generator = new DbQueryHtml(queryResult);
            return new ContentResult()
            {
                Content = generator.GetAsString(),
                ContentType = this.ContentType,
                StatusCode = (int)HttpStatusCode.OK
            };
        }
    }
}
