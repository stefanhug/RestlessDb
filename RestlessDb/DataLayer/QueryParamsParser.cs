using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Linq;

namespace RestlessDb.DataLayer
{
    public static class QueryParamsParser
    {
        public static HashSet<string> GetQueryParams(string sql)
        {
            var ret = new HashSet<string>();
            string pattern = @"\@[A-Za-z][A-Za-z0-9_]*";
            Regex rgx = new Regex(pattern);
            
            foreach (Match match in rgx.Matches(sql))
            {
                ret.Add(match.Value.Substring(1).ToUpperInvariant());
            }
            return ret;
        }

        public static bool ContainsOrderBy(string sql)
        {
            string pattern = @"\Worder\W+by\W";
            Regex rgx = new Regex(pattern, RegexOptions.IgnoreCase);

            return rgx.Matches(sql).Any();
        }

    }
}
