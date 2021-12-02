using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using RestlessDb.DataLayer;
using RestlessDb.Types;
using System.Data;
using System.Data.Common;
using Microsoft.Extensions.Logging;

namespace RestlessDb.DataLayer
{
    public class GenericSqlHelper : IGenericSqlHelper
    {
        public static readonly string PARAM_PREFIX = "@";

        private static Dictionary<Type, QueryColumnType> TypeToColumnTypeDict = new Dictionary<Type, QueryColumnType>()
        {
            { typeof(int), QueryColumnType.INT },
            { typeof(short), QueryColumnType.SHORT },
            { typeof(double), QueryColumnType.DOUBLE },
            { typeof(DateTime), QueryColumnType.DATETIME },
            { typeof(decimal), QueryColumnType.DECIMAL },
            { typeof(string), QueryColumnType.STRING }
        };

        private SqlConnection SqlConnection { get; }
        private ILogger<GenericSqlHelper> Logger { get; }

        public GenericSqlHelper(SqlConnection dbConnection, ILogger<GenericSqlHelper> logger)
        {
            this.SqlConnection = dbConnection;
            this.Logger = logger;
        }

        public (List<Dictionary<string, object>> data, bool hasMoreRows)
            QueryAsDictList(string sqlStatement, int offset, int maxRows, Dictionary<string, object> parameters = null)
        {
            var data = new List<Dictionary<string, object>>();
            bool hasMoreRows = false;
            int numRowsRead = 0;
            int start = Environment.TickCount;

            try
            {
                string sqlWithRange = $"{sqlStatement} offset {offset} rows fetch next {maxRows + 1} rows only";

                Logger.LogInformation("QueryAsDictList SQL start");

                SqlCommand command = new SqlCommand(sqlStatement, SqlConnection);
                AddParamsToCommand(command, parameters);
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    Logger.LogInformation("QueryAsDictList ExecuteReader done ({0} ms)", Environment.TickCount - start);
                    while (reader.Read())
                    {
                        numRowsRead++;
                        if (numRowsRead <= maxRows)
                        {
                            data.Add(GetReaderDataAsDict(reader));
                            Logger.LogDebug("Read record to dictionary {0} ({1} ms)", numRowsRead, Environment.TickCount - start);
                        }
                        else
                        {
                            hasMoreRows = true;
                            // strange, the reader continues to read over the limit 'fetch next {maxRows + 1} rows only'
                            break;
                        }
                    }

                    command.Cancel(); //Otherwise we get a timeout in the dispose if more rows available
                }
                
                Logger.LogInformation("QueryAsDictList SQL: {0} ({1} ms)", sqlWithRange, Environment.TickCount - start);


                return (data, hasMoreRows);
            }
            catch(SqlException e)
            {
                var message = $"SqlException: '{e.Message}'\r\n" +
                              $"Errors: {e.Errors}\r\n" +
                              $"Sql statement: {sqlStatement}\r\n" +
                              $"Parameters: " + string.Join(", ", parameters) +
                              $"Stack trace: {e.StackTrace}";

                Logger.LogError(message);
                throw new GenericDbQueryException(GenericDbQueryExceptionCode.DBQUERY, message);
            }
        }

        public List<QueryColumn> QueryResultColumns(string sqlStatement)
        {
            List<QueryColumn> columns;

            SqlCommand command = new SqlCommand(sqlStatement, SqlConnection);

            // set default for params, otherwise the query even with SchemaOnly will fail
            var sqlParams = QueryParamsParser.GetQueryParams(sqlStatement);

            foreach (var sqlParam in sqlParams)
            {
                command.Parameters.Add(new SqlParameter() { ParameterName = PARAM_PREFIX + sqlParam, IsNullable = true });
            }

            using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.SchemaOnly))
            {
                columns = GetColumnDescription(reader);
            }

            return columns;
        }

        private static List<QueryColumn> GetColumnDescription(SqlDataReader reader)
        {
            var ret = new List<QueryColumn>();

            foreach (var col in reader.GetColumnSchema())
            {
                var qryCol = new QueryColumn();
                qryCol.Label = col.ColumnName;
                qryCol.ColumnType = GetColumnTypeOfType(col.DataType);

                ret.Add(qryCol);
            }
            return ret;
        }

        private static Dictionary<string, object> GetReaderDataAsDict(SqlDataReader reader)
        {
            var ret = new Dictionary<string, object>(reader.FieldCount, StringComparer.InvariantCultureIgnoreCase);
            for (int i = 0; i < reader.FieldCount; i++)
            {
                ret.Add(reader.GetName(i), NormalizeValue(reader.GetValue(i)));
            }
            
            return ret;
        }

        private static void AddParamsToCommand(SqlCommand command, Dictionary<string, object> parameters)
        {
            if (parameters == null)
                return;

            foreach (var param in parameters)
            {
                command.Parameters.Add(new SqlParameter(PARAM_PREFIX + param.Key, param.Value.ToString()));
            }
        }

        private static object NormalizeValue(object value)
        {
            return GetColumnTypeOfType(value.GetType()) == QueryColumnType.STRING ? value.ToString() : value;
        } 

        private static QueryColumnType GetColumnTypeOfType(Type type)
        {
            QueryColumnType val;
            if (TypeToColumnTypeDict.TryGetValue(type, out val))
                return val;
            return QueryColumnType.STRING;
        }
    }
}
