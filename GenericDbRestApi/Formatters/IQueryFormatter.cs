using GenericDbRestApi.Types;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GenericDBRestApi.Formatters
{
    public interface IQueryFormatter
    {
        abstract string ContentType { get; }
        abstract string OutputFormat { get; }
        ActionResult GetActionResult(GenericQueryResult queryResult);
    }
}
