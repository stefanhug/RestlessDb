using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace GenericDbRestApi.Lib.DataLayer
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
    }
}
