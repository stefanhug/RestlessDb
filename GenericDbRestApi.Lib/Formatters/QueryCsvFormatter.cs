using GenericDbRestApi.Lib.Formatters;
using GenericDbRestApi.Lib.Types;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;

namespace GenericDBRestApi.Lib.Formatters
{
    public class QueryCsvFormatter : IQueryFormatter
    {
        public string ContentType => "text/csv";
        public string OutputFormat => "csv";

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