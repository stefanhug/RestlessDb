using RestlessDb.Formatters;
using RestlessDb.Common.Types;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Xunit;

namespace GenericDbRestApi.Test.Formatters
{
    [Trait("Category", "UnitTest")]
    public class DbQueryCsvTest
    {
        [Fact]
        public void WhenQueryResultWithColumnsIsGivenThenHeaderRowAndColHeaderRowIsInserted()
        {
            QueryResult inputData = new QueryResultTestProvider().CreateBasicQueryResult();
            var sut = new DbQueryCsv(inputData);
            var csvStream = sut.GetAsStream();
            var reader = new StreamReader(csvStream);
            
            var headerRow = reader.ReadLine();
            var colHeaderRow = reader.ReadLine();
            var expectedHeader = $"{inputData.MetaData.Label} - {inputData.MetaData.Description}";
            var expectedHeaderRow = string.Join(sut.SeparatorChar, inputData.MetaData.Columns.Select(c => c.Label));
            
            Assert.Equal(headerRow, expectedHeader);
            Assert.Equal(colHeaderRow, expectedHeaderRow);
        }

        [Fact]
        public void WhenQueryResultWithColumnsIsGivenThenCorrectNumberOfRowsIsCreated()
        {
            QueryResult inputData = new QueryResultTestProvider().CreateBasicQueryResult();
            var sut = new DbQueryCsv(inputData);
            var csvStream = sut.GetAsStream();
            var reader = new StreamReader(csvStream);
            
            // Read Header and colheader row
            SkipLines(reader, 2);
            int dataRowsRead = 0;
            while (reader.ReadLine() != null)
                dataRowsRead++;

            Assert.Equal(dataRowsRead, QueryResultTestProvider.MAXROWS);
        }

        [Fact]
        public void WhenQueryResultWithColumnsWithNoValueContainingSeparatorCharIsGivenThenNothingEscaped()
        {
            QueryResult inputData = new QueryResultTestProvider().CreateQueryResultWithSpecialCharacters(',', '\"');
            var sut = new DbQueryCsv(inputData);
            var csvStream = sut.GetAsStream();
            var reader = new StreamReader(csvStream);
            
            SkipLines(reader, 2);
            var dataRow = reader.ReadLine();
            Assert.NotNull(dataRow);
            Assert.Equal("Nothing to escape,1", dataRow);
        }

        [Fact]
        public void WhenQueryResultWithColumnsWithValueContainingSeparatorCharIsGivenThenEscaped()
        {
            QueryResult inputData = new QueryResultTestProvider().CreateQueryResultWithSpecialCharacters(',', '\"');
            var sut = new DbQueryCsv(inputData);
            var csvStream = sut.GetAsStream();
            var reader = new StreamReader(csvStream);
            
            SkipLines(reader, 3);
            var dataRow = reader.ReadLine();
            Assert.NotNull(dataRow);
            Assert.Equal("\"One,two,three\",2", dataRow);
        }

        [Fact]
        public void WhenQueryResultWithColumnsWithValueContainingEscapeCharIsGivenThenEscaped()
        {
            QueryResult inputData = new QueryResultTestProvider().CreateQueryResultWithSpecialCharacters(',', '\"');
            var sut = new DbQueryCsv(inputData);
            var csvStream = sut.GetAsStream();
            var reader = new StreamReader(csvStream);
            
            SkipLines(reader, 4);
            var dataRow = reader.ReadLine();
            Assert.NotNull(dataRow);
            Assert.Equal("\"Hi \"\"escaped text\"\"\",3", dataRow);
        }

        [Fact]
        public void WhenQueryResultWithNullValuesIsGivenThenCorrectCsvIsCreated()
        {
            QueryResult inputData = new QueryResultTestProvider().CreateBasicQueryResultWithNullValues();
            var sut = new DbQueryCsv(inputData);
            var csvStream = sut.GetAsStream();
            var reader = new StreamReader(csvStream);

            var headerRow = reader.ReadLine();
            var colHeaderRow = reader.ReadLine();
            SkipLines(reader, inputData.Data.Count -1);
            var lastLine = reader.ReadLine();
            Assert.NotNull(lastLine);
            var beyondLast = reader.ReadLine();
            Assert.Null(beyondLast);
        }

        [Fact]
        public void WhenHierarchicalQueryResultThenHeaderAndDescriptionInFirstLineIsCorrect()
        {
            var inputData = new QueryResultTestProvider().CreateHierarchicalQueryResult();
            var sut = new DbQueryCsv(inputData);
            var csvStream = sut.GetAsStream();
            var reader = new StreamReader(csvStream);

            var headerRow = reader.ReadLine();
            var expectedHeader = $"{inputData.MetaData.Label} - {inputData.MetaData.Description}";
            
            Assert.Equal(headerRow, expectedHeader);
        }

        [Fact]
        public void WhenHierarchicalQueryResultThenColumnHeadersAreCorrect()
        {
            var inputData = new QueryResultTestProvider().CreateHierarchicalQueryResult();
            var sut = new DbQueryCsv(inputData);
            var csvStream = sut.GetAsStream();
            var reader = new StreamReader(csvStream);

            SkipLines(reader, 1);
            var colHeaderRow = reader.ReadLine();
            
            // in general this apporoach for getting the column headers is too primitive, als column headers could contain separator char too
            // This is not the case for the given test data
            List<string> columnHeaders = new List<string>(colHeaderRow.Split(sut.SeparatorChar));
            var headerCol = 0;

            for (int i = 0; i < inputData.MetaData.Columns.Count; i++)
            {
                Assert.Equal(inputData.MetaData.Columns[i].Label, columnHeaders[headerCol++]);
            }

            var child1Cols = inputData.MetaData.Children[0].Columns;
            for (int i = 0; i < child1Cols.Count; i++)
            {
                Assert.Equal(child1Cols[i].Label, columnHeaders[headerCol++]);
            }

            var grandChild1_1_Cols = inputData.MetaData.Children[0].Children[0].Columns;
            for (int i = 0; i < grandChild1_1_Cols.Count; i++)
            {
                Assert.Equal(grandChild1_1_Cols[i].Label, columnHeaders[headerCol++]);
            }

            var child2Cols = inputData.MetaData.Children[1].Columns;
            for (int i = 0; i < child2Cols.Count; i++)
            {
                Assert.Equal(child2Cols[i].Label, columnHeaders[headerCol++]);
            }
        }

        [Fact]
        public void WhenHierarchicalQueryResultThenTableDataRowCountIsCorrect()
        {
            var inputData = new QueryResultTestProvider().CreateHierarchicalQueryResult();
            var sut = new DbQueryCsv(inputData);
            var csvStream = sut.GetAsStream();
            var reader = new StreamReader(csvStream);

            var filledRows = QueryResultTestProvider.MAXROWS +
                             QueryResultTestProvider.MAXROWS * QueryResultTestProvider.CHILD1ROWS +
                             QueryResultTestProvider.MAXROWS * QueryResultTestProvider.CHILD2ROWS +
                             QueryResultTestProvider.MAXROWS * QueryResultTestProvider.CHILD1ROWS * QueryResultTestProvider.CHILD1ROWS;

            // Read Header and colheader row
            SkipLines(reader, 2);
            int dataRowsRead = 0;
            while (reader.ReadLine() != null)
                dataRowsRead++;

            Assert.Equal(filledRows, dataRowsRead);
        }

        private void SkipLines(StreamReader reader, int numLines)
        {
            for (int i = 0; i < numLines; i++)
                reader.ReadLine();
        }

    }
}
