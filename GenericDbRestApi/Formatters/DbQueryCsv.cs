using GenericDbRestApi.Types;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace GenericDBRestApi.Formatters
{
    public class DbQueryCsv
    {
        private readonly GenericQueryResult queryResult;

        public char SeparatorChar { get; set; } = ',';
        public char EscapeChar { get; set; } = '"';

        public DbQueryCsv(GenericQueryResult queryResult)
        {
            this.queryResult = queryResult;
        }

        public MemoryStream GetAsStream()
        {
            var memoryStream = new MemoryStream();
            var textWriter = new StreamWriter(memoryStream);
            WriteHeader(textWriter);
            WriteColumnHeader(textWriter);
            WriteDataRows(textWriter);
            textWriter.Flush();
            memoryStream.Position = 0;
            return memoryStream;
        }

        private void WriteDataRows(StreamWriter textWriter)
        {
            foreach (var row in queryResult.Data)
            {
                textWriter.WriteLine(string.Join(SeparatorChar, row.Select(col => EscapeValue(col.Value))));
            }
        }

        private void WriteColumnHeader(StreamWriter textWriter)
        {
            textWriter.WriteLine(string.Join(SeparatorChar, queryResult.Columns.Select(col => EscapeValue(col.Label))));
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
            textWriter.WriteLine($"{queryResult.Label} - {queryResult.Description}");
        }
    }
}
