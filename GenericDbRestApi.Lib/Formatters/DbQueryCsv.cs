using GenericDbRestApi.Lib.Types;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace GenericDbRestApi.Lib.Formatters
{
    public class DbQueryCsv
    {
        private readonly QueryResult queryResult;
        private readonly Dictionary<string, int> metaStartColumnMap;

        public char SeparatorChar { get; set; } = ',';
        public char EscapeChar { get; set; } = '"';

        public DbQueryCsv(QueryResult queryResult)
        {
            this.queryResult = queryResult;
            metaStartColumnMap = FormatterHelper.GetMetaDataStartColumnMap(queryResult.MetaData);
        }

        public MemoryStream GetAsStream()
        {
            var memoryStream = new MemoryStream();
            var textWriter = new StreamWriter(memoryStream);
            WriteHeader(textWriter);
            WriteColumnHeader(textWriter);

            InsertTable(textWriter, queryResult.Data, queryResult.MetaData);
            textWriter.Flush();
            memoryStream.Position = 0;
            return memoryStream;
        }

        private void InsertTable(StreamWriter textWriter, List<Dictionary<string, object>> tableData, QueryMetaData metaData)
        {
            foreach (var row in tableData)
            {
                InsertDataRow(textWriter, row, metaData);
            }
        }

        private void InsertDataRow(StreamWriter textWriter, Dictionary<string, object> row, QueryMetaData metaData)
        {
            var startColIndex = metaStartColumnMap[metaData.Name];
            var rowList = new List<string>();

            for (int i = 0; i < startColIndex; i++)
                rowList.Add(string.Empty);

            foreach (var col in metaData.Columns)
            {
                var val = row[col.Label];
                rowList.Add(EscapeValue(val));
            }

            textWriter.WriteLine(string.Join(SeparatorChar, rowList));

            if (metaData.Children != null)
            {
                foreach (var metaChild in metaData.Children)
                {
                    InsertTable(textWriter, (List<Dictionary<string, object>>)row[metaChild.Name], metaChild);
                }
            }
        }

        private void WriteColumnHeader(StreamWriter textWriter)
        {
            var combinedColumns = FormatterHelper.CombineColHeaders(queryResult.MetaData);
            textWriter.WriteLine(string.Join(SeparatorChar, combinedColumns.Select(col => EscapeValue(col.Label))));
        }

        private string EscapeValue(object value)
        {
            var ret = value.ToString();
            bool escape = ret.IndexOf(SeparatorChar) >= 0 || ret.IndexOf(EscapeChar) >= 0;
            ret = ret.Replace(EscapeChar.ToString(), EscapeChar.ToString() + EscapeChar);

            return escape ? EscapeChar + ret + EscapeChar : ret;
        }

        private void WriteHeader(StreamWriter textWriter)
        {
            textWriter.WriteLine($"{queryResult.MetaData.Label} - {queryResult.MetaData.Description}");
        }
    }
}
