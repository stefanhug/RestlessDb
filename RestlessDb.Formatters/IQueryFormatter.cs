using Microsoft.AspNetCore.Mvc;
using RestlessDb.Common.Types;

namespace RestlessDb.Formatters
{
    public interface IQueryFormatter
    {
        abstract string ContentType { get; }
        abstract string OutputFormat { get; }
        abstract string Label { get; }
        abstract string Description { get; }
        abstract string FileExtension { get; }
        abstract Disposition Disposition{ get; }
        ActionResult GetActionResult(QueryResult queryResult);
    }
}
