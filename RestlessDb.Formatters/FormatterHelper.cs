using RestlessDb.Common.Types;
using System.Collections.Generic;

namespace RestlessDb.Formatters
{
    public static class FormatterHelper
    {
        public static List<QueryColumn> CombineColHeaders(QueryMetaData meta)
        {
            var ret = new List<QueryColumn>();

            RecurseCombineColHeaders(ret, meta);
            return ret;
        }

        public static Dictionary<string, int> GetMetaDataStartColumnMap(QueryMetaData meta)
        {
            var ret = new Dictionary<string, int>();

            RecurseMetaForStartColumn(ret, meta, 0);

            return ret;
        }

        private static int RecurseMetaForStartColumn(Dictionary<string, int> metaDataStartColumnMap, QueryMetaData meta, int nextAvailableColIndex)
        {
            metaDataStartColumnMap.Add(meta.Name, nextAvailableColIndex);
            nextAvailableColIndex += meta.Columns.Count;

            if (meta.Children != null)
            {
                foreach (var childMeta in meta.Children)
                {
                    nextAvailableColIndex = RecurseMetaForStartColumn(metaDataStartColumnMap, childMeta, nextAvailableColIndex);
                }
            }

            return nextAvailableColIndex;
        }

        public static void RecurseCombineColHeaders(List<QueryColumn> combinedColumns, QueryMetaData meta)
        {
            combinedColumns.AddRange(meta.Columns);
            if (meta.Children != null)
            {
                foreach (var childMeta in meta.Children)
                {
                    RecurseCombineColHeaders(combinedColumns, childMeta);
                }
            }
        }
    }
}
