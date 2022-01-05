using Microsoft.AspNetCore.Mvc;
using RestlessDb.Common.Types;
using System.Net;

namespace RestlessDb.Formatters
{
    public class QueryEmbeddedFormatter : QueryJsonFormatter
    {
        public override Disposition Disposition => Disposition.EMBEDDED;
        public override string OutputFormat => "htmlembedded";
        public override string Label => "Html embedded";
    }
}
