using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Data.Common;
using Microsoft.Data.SqlClient;
using GenericDbRestApi.Utils;
using System;
using GenericDbRestApi.Types;

namespace GenericDbRestApi.DataLayer
{
    public class GenericQueryRepository
    {
        const string QRY_QRY_REPOSITORY = "select * from GQUERY.QUERYREPOSITORY where name=@NAME";
        public GenericQueryRepository(MyDbContext dbContext, ILogger<GenericQueryRepository> logger) 
        {
            this.dbContext = dbContext;
            this.logger = logger;
        }

        public GenericQueryResult GetQueryResults(string queryName, int offset, int maxRows, 
            Dictionary<string, string> queryParameters)
        {
            var ret = new GenericQueryResult();
            var queryRepositoryRow = GenericSqlHelper.QuerySingleRow(QRY_QRY_REPOSITORY, 
                                                              DbConnection, 
                                                              new Dictionary<string, string>() { { "@NAME", queryName } });
            if (queryRepositoryRow == null)
            {
                ret.Status = GenericQueryResultStatus.QRY_NOTFOUND;
                ret.ErrorMessage = $"No query repository entry with name {queryName} found";
            }
            else
            {
                ret.Name = queryRepositoryRow["Name"] as string;
                ret.Label = queryRepositoryRow["Label"] as string;
                ret.Description = queryRepositoryRow["Description"] as string;
                ret.Offset = offset;
                ret.MaxRows = maxRows;

                string sqlStmt = (string)queryRepositoryRow["SQL"];
                string sql = $"{sqlStmt} offset {offset} rows fetch next {maxRows} rows only";

                logger.LogInformation("GetQueryResults SQL: {0}", sql);

                try
                {
                    (ret.Data, ret.Columns) = GenericSqlHelper.QueryAsDictList(sql, DbConnection, queryParameters);
                }
                catch (Exception e)
                {
                    ret.ErrorMessage = $"Error retrieving data: {e.Message}";
                    ret.Status = GenericQueryResultStatus.SERVER_ERROR;
                }
                
            }


            return ret;
        }

        private SqlConnection dbConnection;

        private SqlConnection DbConnection { 
            get
            {
                if(dbConnection == null)
                {
                    dbConnection = dbContext.Database.GetDbConnection() as SqlConnection;
                    dbConnection.Open();
                }
                return dbConnection;
            } 
        }

        private readonly MyDbContext dbContext;
        private readonly ILogger<GenericQueryRepository> logger;
    }

}
