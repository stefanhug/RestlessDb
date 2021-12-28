using Microsoft.AspNetCore.Mvc;
using RestlessDb.Common.Types;
using System.Net;

namespace RestlessDb.Formatters
{
    public class QueryHtmlEmbeddedFormatter : QueryHtmlFormatter
    {
        public override Disposition Disposition => Disposition.EMBEDDED;
        public override string OutputFormat => "htmlembedded";
        public override string Label => "Html embedded";

        public override ActionResult GetActionResult(QueryResult queryResult)
        {
            // TODO: use other template without html head and body scaffolding
            var templateString = GetDefaultTemplateAsString();
            var generator = new DbQueryHtml(queryResult, templateString);
            return new ContentResult()
            {
                Content = generator.GetAsString(),
                ContentType = this.ContentType,
                StatusCode = (int)HttpStatusCode.OK
               
            };
        }
        
    }
}
