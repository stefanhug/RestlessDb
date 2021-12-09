using Newtonsoft.Json;
using RestlessDb.Common.Types;
using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace RestlessDb.Client.Model
{
    public class ClientModel
    {
        private readonly HttpClient httpClient;
                
        public QueryConfigResult QueryConfig { get; private set; }
        public string ErrorMessage { get; private set;  } = null;

        public ClientModel(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        public async Task<QueryConfigItem> GetConfigItemAsync(string name)
        {
            await CheckInitAsync();
            return QueryConfig.QueryConfigItems.Find(i => i.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase));
        }

        public async Task<QueryAdminResult> FetchQueryItemsAsync()
        {
            var responseString = await httpClient.GetStringAsync("dbapi/admin/queryitems");
            var ret = JsonConvert.DeserializeObject<QueryAdminResult>(responseString);
            return ret;
        }

        public async Task<string> FetchQueryContentAsync(string queryItemName, string format, int offset, int maxRows)
        {
            var content = await httpClient.GetStringAsync($"dbapi/{queryItemName}?outputformat={format}&offset={offset}&maxrows={maxRows}");
            return content;
        }

        public async Task CheckInitAsync()
        {
            try
            {
                if (QueryConfig != null)
                {
                    return;
                }
                QueryConfig = await httpClient.GetFromJsonAsync<QueryConfigResult>("dbapiconfig/allqueries");
            }
            catch (HttpRequestException e)
            {
                ErrorMessage = $"HttpRequestException: httpstatus: {e.StatusCode}\nMessage:{e.Message}\n=====\nType: {e.GetType()}\nStack trace:\n==========\n{e.StackTrace}";
            }
            catch (Exception e)
            {
                ErrorMessage = $"{e.Message}\nType: {e.GetType()}\nStack trace:\n==========\n{e.StackTrace}";
            }
        }
    }
}
