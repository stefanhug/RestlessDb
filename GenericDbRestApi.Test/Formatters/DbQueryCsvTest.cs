using GenericDbRestApi.Types;
using GenericDBRestApi.Formatters;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using Xunit;
using System;

namespace GenericDBRestApi.Test
{
    public class DbQueryCsvTest
    {
        const int MAXROWS = 1000;

        [Fact]
        public void WhenQueryResultWithColumnsIsGivenThenHeaderRowAndColHeaderRowIsInserted()
        {
            GenericQueryResult inputData = CreateBasicQueryResult();
            var sut = new DbQueryCsv(inputData);
            var csvStream = sut.GetAsStream();
            var reader = new StreamReader(csvStream);
            var headerRow = reader.ReadLine();
            var colHeaderRow = reader.ReadLine();
            var expectedHeader = $"{inputData.Label} - {inputData.Description}";
            var expectedHeaderRow = string.Join(sut.SeparatorChar, inputData.Columns.Select(c => c.Label));
            Assert.Equal(headerRow, expectedHeader);
            Assert.Equal(colHeaderRow, expectedHeaderRow);
        }

        [Fact]
        public void WhenQueryResultWithColumnsIsGivenThenCorrectNumberOfRowsIsCreated()
        {
            GenericQueryResult inputData = CreateBasicQueryResult();
            var sut = new DbQueryCsv(inputData);
            var csvStream = sut.GetAsStream();
            var reader = new StreamReader(csvStream);
            // Read Header and colheader row
            SkipLines(reader, 2);
            int dataRowsRead = 0;
            while (reader.ReadLine() != null)
                dataRowsRead++;

            Assert.Equal(dataRowsRead, MAXROWS);
        }

        [Fact]
        public void WhenQueryResultWithColumnsWithNoValueContainingSeparatorCharIsGivenThenNothingEscaped()
        {
            GenericQueryResult inputData = CreateSpecialQueryResult(',', '\"');
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
            GenericQueryResult inputData = CreateSpecialQueryResult(',', '\"');
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
            GenericQueryResult inputData = CreateSpecialQueryResult(',', '\"');
            var sut = new DbQueryCsv(inputData);
            var csvStream = sut.GetAsStream();
            var reader = new StreamReader(csvStream);
            SkipLines(reader, 4);
            var dataRow = reader.ReadLine();
            Assert.NotNull(dataRow);
            Assert.Equal("\"Hi \"\"escaped text\"\"\",3", dataRow);
        }

        private void SkipLines(StreamReader reader, int numLines)
        {
            for (int i = 0; i < numLines; i++)
                reader.ReadLine();
        }

        private GenericQueryResult CreateSpecialQueryResult(char separatorChar, char escapeChar)
        {
            var ret = new GenericQueryResult();
            ret.Name = "TestSpecialChars";
            ret.Label = "Test Label Special Chars";
            ret.Description = "Test Description";
            ret.Columns = new List<QueryColumn>()
            {
                new QueryColumn()
                {
                    Label = "Column 1",
                    ColumnType = QueryColumnType.STRING
                },
                new QueryColumn()
                {
                    Label = "Column 2",
                    ColumnType = QueryColumnType.DOUBLE
                }
            };

            ret.Data = new List<Dictionary<string, object>>();
            ret.Data.Add(
                 new Dictionary<string, object>()
                 {
                                { ret.Columns[0].Label, "Nothing to escape" }, { ret.Columns[1].Label, 1 }
                 }
             );

            ret.Data.Add(
                 new Dictionary<string, object>()
                 {
                                { ret.Columns[0].Label, "One,two,three" }, { ret.Columns[1].Label, 2 }
                 }
             );

            ret.Data.Add(
                 new Dictionary<string, object>()
                 {
                                { ret.Columns[0].Label, "Hi \"escaped text\"" }, { ret.Columns[1].Label, 3 }
                 }
             );

            return ret;
        }

        private GenericQueryResult CreateBasicQueryResult()
        {
            var ret = new GenericQueryResult();
            ret.Name = "TestName";
            ret.Label = "Test Label";
            ret.Description = "Test Description";
            ret.Columns = new List<QueryColumn>()
            {
                new QueryColumn()
                {
                    Label = "Column 1",
                    ColumnType = QueryColumnType.STRING
                },
                new QueryColumn()
                {
                    Label = "Double Col",
                    ColumnType = QueryColumnType.DOUBLE
                },
                new QueryColumn()
                {
                    Label = "Integer",
                    ColumnType = QueryColumnType.INT
                },
                new QueryColumn()
                {
                    Label = "DateTime Col",
                    ColumnType = QueryColumnType.DATETIME
                }

            };

            ret.Data = new List<Dictionary<string, object>>();

            for (int i = 0; i < MAXROWS; i++)
            {
                ret.Data.Add(
                    new Dictionary<string, object>()
                    {
                        { ret.Columns[0].Label, $"String {i}" },
                        { ret.Columns[1].Label, i * 0.1 },
                        { ret.Columns[2].Label, i },
                        { ret.Columns[3].Label, new DateTime(2020, 1, 1).AddDays(i) }
                    }
                );
            }

            return ret;
        }
    }
}
