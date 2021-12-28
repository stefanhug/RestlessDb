using RestlessDb.Common.Types;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;

namespace RestlessDb.Formatters
{
    public class QueryCsvFormatter : IQueryFormatter
    {
        public string ContentType => "text/csv";
        public string OutputFormat => "csv";
        public string Label => "Csv file";
        public string Description => "Data export in comma separated value format";
        public string FileExtension => "csv";
        public Disposition Disposition => Disposition.STANDALONE;

        public ActionResult GetActionResult(QueryResult queryResult)
        {
            var workbook = new DbQueryCsv(queryResult);
            MemoryStream outputStream = workbook.GetAsStream();
            var content = outputStream.ToArray();
            FileResult res = new FileContentResult(content, ContentType);
            res.FileDownloadName = $"{queryResult.MetaData.Name}_{DateTime.Now:yyyy-MM-dd}.csv";
            res.LastModified = DateTimeOffset.Now;
            return res;
        }
    }
}