using System;

namespace RestlessDb.Common.Types
{
    public enum GenericDbQueryExceptionCode
    {
        QUERY_NOTFOUND, RECURSION, DBQUERY, FORMATTER_NOTFOUND,
        PARAMS_MISSING, PARAMS_NOTNEEDED, TEMPLATE_ERROR,
        SQL_MUST_HAVE_ORDER_BY, DUPLICATE_KEY, DML_ERROR, ITEM_NOTFOUND
    }

    public class GenericDbQueryException : Exception
    {
        public GenericDbQueryException(GenericDbQueryExceptionCode code, string message) : base(message) 
        {
            ExceptionCode = code;
        }

        public override string Message => $"Exception: {ExceptionCode.ToString()}\r\n{base.Message}";

        public GenericDbQueryExceptionCode ExceptionCode { get; }
    }
}
