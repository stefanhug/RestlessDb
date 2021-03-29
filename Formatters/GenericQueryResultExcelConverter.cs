using ClosedXML.Excel;
using GenericDbRestApi.Types;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;

namespace GenericDBRestApi.Formatters
{
    public class GenericQueryResultExcelConverter
    {
        public const string ExcelContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
        
        private readonly GenericQueryResult queryResult;

        public GenericQueryResultExcelConverter(GenericQueryResult queryResult)
        {
            this.queryResult = queryResult;
        }

        public MemoryStream GetAsExcelStream()
        {
            var memStream = new MemoryStream();
            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add(queryResult.Label);
                worksheet.Cell(1, 1).Value = queryResult.Label;
                worksheet.Cell(1, 1).Style.Font.Bold = true;
                worksheet.Cell(1, 1).Style.Font.FontSize = 20;

                worksheet.Cell(2, 1).Value = queryResult.Description;
                worksheet.Cell(2, 1).Style.Font.Italic = true;

                InsertHeader(worksheet, 4, 1);
                InsertTable(worksheet, 5, 1);

                workbook.SaveAs(memStream);
            }

            return memStream;
        }

        private void InsertHeader(IXLWorksheet worksheet, int initRow, int initCol)
        {
            var currentColIndex = initCol;
            foreach(var col in queryResult.Columns)
            {
                worksheet.Cell(initRow, currentColIndex).Value = col.Label;
                worksheet.Cell(initRow, currentColIndex).Style.Font.Bold = true;
                currentColIndex++;
            }
        }

        private void InsertTable(IXLWorksheet worksheet, int initRow, int initCol)
        {
            var currentRowIndex = initRow;
            foreach (var row in queryResult.Data)
            {
                InsertDataRow(worksheet, currentRowIndex, initCol, row);
                currentRowIndex++;
            }
        }

        private void InsertDataRow(IXLWorksheet worksheet, int currentRowIndex, int initCol, Dictionary<string, object> row)
        {
            var currentColIndex = initCol;
            foreach (var pair in row)
            {
                worksheet.Cell(currentRowIndex, currentColIndex).Value = pair.Value;
                currentColIndex++;
            }
        }

        public FileResult GetAsFileResult()
        {
            MemoryStream outputStream = GetAsExcelStream();
            var content = outputStream.ToArray();
            FileResult res = new FileContentResult(content, ExcelContentType);
            res.FileDownloadName = $"{queryResult.Name}_{DateTime.Now:yyyy-MM-dd}.xlsx";
            res.LastModified = DateTimeOffset.Now;
            return res;
        }
    }
}
