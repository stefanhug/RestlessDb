using ClosedXML.Excel;
using RestlessDb.Common.Types;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace RestlessDb.Formatters
{
    public class DbQueryExcelWorkbook : XLWorkbook
    {
        private readonly QueryResult queryResult;
        private readonly Dictionary<string, int> metaStartColumnMap;

        private readonly double DESCRIPTION_ROW_HEIGHT = 100;

        public QueryResult QueryResult 
        {
            get
            {
                return queryResult;
            }
        }

        public DbQueryExcelWorkbook(QueryResult queryResult)
        {
            this.queryResult = queryResult;
            metaStartColumnMap = FormatterHelper.GetMetaDataStartColumnMap(queryResult.MetaData);
        }

        public MemoryStream GetAsStream()
        {
            var memStream = new MemoryStream();
            var workSheet = Worksheets.Add(queryResult.MetaData.Label);
            
            var combinedColumns = FormatterHelper.CombineColHeaders(queryResult.MetaData);

            workSheet.Range(1, 1, 1, combinedColumns.Count).Merge();
            workSheet.Range(2, 1, 2, combinedColumns.Count).Merge();

            SetCellValueAndFormat(workSheet, 1, 1, value: queryResult.MetaData.Label, bold: true, 
                fontSize: 20, backgroundColor: XLColor.LightSteelBlue);
            var descriptionCell = SetCellValueAndFormat(workSheet, 2, 1, value: queryResult.MetaData.Description, italic: true, 
                fontSize: 16, backgroundColor: XLColor.LightGray);
            descriptionCell.Style.Alignment.SetWrapText(true);
            workSheet.Row(2).Height = DESCRIPTION_ROW_HEIGHT;
                        
            InsertHeader(workSheet, combinedColumns, 4, 1);
            InsertTable(workSheet, 5, queryResult.Data, queryResult.MetaData);

            AdjustToContents(workSheet, 4, 1);
            workSheet.SheetView.FreezeRows(4);

            SaveAs(memStream);
            return memStream;
        }

        private IXLCell SetCellValueAndFormat(IXLWorksheet worksheet, int row, int col, string value, 
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
            return cell;
        }

        private void AdjustToContents(IXLWorksheet worksheet, int row, int startCol)
        {
            for (int i = 0; i < queryResult.MetaData.Columns.Count(); i++)
            {
                worksheet.Column(startCol + i).AdjustToContents(row);
            }
        }

        private void InsertHeader(IXLWorksheet worksheet, List<QueryColumn> combinedColumns, int initRow, int initCol)
        {
            var currentColIndex = initCol;
            
            foreach (var col in combinedColumns)
            {
                SetCellValueAndFormat(worksheet, initRow, currentColIndex, value: col.Label, 
                    bold: true, backgroundColor: XLColor.LightSkyBlue);
                currentColIndex++;
            }
        }

        private int InsertTable(IXLWorksheet worksheet, int initRow, List<Dictionary<string, object>> tableData, QueryMetaData metaData)
        {
            var currentRowIndex = initRow;
            foreach (var row in tableData)
            {
                currentRowIndex = InsertDataRow(worksheet, currentRowIndex, row, metaData);
            }

            return currentRowIndex;
        }

        private int InsertDataRow(IXLWorksheet worksheet, int currentRowIndex, Dictionary<string, object> row, QueryMetaData metaData)
        {
            var startColIndex = metaStartColumnMap[metaData.Name] + 1;
            
            foreach (var col in metaData.Columns)
            {
                var cell = worksheet.Cell(currentRowIndex, startColIndex);
                var val = row[col.Label];
                switch (col.ColumnType)
                {
                    case QueryColumnType.DATETIME:
                        cell.SetValue<DateTime>((DateTime)val);
                        break;
                    case QueryColumnType.DOUBLE:
                        cell.SetValue<double>((double)val);
                        break;
                    case QueryColumnType.INT:
                        cell.SetValue<int>((int)val);
                        break;
                    case QueryColumnType.SHORT:
                        cell.SetValue<short>((short)val);
                        break;
                    case QueryColumnType.DECIMAL:
                        cell.SetValue<decimal>((decimal)val);
                        break;
                    case QueryColumnType.STRING:
                    default:
                        cell.SetValue<string>(val.ToString());
                        break;
                }
                startColIndex++;
            }
            currentRowIndex++;


            if (metaData.Children != null)
            {
                foreach (var metaChild in metaData.Children)
                {
                    currentRowIndex = InsertTable(worksheet, currentRowIndex, (List<Dictionary<string, object>>)row[metaChild.Name], metaChild);
                }
            }

            return currentRowIndex;
        }
    }
}
