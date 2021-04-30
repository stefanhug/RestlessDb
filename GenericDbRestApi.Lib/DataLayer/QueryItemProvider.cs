using GenericDbRestApi.Lib.Types;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GenericDbRestApi.Lib.DataLayer
{
    public class QueryItemProvider
    {
        const int MAXCHILDQUERIES = 100;

        const string QRY_QRY_REPOSITORY = @"
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

        public QueryItemProvider(IGenericSqlHelper genericSqlHelper)
        {
            this.GenericSqlHelper = genericSqlHelper;
        }

        public QueryItem LoadQueryItem(string queryName)
        {
            var (queryItemsForName, hasMoreRows) = GenericSqlHelper.QueryAsDictList(QRY_QRY_REPOSITORY,
                                                                  MAXCHILDQUERIES + 1,
                                                                  new Dictionary<string, object>() { { "NAME", queryName } });

            var topRows = from a in queryItemsForName where a["Name"].ToString().Equals(queryName, StringComparison.OrdinalIgnoreCase ) select a;

            if (topRows.Count() == 0)
            {
                throw new QueryItemLoaderException($"No query repository entry with name {queryName} found");
            }
            
            var ret = CreateItemFromRow(topRows.First(), queryItemsForName, new List<string>());
            return ret;
        }

        private void RecurseChildItems(QueryItem currentItem, List<Dictionary<string, object>> queryItemsForName, List<string> parentsList)
        {
            var childRows = from a in queryItemsForName where a["Parent"].ToString() == currentItem.Name orderby a["Pos"] select a;

            if (childRows.Any())
            {
                currentItem.ChildItems = new List<QueryItem>();

                foreach(var childRow in childRows)
                {
                    currentItem.ChildItems.Add(CreateItemFromRow(childRow, queryItemsForName, parentsList));
                }
            }
        }

        private QueryItem CreateItemFromRow(Dictionary<string, object> childRow, List<Dictionary<string, object>> queryItemsForName, List<string> parentsList)
        {
            var ret = new QueryItem()
            {
                Name = childRow["Name"] as string,
                Label = childRow["Label"] as string,
                Description = childRow["Description"] as string,
                Sql = childRow["Sql"] as string,
            };

            if (parentsList.Contains(ret.Name))
            {
                throw new QueryItemLoaderException($"Loop recursion detected for item {ret.Name}, please correct setup");
            }
            parentsList.Add(ret.Name);

            ret.Columns = GenericSqlHelper.QueryResultColumns(ret.Sql);
            RecurseChildItems(ret, queryItemsForName, new List<string>() { ret.Name });

            return ret;
        }

        protected IGenericSqlHelper GenericSqlHelper { get; }
    }
}


