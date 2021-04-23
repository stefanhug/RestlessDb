using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using System.Data.Common;
using GenericDbRestApi.DataLayer;
using GenericDbRestApi.Types;

namespace GenericDbRestApi.Utils
{
    public static class GenericSqlHelper
    {
        public static Dictionary<string, object> QuerySingleRow(string sqlStatement, SqlConnection dbConnection, Dictionary<string, string> parameters = null)
        {
            SqlCommand command = new SqlCommand(sqlStatement, dbConnection);
            AddParamsToCommand(command, parameters);

            using (SqlDataReader reader = command.ExecuteReader())
            {
                if (reader.Read())
                {
                    return GetReaderDataAsDict(reader);
                }
                else
                {
                    return null;
                }
            }
        }

        public static (List<Dictionary<string, object>> data, List<QueryColumn> columns, bool hasMoreRows)
            QueryAsDictList(string sqlStatement, SqlConnection dbConnection, int maxRows, Dictionary<string, string> parameters = null)
        {
            var data = new List<Dictionary<string, object>>();
            bool hasMoreRows = false;
            List<QueryColumn> columns;
            int numRowsRead = 0;

            SqlCommand command = new SqlCommand(sqlStatement, dbConnection);
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

                columns = GetColumnDescription(reader);
            }

            return (data, columns, hasMoreRows);
        }

        private static List<QueryColumn> GetColumnDescription(SqlDataReader reader)
        {
            var ret = new List<QueryColumn>();

            foreach(var col in reader.GetColumnSchema())
            {
                var qryCol = new QueryColumn();
                qryCol.Label = col.ColumnName;
                if (col.DataType == typeof(int))
                    qryCol.ColumnType = QueryColumnType.INT;
                else if (col.DataType == typeof(double))
                    qryCol.ColumnType = QueryColumnType.DOUBLE;
                else if (col.DataType == typeof(DateTime))
                    qryCol.ColumnType = QueryColumnType.DATETIME;
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

        private static void AddParamsToCommand(SqlCommand command, Dictionary<string, string> parameters)
        {
            if (parameters == null)
                return;

            foreach (var param in parameters)
            {
                command.Parameters.Add(new SqlParameter(param.Key, param.Value));
            }
        }

        private static object NormalizeValue(object value)
        {
            if (value is int || value is double || value is DateTime)
            {
                return value;
            }

            return value.ToString();
        }
    }
}
