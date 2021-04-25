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
