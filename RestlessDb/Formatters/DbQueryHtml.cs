﻿using RestlessDb.Types;
using RazorLight;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

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
                                .SetOperatingAssembly(typeof(QueryResult).Assembly)
                                .UseMemoryCachingProvider()
                                .Build();
            }
            return razorLightEngine;
        }
    }
}
 