using ClosedXML.Excel;
using GenericDbRestApi.Lib.Formatters;
using GenericDbRestApi.Lib.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace GenericDbRestApi.Test.Formatters
{
    public class DbQueryExcelWorkBookTest
    {
        private const int FIRSTDATAROW_OFFSET = 5;

        [Fact]
        public void WhenSimpleQueryResultThenHeaderRowAndDescriptionRowIsCorrect()
        {
            var inputData = new QueryResultTestProvider().CreateBasicQueryResult();
            var sut = new DbQueryExcelWorkbook(inputData);
            var xlsxStream = sut.GetAsStream();
            var workBook = new XLWorkbook(xlsxStream);
            var workSheet = workBook.Worksheets.First();
            var headerCell = workSheet.Cell(1, 1);
            var descriptionCell = workSheet.Cell(2, 1);

            Assert.Equal(inputData.MetaData.Label, headerCell.GetString());
            Assert.Equal(inputData.MetaData.Description, descriptionCell.GetString());
        }

        [Fact]
        public void WhenSimpleQueryResultThenColumnHeadersAreCorrect()
        {
            var inputData = new QueryResultTestProvider().CreateBasicQueryResult();
            var sut = new DbQueryExcelWorkbook(inputData);
            var xlsxStream = sut.GetAsStream();
            var workBook = new XLWorkbook(xlsxStream);
            var workSheet = workBook.Worksheets.First();
            var excelColHeaderRow = workSheet.Row(4);
            
            for (int i = 0; i < inputData.MetaData.Columns.Count; i++)
            {
                Assert.Equal(inputData.MetaData.Columns[i].Label, excelColHeaderRow.Cell(i + 1).GetString());
            }
        }

        [Fact]
        public void WhenSimpleQueryResultThenDataInsertedCorrectly()
        {
            var inputData = new QueryResultTestProvider().CreateBasicQueryResult();
            var sut = new DbQueryExcelWorkbook(inputData);
            var xlsxStream = sut.GetAsStream();
            var workBook = new XLWorkbook(xlsxStream);
            var workSheet = workBook.Worksheets.First();

            //workBook.SaveAs(@"c:\temp\test.xlsx");
         
            var columns = inputData.MetaData.Columns;

            for (int row = 0; row < inputData.Data.Count; row++)
            {
                for (int col = 0; col < columns.Count; col++)
                {
                    switch (columns[col].ColumnType)
                    {
                        case QueryColumnType.DOUBLE:
                            Assert.Equal((double)inputData.Data[row][columns[col].Label], workSheet.Cell(row + FIRSTDATAROW_OFFSET, col + 1).GetValue<double>(), 4);
                            break;
                        case QueryColumnType.DATETIME:
                            Assert.Equal((DateTime)inputData.Data[row][columns[col].Label], workSheet.Cell(row + FIRSTDATAROW_OFFSET, col + 1).GetValue<DateTime>());
                            break;
                        case QueryColumnType.STRING:
                        case QueryColumnType.INT:
                        default:
                            Assert.Equal(inputData.Data[row][columns[col].Label].ToString(), workSheet.Cell(row + FIRSTDATAROW_OFFSET, col + 1).GetString());
                            break;
                    }
                }
            }
        }

        [Fact]
        public void WhenHierarchicalQueryResultThenHeaderRowAndDescriptionRowIsCorrect()
        {
            var inputData = new QueryResultTestProvider().CreateHierarchicalQueryResult();
            var sut = new DbQueryExcelWorkbook(inputData);
            var xlsxStream = sut.GetAsStream();
            var workBook = new XLWorkbook(xlsxStream);
            var workSheet = workBook.Worksheets.First();
            var headerCell = workSheet.Cell(1, 1);
            var descriptionCell = workSheet.Cell(2, 1);

            Assert.Equal(inputData.MetaData.Label, headerCell.GetString());
            Assert.Equal(inputData.MetaData.Description, descriptionCell.GetString());
        }

        [Fact]
        public void WhenHierarchicalQueryResultThenColumnHeadersAreCorrect()
        {
            var inputData = new QueryResultTestProvider().CreateHierarchicalQueryResult();
            var sut = new DbQueryExcelWorkbook(inputData);
            var xlsxStream = sut.GetAsStream();
            var workBook = new XLWorkbook(xlsxStream);
            var workSheet = workBook.Worksheets.First();
            var excelColHeaderRow = workSheet.Row(4);

            var excelHeaderCol = 0;

            for (int i = 0; i < inputData.MetaData.Columns.Count; i++)
            {
                Assert.Equal(inputData.MetaData.Columns[i].Label, excelColHeaderRow.Cell(++excelHeaderCol).GetString());
            }

            var child1Cols = inputData.MetaData.Children[0].Columns;
            for (int i = 0; i < child1Cols.Count; i++)
            {
                Assert.Equal(child1Cols[i].Label, excelColHeaderRow.Cell(++excelHeaderCol).GetString());
            }

            var grandChild1_1_Cols = inputData.MetaData.Children[0].Children[0].Columns;
            for (int i = 0; i < grandChild1_1_Cols.Count; i++)
            {
                Assert.Equal(grandChild1_1_Cols[i].Label, excelColHeaderRow.Cell(++excelHeaderCol).GetString());
            }

            var child2Cols = inputData.MetaData.Children[1].Columns;
            for (int i = 0; i < child2Cols.Count; i++)
            {
                Assert.Equal(child2Cols[i].Label, excelColHeaderRow.Cell(++excelHeaderCol).GetString());
            }
        }

        [Fact]
        public void WhenHierarchicalQueryResultThenTableDataRowCountIsCorrect()
        {
            var inputData = new QueryResultTestProvider().CreateHierarchicalQueryResult();
            var sut = new DbQueryExcelWorkbook(inputData);
            var xlsxStream = sut.GetAsStream();
            var workBook = new XLWorkbook(xlsxStream);
            var workSheet = workBook.Worksheets.First();

            var filledRows = QueryResultTestProvider.MAXROWS +
                             QueryResultTestProvider.MAXROWS * QueryResultTestProvider.CHILD1ROWS +
                             QueryResultTestProvider.MAXROWS * QueryResultTestProvider.CHILD2ROWS +
                             QueryResultTestProvider.MAXROWS * QueryResultTestProvider.CHILD1ROWS * QueryResultTestProvider.CHILD1ROWS;

            var maxFilledColumns = inputData.MetaData.Columns.Count +
                                   inputData.MetaData.Children[0].Columns.Count +
                                   inputData.MetaData.Children[1].Columns.Count +
                                   inputData.MetaData.Children[0].Children[0].Columns.Count;
            
            int row;

            for (row = FIRSTDATAROW_OFFSET; row < filledRows + FIRSTDATAROW_OFFSET; row++)
            {
                if (!RowHasContent(workSheet.Row(row), maxFilledColumns))
                    break;
            }
                
            Assert.Equal(filledRows + FIRSTDATAROW_OFFSET, row);
        }

        private bool RowHasContent(IXLRow row, int maxFilledColumns)
        {
            bool hasContent = false;
            for (int col = 1; col <= maxFilledColumns; col++)
            {
                hasContent = hasContent || !string.IsNullOrWhiteSpace(row.Cell(col).GetString());
            };
            return hasContent;
        }
    }
}
