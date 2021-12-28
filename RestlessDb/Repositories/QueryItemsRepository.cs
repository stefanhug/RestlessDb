using Microsoft.Extensions.Logging;
using RestlessDb.Common.Types;
using RestlessDb.DataLayer;
using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;

namespace RestlessDb.Repositories
{
    public class QueryItemsRepository
    {
        public const string SQL_GET_ALL = @"
            select Id, Name, Label, Description, Sql, Parent, Pos 
            from GQuery.QueryItem
            order by Name
        ";

        private const string SQL_GET = @"
            select Id, Name, Label, Description, Sql, Parent, Pos 
            from GQuery.QueryItem
            where Name = @Name
        ";

        private const string SQL_GET_CHILDREN = @"
            select Name 
            from GQuery.QueryItem
            where Parent = @Parent
        ";

        private const string SQL_INSERT = @"
            insert into GQuery.QueryItem(Name, Label, Description, Sql, Parent, Pos)
            values(@Name, @Label, @Description, @Sql, @Parent, @Pos)
        ";

        private const string SQL_UPDATE = @"
            update GQuery.QueryItem
            set Label = @Label,
                Description = @Description,
                Sql = @Sql,
                Parent = @Parent,
                Pos = @Pos
            where Name = @Name
        ";

        private const string SQL_DELETE= @"
            delete from GQuery.QueryItem
            where Name = @Name
        ";

        private readonly IGenericSqlHelper genericSqlHelper;
        private readonly ILogger<QueryItemsRepository> logger;
   
        public QueryItemsRepository(IGenericSqlHelper genericSqlHelper, ILogger<QueryItemsRepository> logger)
        {
            this.genericSqlHelper = genericSqlHelper ?? throw new Exception("No IGenericSqlHelper given");
            this.logger = logger ?? throw new Exception("No ILogger given"); 
        }

        public List<QueryItem> GetAll()
        {
            var (items, hasMoreRows) = genericSqlHelper.QueryAsDictList(SQL_GET_ALL);

            var ret =
                from item
                in items
                select CreateItemForDict(item);

            return ret.ToList();
        }

        public QueryItem Get(string name)
        {
            var (items, hasMoreRows) = genericSqlHelper.QueryAsDictList(SQL_GET,
                                                                    0, IGenericSqlHelper.MAX_QRY_ROWS,
                                                                    new () { { "NAME", name } });
            var ret =
                from item
                in items
                select CreateItemForDict(item);

            return ret.FirstOrDefault();
        }

        public List<string> GetChildren(string queryItemName)
        {
            var (items, hasMoreRows) = genericSqlHelper.QueryAsDictList(SQL_GET_CHILDREN,
                                                                    0, IGenericSqlHelper.MAX_QRY_ROWS,
                                                                    new() { { "PARENT", queryItemName } });
            var ret =
                from item
                in items
                select (string)item["Name"];

            return ret.ToList();
        }

        public int Insert(QueryItem queryItem)
        {
            return genericSqlHelper.ExecuteNonQuery(SQL_INSERT, 
                                            new()
                                            {
                                                { "NAME", queryItem.Name },
                                                { "LABEL", queryItem.Label },
                                                { "DESCRIPTION", queryItem.Description },
                                                { "SQL", queryItem.Sql },
                                                { "Parent", ConsolidateNull(queryItem.Parent) },
                                                { "POS", queryItem.Pos }
                                            });                                           
        }

        public int Update(QueryItem queryItem)
        {
            return genericSqlHelper.ExecuteNonQuery(SQL_UPDATE,
                                 new()
                                 {
                                     { "NAME", queryItem.Name },
                                     { "LABEL", queryItem.Label },
                                     { "DESCRIPTION", queryItem.Description },
                                     { "SQL", queryItem.Sql },
                                     { "Parent", ConsolidateNull(queryItem.Parent) },
                                     { "POS", queryItem.Pos }
                                 });
        }

        public int Delete(string name)
        {
            return genericSqlHelper.ExecuteNonQuery(SQL_DELETE, new() {{ "NAME", name }});
        }

