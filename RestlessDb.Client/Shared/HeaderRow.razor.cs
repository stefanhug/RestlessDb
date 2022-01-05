using Microsoft.AspNetCore.Components;
using RestlessDb.Common.Types;
using System;

namespace RestlessDb.Client.Shared
{
    public partial class HeaderRow
    {
        [Parameter]
        public string Header { get; set; }
        [Parameter] 
        public string SubHeader { get; set; }
        [Parameter] 
        public string ErrorMessage { get; set; }
    }
}