using Microsoft.AspNetCore.Components;
using RestlessDb.Common.Types;
using System;

namespace RestlessDb.Client.Shared
{
    public partial class QueryResultTable
    {
        [Parameter, EditorRequired]
        public QueryResult QueryResult { get; set; }
        [Parameter, EditorRequired]
        public TableDisplayOptions TableDisplayOptions { get; set; }

        public bool Loading { get; set; }

        public string TableLabel { get => QueryResult == null ? string.Empty : $"{QueryResult.MetaData.Label} - {QueryResult.MetaData.Description}"; }
        public string TableDescription
        {
            get
            {
                if (QueryResult == null)    
                    return string.Empty;

                return $"Showing {QueryResult.RetrievedRows} rows" + (QueryResult.HasMoreRows ? "More rows available" : string.Empty);
            }
        }
    }
}