using RestlessDb.Common.Types;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace RestlessDb.Formatters
{
    public class QueryXmlFormatter : IQueryFormatter
    {
        public string ContentType => "application/xml ";
        public string OutputFormat => "xml";
        public string Label => "Xml file";
        public string Description => "Data export as XML file";
        public string FileExtension => "xml";
        public Disposition Disposition => Disposition.STANDALONE;

        public ActionResult GetActionResult(QueryResult queryResult)
        {
            MemoryStream outputStream = GetQueryResultAsXmlStream(queryResult);
            var content = outputStream.ToArray();
            FileResult res = new FileContentResult(content, ContentType);
            res.FileDownloadName = $"{queryResult.MetaData.Name}_{DateTime.Now:yyyy-MM-dd}.xml";
            res.LastModified = DateTimeOffset.Now;
            return res;
        }

        private MemoryStream GetQueryResultAsXmlStream(QueryResult queryResult)
        {
            var serializer = new DataContractSerializer(typeof(QueryResult));
            MemoryStream memoryStream = new MemoryStream();
            serializer.WriteObject(memoryStream, queryResult);
            memoryStream.Position = 0;
            return memoryStream;
        }
    }
}
