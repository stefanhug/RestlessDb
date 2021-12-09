using RestlessDb.Common.Types;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace RestlessDb.DataLayer
{
    public class QueryParamsProvider
    {
        private readonly ILogger<QueryParamsProvider> logger;

        public QueryParamsProvider(ILogger<QueryParamsProvider> logger)
        {
            this.logger = logger;
        }

        public Dictionary<string, object> CollectParamsForQuery(string sqlStmt, Dictionary<string, object> commandParams,
            List<Dictionary<string, object>> parentRows)     
        {
            var resultParams = new Dictionary<string, object>(commandParams);
            var neededParams = QueryParamsParser.GetQueryParams(sqlStmt);
            var missingParams = new HashSet<string>(neededParams);
            missingParams.ExceptWith(commandParams.Keys);

            logger.LogInformation($"Needed parameters for query {sqlStmt}: {string.Join(", ", neededParams)}");
            
            if (missingParams.Count > 0)
            {
                logger.LogInformation($"Params not found in command params: {string.Join(", ", missingParams)}");
            }

            foreach (var missingParam in missingParams)
            {
                bool found;
                object value;
                (found, value) = FindValueInParentRows(missingParam, parentRows);

                if (found)
                {
                    resultParams.Add(missingParam, value);
                }
            }

            // check if all parameters found our not needed params are set, both will lead to an exeption
            var stillMissing = new HashSet<string>(neededParams);
            stillMissing.ExceptWith(resultParams.Keys);

            if (stillMissing.Count > 0)
            {
                throw new GenericDbQueryException(GenericDbQueryExceptionCode.PARAMS_MISSING, $"Needed parameter(s) '{string.Join(", ", stillMissing)}' not provided in parent rows nor in query parameters");
            }

            var notNeededParams = new HashSet<string>(commandParams.Keys);
            notNeededParams.ExceptWith(neededParams);

            if (notNeededParams.Count > 0)
            {
                throw new GenericDbQueryException(GenericDbQueryExceptionCode.PARAMS_NOTNEEDED, $"Not needed parameter(s) >{string.Join(", ", notNeededParams)},< provided to query. Remove not needed query parameters");
            }

            return resultParams;
        }

        private (bool found, object value) FindValueInParentRows(string missingParam, List<Dictionary<string, object>> parentRows)
        {
            int level = 0;
            foreach(var parentRow in parentRows)
            {
                if (parentRow.ContainsKey(missingParam))
                {
                    var ret = (true, parentRow[missingParam]);

                    logger.LogInformation($"Found missing param {missingParam} in parent row level {level}");
                    return ret;
                }
                level++;
            }

            return (false, null);
        }
    }
}
