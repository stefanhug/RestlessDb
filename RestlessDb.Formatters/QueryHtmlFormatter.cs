using Microsoft.AspNetCore.Mvc;
using RestlessDb.Common.Types;
using System.Net;
using System.Reflection;
using System.Text;

namespace RestlessDb.Formatters
{
    public class QueryHtmlFormatter : IQueryFormatter
    {
        protected static readonly string DEFAULT_TEMPLATE_NAME = Path.Combine("templates", "DefaultRazorTemplate.cshtml");

        public virtual string ContentType => "text/html";
        public virtual string OutputFormat => "html";
        public virtual string Label => "Html file";
        public virtual string Description => "Data export as HTML file";
        public virtual string FileExtension => "html";
        public virtual Disposition Disposition => Disposition.STANDALONE;

        public virtual ActionResult GetActionResult(QueryResult queryResult)
        {
            var templateString = GetDefaultTemplateAsString();
            var generator = new DbQueryHtml(queryResult, templateString);
            return new ContentResult()
            {
                Content = generator.GetAsString(),
                ContentType = this.ContentType,
                StatusCode = (int)HttpStatusCode.OK
            };
        }

        protected string GetDefaultTemplateAsString()
        {
            string executionDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string templatePath = Path.Combine(executionDirectory, DEFAULT_TEMPLATE_NAME);
            if (!File.Exists(templatePath))
                throw new GenericDbQueryException(GenericDbQueryExceptionCode.TEMPLATE_ERROR,
                                                  $"Default template {templatePath} not found");

            string template = File.ReadAllText(templatePath, Encoding.UTF8);
            if (string.IsNullOrEmpty(template))
                throw new GenericDbQueryException(GenericDbQueryExceptionCode.TEMPLATE_ERROR,
                                                  $"Default template {templatePath} is empty");
            return template;
        }
    }
}
