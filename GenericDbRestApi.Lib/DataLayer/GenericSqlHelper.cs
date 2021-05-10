using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using GenericDbRestApi.Lib.DataLayer;
using GenericDbRestApi.Lib.Types;
using System.Data;
using System.Data.Common;
using Microsoft.Extensions.Logging;

namespace GenericDbRestApi.Lib.DataLayer
{
    public class GenericSqlHelper : IGenericSqlHelper
    {
        public static readonly string PARAM_PREFIX = "@";
        private SqlConnection SqlConnection { get; }
        private ILogger<GenericSqlHelper> Logger { get; }

        public GenericSqlHelper(SqlConnection dbConnection, ILogger<GenericSqlHelper> logger)
        {
            this.SqlConnection = dbConnection;
            this.Logger = logger;
        }

        public (List<Dictionary<string, object>> data, bool hasMoreRows)
            QueryAsDictList(string sqlStatement, int maxRows, Dictionary<string, object> parameters = null)
        {
            var data = new List<Dictionary<string, object>>();
            bool hasMoreRows = false;
            int numRowsRead = 0;

            try
            {
                SqlCommand command = new SqlCommand(sqlStatement, SqlConnection);
                AddParamsToCommand(command, parameters);
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        numRowsRead++;
                        if (numRowsRead <= maxRows)
                            data.Add(GetReaderDataAsDict(reader));
                        else
                            hasMoreRows = true;
                    }
                }

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
                if (col.DataType == typeof(int))
                    qryCol.ColumnType = QueryColumnType.INT;
                else if (col.DataType == typeof(short))
                    qryCol.ColumnType = QueryColumnType.SHORT;
                else if (col.DataType == typeof(double))
                    qryCol.ColumnType = QueryColumnType.DOUBLE;
                else if (col.DataType == typeof(DateTime))
                    qryCol.ColumnType = QueryColumnType.DATETIME;
                else if (col.DataType == typeof(decimal))
                    qryCol.ColumnType = QueryColumnType.DECIMAL;
                else
                    qryCol.ColumnType = QueryColumnType.STRING;

                ret.Add(qryCol);
            }
            return ret;
        }

        private static Dictionary<string, object> GetReaderDataAsDict(SqlDataReader reader)
        {
            var ret = new Dictionary<string, object>((StringComparer.InvariantCultureIgnoreCase));
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
            if (value is int || value is double || value is DateTime || value is decimal || value is short)
            {
                return value;
            }

            return value.ToString();
        } 
    }
}
