using RazorLight;
using RestlessDb.Common.Types;
using System.Collections.Generic;

namespace RestlessDb.Formatters
{
    public class DbQueryHtml
    {
        private static readonly string DEFAULT_TEMPLATE_KEY = "DefaultRazorTemplate.cshtml";
        private readonly QueryResult queryResult;
        private readonly string templateString;
        private readonly Dictionary<string, int> metaStartColumnMap;
        private RazorLightEngine razorLightEngine;

        public DbQueryHtml(QueryResult queryResult, string templateString)
        {
            this.queryResult = queryResult;
            this.templateString = templateString;
            metaStartColumnMap = FormatterHelper.GetMetaDataStartColumnMap(queryResult.MetaData);
        }

        public string GetAsString()
        {
            string result = GetEngine()
                            .CompileRenderStringAsync<QueryResult>(DEFAULT_TEMPLATE_KEY, templateString, queryResult)
                            .GetAwaiter().GetResult();

            return result;
        }

        private RazorLightEngine GetEngine()
        {
            if (razorLightEngine == null)
            {
                razorLightEngine = new RazorLightEngineBuilder()
                                .UseEmbeddedResourcesProject(typeof(QueryResult))
                                .SetOperatingAssembly(this.GetType().Assembly)
                                .UseMemoryCachingProvider()
                                .Build();
            }
            return razorLightEngine;
        }
    }
}
 