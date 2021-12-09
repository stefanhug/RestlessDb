using RestlessDb.Common.Types;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace RestlessDb.Controllers
{
    public static class ExceptionStatusToHttpStatusCodeMapper
    {
        private static readonly Dictionary<GenericDbQueryExceptionCode, HttpStatusCode> ExceptionToStatusCodeMap =
            new Dictionary<GenericDbQueryExceptionCode, HttpStatusCode>()
            {
                {GenericDbQueryExceptionCode.DBQUERY, HttpStatusCode.InternalServerError},
                {GenericDbQueryExceptionCode.FORMATTER_NOTFOUND, HttpStatusCode.NotImplemented},
                {GenericDbQueryExceptionCode.QUERY_NOTFOUND, HttpStatusCode.NotFound},
                {GenericDbQueryExceptionCode.RECURSION, HttpStatusCode.InternalServerError},
                {GenericDbQueryExceptionCode.PARAMS_MISSING, HttpStatusCode.BadRequest},
                {GenericDbQueryExceptionCode.PARAMS_NOTNEEDED, HttpStatusCode.BadRequest},
                {GenericDbQueryExceptionCode.SQL_MUST_HAVE_ORDER_BY, HttpStatusCode.InternalServerError}
            };

        public static HttpStatusCode GetHttpStatusCode(GenericDbQueryException e)
        {
            HttpStatusCode ret;
            if (ExceptionToStatusCodeMap.TryGetValue(e.ExceptionCode, out ret))
                return ret;
            return HttpStatusCode.InternalServerError;
        }
    }
}
