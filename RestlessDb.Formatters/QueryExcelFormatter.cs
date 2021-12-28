using RestlessDb.Common.Types;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;

namespace RestlessDb.Formatters
{
    public class QueryExcelFormatter : IQueryFormatter
    {
        public string ContentType => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
        public string OutputFormat => "excel";
        public string Label => "Excel file";
        public string Description => "Data export in MS Excel format";
        public string FileExtension => "xlsx";
        public Disposition Disposition => Disposition.STANDALONE;

        public ActionResult GetActionResult(QueryResult queryResult)
        {
            var workbook = new DbQueryExcelWorkbook(queryResult);
            MemoryStream outputStream = workbook.GetAsStream();
            var content = outputStream.ToArray();
            FileResult res = new FileContentResult(content, ContentType);
            res.FileDownloadName = $"{workbook.QueryResult.MetaData.Name}_{DateTime.Now:yyyy-MM-dd}.xlsx";
            res.LastModified = DateTimeOffset.Now;
            return res;
        }
    }
}
