using GenericDbRestApi.Types;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace GenericDBRestApi.Formatters
{
    public class QueryXmlFormatter : IQueryFormatter
    {
        public string ContentType => "application/xml ";
        public string OutputFormat => "xml";

        public ActionResult GetActionResult(GenericQueryResult queryResult)
        {
            MemoryStream outputStream = GetQueryResultAsXmlStream(queryResult);
            var content = outputStream.ToArray();
            FileResult res = new FileContentResult(content, ContentType);
            res.FileDownloadName = $"{queryResult.Name}_{DateTime.Now:yyyy-MM-dd}.xml";
            res.LastModified = DateTimeOffset.Now;
            return res;
        }

        private MemoryStream GetQueryResultAsXmlStream(GenericQueryResult queryResult)
        {
            var serializer = new DataContractSerializer(typeof(GenericQueryResult));
            MemoryStream memoryStream = new MemoryStream();
            serializer.WriteObject(memoryStream, queryResult);
            memoryStream.Position = 0;
            return memoryStream;
        }
    }
}
