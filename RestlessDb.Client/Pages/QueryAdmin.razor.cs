using RestlessDb.Common.Types;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RestlessDb.Client.Pages
{
    public partial class QueryAdmin
    {
        private string ErrorMessage { get; set; }
        private List<QueryMetaData> queryMetaDatas = null;

        protected override async Task OnInitializedAsync()
        {
            if (!await clientModel.CheckInitAsync())
            {
                ErrorMessage = clientModel.ErrorMessage;
            }
            else
            {
                queryMetaDatas = clientModel.QueryMetaDatas;
            }
        }

        private void CreateNewQuery(string parentQuery)
        {
            uriHelper.NavigateTo($"/qryitemnew?parent={parentQuery}");
        }
    }
}
