using GenericDbRestApi.Lib.Types;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GenericDbRestApi.Lib.DataLayer
{
    public class QueryItemProvider
    {
        private class DbQueryItem
        {
            public string Name { get; set; }
            public string Label { get; set; }
            public string Description { get; set; }
            public string Parent { get; set; }
            public string Sql { get; set; }
            public int Pos { get; set; }
        }

        public const int MAXCHILDQUERIES = 100;

        public const string QRY_QRY_REPOSITORY = @"
            WITH CTE (Name, Label, Description, Parent, Pos, Sql)
            AS (
	            SELECT Name, Label, Description, Parent, Pos, Sql
	            FROM GQuery.QueryItem
	            WHERE Name = @NAME
	            UNION ALL
	            SELECT child.Name, child.Label, child.Description, child.Parent, child.Pos, child.Sql
	            FROM GQuery.QueryItem child
	            INNER JOIN CTE CTE ON child.Parent = CTE.Name
	            WHERE child.parent IS NOT NULL
            )
            SELECT *
            FROM CTE
        ";

        protected IGenericSqlHelper GenericSqlHelper { get; }

        public QueryItemProvider(IGenericSqlHelper genericSqlHelper)
        {
            this.GenericSqlHelper = genericSqlHelper;
        }

        public QueryItem LoadQueryItem(string queryName)
        {
            var dbQueryItems = GetDbQueryItems(queryName);

            var topItem = dbQueryItems.Find(i => i.Name.Equals(queryName, StringComparison.InvariantCultureIgnoreCase));
            if (topItem == null)
            {
                throw new GenericDbQueryException(GenericDbQueryExceptionCode.QUERY_NOTFOUND, $"No query repository entry with name {queryName} found");
            }
            
            var ret = CreateItemFromRow(topItem, dbQueryItems, new List<string>());
            return ret;
        }

        private List<DbQueryItem> GetDbQueryItems(string queryName)
        {
            // possible to use dapper or EF, but typed results are only needed for this single query
            var (queryItemsForName, hasMoreRows) = GenericSqlHelper.QueryAsDictList(QRY_QRY_REPOSITORY,
                                                                  0, MAXCHILDQUERIES,
                                                                  new Dictionary<string, object>() { { "NAME", queryName } });

            string SafeGetAsToLower(object o)
            {
                var ret = o as string;
                return o == null ? null : ret.ToLowerInvariant();
            }

            var ret = 
                from a 
                in queryItemsForName 
                select new DbQueryItem()
                {
                    Name = ((string)a["Name"]).ToLowerInvariant(),
                    Label = (string)a["Label"],
                    Description = (string)a["Description"],
                    Parent = SafeGetAsToLower(a["Parent"]),
                    Pos = (int)a["Pos"],
                    Sql = (string)a["Sql"]
                };
            return ret.ToList();
        }


        private void RecurseChildItems(QueryItem currentItem, List<DbQueryItem> dbQueryItems, List<string> parentsList)
        {
            var childRows = from a in dbQueryItems where SafeCompare(a.Parent, currentItem.Name) orderby a.Pos select a;

            if (childRows.Any())
            {
                currentItem.ChildItems = new List<QueryItem>();

                foreach(var childRow in childRows)
                {
                    currentItem.ChildItems.Add(CreateItemFromRow(childRow, dbQueryItems, parentsList));
                }
            }
        }

        private QueryItem CreateItemFromRow(DbQueryItem dbItem, List<DbQueryItem> dbQueryItems, List<string> parentsList)
        {
            var ret = new QueryItem()
            {
                Name = dbItem.Name,
                Label = dbItem.Label,
                Description = dbItem.Description,
                Sql = dbItem.Sql,
            };

            if (!QueryParamsParser.ContainsOrderBy(ret.Sql))
            {
                throw new GenericDbQueryException(GenericDbQueryExceptionCode.SQL_MUST_HAVE_ORDER_BY,
                                                  $"Error in SQL \"{ret.Sql}\": All SQL statements in query item table must have an order by clause");
            }

            if (parentsList.Contains(ret.Name))
            {
                throw new GenericDbQueryException(GenericDbQueryExceptionCode.RECURSION, $"Loop recursion detected for item {ret.Name}, please correct setup");
            }
            parentsList.Add(ret.Name);

            ret.Columns = GenericSqlHelper.QueryResultColumns(ret.Sql);
            RecurseChildItems(ret, dbQueryItems, new List<string>() { ret.Name });

            return ret;
        }

        private bool SafeCompare(object a, object b)
        {
            if (a == null && b == null)
                return true;
            else if (a == null || b == null)
                return false;

            return a.ToString().Equals(b.ToString(), StringComparison.OrdinalIgnoreCase);
        }
    }
}


