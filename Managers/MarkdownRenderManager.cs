using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using MarkdownSharp;
using Microsoft.Extensions.Logging;

namespace GenericDBRestApi.Managers
{
    public class MarkdownRenderManager
    {
        private readonly ILogger<MarkdownRenderManager> logger;
        private readonly Markdown markdown;

        public MarkdownRenderManager(ILogger<MarkdownRenderManager> logger)
        {
            this.logger = logger;
            markdown = new Markdown();
        }

        public string GetMdFileAsHtml(string fileName)
        {
            string content = File.ReadAllText(Path.Combine(AppContext.BaseDirectory, fileName));
            return markdown.Transform(content);
        }
    }
}
