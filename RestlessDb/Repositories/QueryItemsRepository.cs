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
        private const string SQL_GET_ALL = @"
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
            try
            {
                genericSqlHelper.QueryResultColumns(sql);
            }
            catch(SqlException e)
            {
                var message = $"SqlException: '{e.Message}'\r\nErrors: {e.Errors}\r\nSql statement: {sql}\r\nStack trace: {e.StackTrace}";
                logger.LogError(message);
                throw new GenericDbQueryException(GenericDbQueryExceptionCode.DBQUERY, message);
            }
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
                Parent = ((string)row["Parent"]).ToLowerInvariant(),
                Pos = (int)row["Pos"],
                Sql = (string)row["Sql"]
            };
        }
    }
}
