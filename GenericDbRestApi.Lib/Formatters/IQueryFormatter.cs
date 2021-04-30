using GenericDbRestApi.Lib.Types;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GenericDBRestApi.Lib.Formatters
{
    public interface IQueryFormatter
    {
        abstract string ContentType { get; }
        abstract string OutputFormat { get; }
        ActionResult GetActionResult(QueryResult queryResult);
    }
}
