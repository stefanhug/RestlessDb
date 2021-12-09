using RestlessDb.Formatters;
using RestlessDb.Common.Types;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Net;
using System.Reflection;
using System.Text;

namespace RestlessDb.Formatters
{
    public class QueryHtmlFormatter : IQueryFormatter
    {
        private static readonly string DEFAULT_TEMPLATE_NAME = Path.Combine("templates", "DefaultRazorTemplate.cshtml");

        public string ContentType => "text/html";
        public string OutputFormat => "html";

        public ActionResult GetActionResult(QueryResult queryResult)
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

        private string GetDefaultTemplateAsString()
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
