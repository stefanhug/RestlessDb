using System;

namespace RestlessDb.Common.Types
{
    public enum GenericDbQueryExceptionCode
    {
        QUERY_NOTFOUND, RECURSION, DBQUERY, FORMATTER_NOTFOUND,
        PARAMS_MISSING, PARAMS_NOTNEEDED, TEMPLATE_ERROR,
        SQL_MUST_HAVE_ORDER_BY
    }

    public class GenericDbQueryException : Exception
    {
        public GenericDbQueryException(GenericDbQueryExceptionCode code, string message) : base(message) 
        {
            ExceptionCode = code;
        }

        public GenericDbQueryExceptionCode ExceptionCode { get; }
    }
}
