using ClosedXML.Excel;
using GenericDbRestApi.Types;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace GenericDBRestApi.Formatters
{
    public class DbQueryExcelWorkbook : XLWorkbook
    {
        private readonly GenericQueryResult queryResult;
        public GenericQueryResult QueryResult 
        {
            get
            {
                return queryResult;
            }
        }

        public DbQueryExcelWorkbook(GenericQueryResult queryResult)
        {
            this.queryResult = queryResult;
        }

        public MemoryStream GetAsStream()
        {
            var memStream = new MemoryStream();
            var worksheet = Worksheets.Add(queryResult.Label);
            var lastCol = queryResult.Columns.Count();
            worksheet.Range(1, 1, 1, lastCol).Merge();
            worksheet.Range(2, 1, 2, lastCol).Merge();

            SetCellValueAndFormat(worksheet, 1, 1, value: queryResult.Label, bold: true, 
                fontSize: 20, backgroundColor: XLColor.LightSteelBlue);
            SetCellValueAndFormat(worksheet, 2, 1, value: queryResult.Description, italic: true, 
                fontSize: 16, backgroundColor: XLColor.LightGray);
                        
            InsertHeader(worksheet, 4, 1);
            InsertTable(worksheet, 5, 1);

            AdjustToContents(worksheet, 4, 1);
            worksheet.SheetView.FreezeRows(4);

            SaveAs(memStream);
            return memStream;
        }

        private void SetCellValueAndFormat(IXLWorksheet worksheet, int row, int col, string value, 
            bool bold=false, bool italic=false, int? fontSize=null, XLColor backgroundColor = null)
        {
            var cell = worksheet.Cell(row, col);
            cell.Value = value;
            cell.Style.Font.Bold = bold;
            cell.Style.Font.Italic = italic;
            if (fontSize.HasValue)
                cell.Style.Font.FontSize = fontSize.Value;
            if (backgroundColor != null)
                cell.Style.Fill.BackgroundColor = backgroundColor;
        }

        private void AdjustToContents(IXLWorksheet worksheet, int row, int startCol)
        {
            for (int i = 0; i < queryResult.Columns.Count(); i++)
            {
                worksheet.Column(startCol + i).AdjustToContents(row);
            }
        }

        private void InsertHeader(IXLWorksheet worksheet, int initRow, int initCol)
        {
            var currentColIndex = initCol;
            foreach (var col in queryResult.Columns)
            {
                SetCellValueAndFormat(worksheet, initRow, currentColIndex, value: col.Label, 
                    bold: true, backgroundColor: XLColor.LightSkyBlue);
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
    }
}
