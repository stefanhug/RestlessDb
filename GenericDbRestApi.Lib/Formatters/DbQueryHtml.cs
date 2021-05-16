using GenericDbRestApi.Lib.Types;
using RazorLight;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace GenericDbRestApi.Lib.Formatters
{
    public class DbQueryHtml
    {
        private static readonly string DEFAULT_TEMPLATE_NAME = Path.Combine("templates", "DefaultRazorTemplate.cshtml");
        private readonly QueryResult queryResult;
        private readonly Dictionary<string, int> metaStartColumnMap;
        private RazorLightEngine razorLightEngine;

        public DbQueryHtml(QueryResult queryResult)
        {
            this.queryResult = queryResult;
            metaStartColumnMap = FormatterHelper.GetMetaDataStartColumnMap(queryResult.MetaData);
        }

        public string GetAsString()
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

            string result = GetEngine()
                            .CompileRenderStringAsync<QueryResult>(DEFAULT_TEMPLATE_NAME, template, queryResult)
                            .GetAwaiter().GetResult();

            return result;
        }


        private RazorLightEngine GetEngine()
        {
            if (razorLightEngine == null)
            {
                razorLightEngine = new RazorLightEngineBuilder()
                                .UseEmbeddedResourcesProject(typeof(QueryResult))
                                .SetOperatingAssembly(typeof(QueryResult).Assembly)
                                .UseMemoryCachingProvider()
                                .Build();
            }
            return razorLightEngine;
        }
    }
}
