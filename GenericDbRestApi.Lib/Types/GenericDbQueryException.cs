using System;
using System.Collections.Generic;
using System.Text;

namespace GenericDbRestApi.Lib.Types
{
    public enum GenericDbQueryExceptionCode
    {
        QUERY_NOTFOUND, RECURSION, DBQUERY, FORMATTER_NOTFOUND,
        PARAMS_MISSING, PARAMS_NOTNEEDED, TEMPLATE_ERROR
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