        public void CheckValidSql(string sql)
        {
            // execute a schemaOnly query with filled params
            // TODO: move to QueryRepository, this is a test against a possibly different target DB
            try
            {
                genericSqlHelper.QueryResultColumns(sql);
            }
            catch(SqlException e)
            {
                var message = $"Check SQL statement {sql} failed:\r\n({e.Message})";
                logger.LogError(message);
                throw new GenericDbQueryException(GenericDbQueryExceptionCode.DBQUERY, message);
            }
        }

        public void CheckContainsOrderBy(string sql)
        {
            if (!QueryParamsParser.ContainsOrderBy(sql))
            {
                var message = $"Error: SQL statement: ''{sql}' does not contain the mandatory ORDER BY clause";
                logger.LogError(message);
                throw new GenericDbQueryException(GenericDbQueryExceptionCode.DBQUERY, message);
            }
        }

        public List<QueryItemExt> GetAllQueryItemsExt()
        {
            var ret = new List<QueryItemExt>();
            var queryItems = GetAll();

            var topItems = queryItems.FindAll(qryItem => string.IsNullOrWhiteSpace(qryItem.Parent));
            
            foreach (var topItem in topItems)
            {
                ret.Add(CreateItemFromRow(topItem, queryItems, new List<string>()));
            }
            
            return ret;
        }

        public QueryItemExt GetQueryItemExt(string queryName)
        {
            var queryItems = GetAll();

            var topItem = queryItems.Find(i => i.Name.Equals(queryName, StringComparison.InvariantCultureIgnoreCase));
            if (topItem == null)
            {
                throw new GenericDbQueryException(GenericDbQueryExceptionCode.QUERY_NOTFOUND, $"No query repository entry with name {queryName} found");
            }

            var ret = CreateItemFromRow(topItem, queryItems, new List<string>());
            return ret;
        }

        private void RecurseChildItems(QueryItemExt currentItem, List<QueryItem> queryItems, List<string> parentsList)
        {
            var childRows = from a in queryItems where SafeCompare(a.Parent, currentItem.QueryItem.Name) orderby a.Pos select a;

            if (childRows.Any())
            {
                currentItem.Children = new List<QueryItemExt>();

                foreach (var childRow in childRows)
                {
                    currentItem.Children.Add(CreateItemFromRow(childRow, queryItems, parentsList));
                }
            }
        }

        private QueryItemExt CreateItemFromRow(QueryItem queryItem, List<QueryItem> allQueryItems, List<string> parentsList)
        {
            var ret = new QueryItemExt()
            {
                QueryItem = queryItem
            };

            if (!QueryParamsParser.ContainsOrderBy(ret.QueryItem.Sql))
            {
                throw new GenericDbQueryException(GenericDbQueryExceptionCode.SQL_MUST_HAVE_ORDER_BY,
                                                  $"Error in SQL \"{ret.QueryItem.Sql}\": All SQL statements in query item table must have an order by clause");
            }

            if (parentsList.Contains(ret.QueryItem.Name))
            {
                throw new GenericDbQueryException(GenericDbQueryExceptionCode.RECURSION, $"Loop recursion detected for item {ret.QueryItem.Name}, please correct setup");
            }
            parentsList.Add(ret.QueryItem.Name);

            // todo: wrong DB, move to QueryRepository
            ret.Columns = genericSqlHelper.QueryResultColumns(ret.QueryItem.Sql);
            ret.Parameters = QueryParamsParser.GetQueryParams(ret.QueryItem.Sql);

            RecurseChildItems(ret, allQueryItems, new List<string>() { ret.QueryItem.Name });

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
    
        private string ConsolidateNull(string value)
        {
            return string.IsNullOrWhiteSpace(value) ? null : value;
        }

        private QueryItem CreateItemForDict(Dictionary<string, object> row)
        {
            return new QueryItem()
            {
                Id = (int)row["Id"],
                Name = ((string)row["Name"]).ToLowerInvariant(),
                Label = (string)row["Label"],
                Description = (string)row["Description"],
                Parent = ((string)row["Parent"])?.ToLowerInvariant(),
                Pos = (int)row["Pos"],
                Sql = (string)row["Sql"]
            };
        }
    }
}
