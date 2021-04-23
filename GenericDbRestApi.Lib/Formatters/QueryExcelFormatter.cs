using ClosedXML.Excel;
using GenericDbRestApi.Types;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;

namespace GenericDBRestApi.Formatters
{
    public class QueryExcelFormatter : IQueryFormatter
    {
        public string ContentType => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
        public string OutputFormat => "excel";

        public ActionResult GetActionResult(GenericQueryResult queryResult)
        {
            var workbook = new DbQueryExcelWorkbook(queryResult);
            MemoryStream outputStream = workbook.GetAsStream();
            var content = outputStream.ToArray();
            FileResult res = new FileContentResult(content, ContentType);
            res.FileDownloadName = $"{workbook.QueryResult.Name}_{DateTime.Now:yyyy-MM-dd}.xlsx";
            res.LastModified = DateTimeOffset.Now;
            return res;
        }
    }
}
