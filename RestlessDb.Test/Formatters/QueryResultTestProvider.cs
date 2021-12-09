using RestlessDb.Common.Types;
using System;
using System.Collections.Generic;
using System.Text;

namespace GenericDbRestApi.Test.Formatters
{
    public class QueryResultTestProvider
    {
        public const int MAXROWS = 1000;
        public const int CHILD1ROWS = 5;
        public const int CHILD2ROWS = 20;

        public QueryResult CreateBasicQueryResult()
        {
            var ret = new QueryResult();
            ret.MetaData = new QueryMetaData();
            ret.MetaData.Name = "TestName";
            ret.MetaData.Label = "Test Label";
            ret.MetaData.Description = "Test Description";
            ret.MetaData.Columns = new List<QueryColumn>()
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
                        { ret.MetaData.Columns[0].Label, $"String {i}" },
                        { ret.MetaData.Columns[1].Label, i * 0.1 },
                        { ret.MetaData.Columns[2].Label, i },
                        { ret.MetaData.Columns[3].Label, new DateTime(2020, 1, 1).AddDays(i) }
                    }
                );
            }

            return ret;
        }

        public QueryResult CreateQueryResultWithSpecialCharacters(char separatorChar, char escapeChar)
        {
            var ret = new QueryResult();
            ret.MetaData = new QueryMetaData();
            ret.MetaData.Name = "TestSpecialChars";
            ret.MetaData.Label = "Test Label Special Chars";
            ret.MetaData.Description = "Test Description";
            ret.MetaData.Columns = new List<QueryColumn>()
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
                    { ret.MetaData.Columns[0].Label, "Nothing to escape" }, { ret.MetaData.Columns[1].Label, 1 }
                 }
             );

            ret.Data.Add(
                 new Dictionary<string, object>()
                 {
                    { ret.MetaData.Columns[0].Label, "One,two,three" }, { ret.MetaData.Columns[1].Label, 2 }
                 }
             );

            ret.Data.Add(
                 new Dictionary<string, object>()
                 {
                                { ret.MetaData.Columns[0].Label, "Hi \"escaped text\"" }, { ret.MetaData.Columns[1].Label, 3 }
                 }
             );

            return ret;
        }

        public QueryResult CreateHierarchicalQueryResult()
        {
            var ret = new QueryResult();
            ret.MetaData = CreateHierarchicalMetadata("TestHierachical", "");
            var metaChild1 = CreateHierarchicalMetadata("TestHierachicalChild1", "Child1");
            var metaChild2 = CreateHierarchicalMetadata("TestHierachicalChild2", "Child2");
            var metaGrandChild1_1 = CreateHierarchicalMetadata("TestHierachicalGrandChild1_1", "GrandChild1_1");

            ret.MetaData.Children = new List<QueryMetaData>() { metaChild1, metaChild2 };
            ret.MetaData.Children[0].Children = new List<QueryMetaData>() { metaGrandChild1_1 };


            ret.Data = new List<Dictionary<string, object>>();

            for (int i = 0; i < MAXROWS; i++)
            {
                ret.Data.Add(
                    new Dictionary<string, object>()
                    {
                            { ret.MetaData.Columns[0].Label, $"String {i}" },
                            { ret.MetaData.Columns[1].Label, i * 0.1 },
                            { ret.MetaData.Columns[2].Label, i },
                            { ret.MetaData.Columns[3].Label, new DateTime(2020, 1, 1).AddDays(i) },
                            { metaChild1.Name, CreateChildData(CHILD1ROWS, metaChild1, "Child1", metaGrandChild1_1.Name, () => CreateChildData(CHILD1ROWS, metaGrandChild1_1, $"Child1_{i}")) },
                            { metaChild2.Name, CreateChildData(20, metaChild2, "Child2" ) },
                    }
                );
            }

            return ret;
        }

        private QueryMetaData CreateHierarchicalMetadata(string name, string prefix)
        {
            var meta = new QueryMetaData();
            meta.Name = name;
            meta.Label = $"{name} Label";
            meta.Description = $"{name} Description";
            meta.Columns = new List<QueryColumn>()
                {
                    new QueryColumn()
                    {
                        Label = $"{prefix}_Column 1",
                        ColumnType = QueryColumnType.STRING
                    },
                    new QueryColumn()
                    {
                        Label = $"{prefix}_Double Col",
                        ColumnType = QueryColumnType.DOUBLE
                    },
                    new QueryColumn()
                    {
                        Label = $"{prefix}_Integer",
                        ColumnType = QueryColumnType.INT
                    },
                    new QueryColumn()
                    {
                        Label = $"{prefix}_DateTime Col",
                        ColumnType = QueryColumnType.DATETIME
                    }
                };
            return meta;
        }

        private List<Dictionary<string, object>> CreateChildData(int childRows, QueryMetaData meta, string prefix, string childLabel = null, Func<List<Dictionary<string, object>>> childFunc = null)
        {
            var ret = new List<Dictionary<string, object>>();
            for (int i = 0; i < childRows; i++)
            {
                var row = new Dictionary<string, object>()
                    {
                            { meta.Columns[0].Label, $"{prefix}_String {i}" },
                            { meta.Columns[1].Label, i * 0.1 },
                            { meta.Columns[2].Label, i },
                            { meta.Columns[3].Label, new DateTime(2020, 1, 1).AddDays(i) }
                    };

                if (childFunc != null)
                {
                    row.Add(childLabel, childFunc());
                }
                ret.Add(row);
            }
            return ret;
        }
    }
}
