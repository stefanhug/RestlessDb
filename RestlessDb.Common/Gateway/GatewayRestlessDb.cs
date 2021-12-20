using Newtonsoft.Json;
using RestlessDb.Common.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace RestlessDb.Common.Gateway
{
    public class GatewayRestlessDb
    {
        private const string EP_QUERYITEM = "/dbapi/admin/queryitems";
        private const string EP_ALLQUERIES = "/dbapiconfig/allqueries";
        private const string EP_QUERIES = "/dbapi";



        private readonly HttpClient httpClient;

        public GatewayRestlessDb(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        public async Task<List<QueryItem>> FetchQueryItemsAsync()
        {
            return await httpClient.GetFromJsonAsync<List<QueryItem>>(EP_QUERYITEM);
        }

        public async Task<QueryItem> FetchQueryItemAsync(string itemName)
        {
            return await httpClient.GetFromJsonAsync<QueryItem>($"{EP_QUERYITEM}/{itemName}");
        }

        public async Task<QueryItem> InsertQueryItemAsync(QueryItem queryItem)
        {
            var json = JsonConvert.SerializeObject(queryItem);
            var data = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await httpClient.PostAsync(EP_QUERYITEM, data);

            if (response.StatusCode != HttpStatusCode.Created)
            {
                throw new Exception($"Unexpected HTTP returncode {response.StatusCode} for POST message");
            }

            string result = response.Content.ReadAsStringAsync().Result;
            var ret = JsonConvert.DeserializeObject<QueryItem>(result);

            return ret;
        }

        public async Task<QueryItem> UpdateQueryItemAsync(QueryItem queryItem)
        {
            var json = JsonConvert.SerializeObject(queryItem);
            var data = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await httpClient.PutAsync(EP_QUERYITEM, data);

            if (response.StatusCode != HttpStatusCode.OK)
            {
                throw new Exception($"Unexpected HTTP returncode {response.StatusCode} for PUT message");
            }

            string result = response.Content.ReadAsStringAsync().Result;
            var ret = JsonConvert.DeserializeObject<QueryItem>(result);

            return ret;
        }

        public async Task<bool> DeleteQueryItemAsync(string itemName)
        {
            var response = await httpClient.DeleteAsync($"{EP_QUERYITEM}/{itemName}");

            if (response.StatusCode != HttpStatusCode.OK)
            {
                throw new Exception($"Unexpected HTTP returncode {response.StatusCode} for POST message");
            }

            return true;
        }

        public async Task<string> FetchQueryContentAsync(string queryItemName, string format, int offset, int maxRows)
        {
            var content = await httpClient.GetStringAsync($"dbapi/{queryItemName}?outputformat={format}&offset={offset}&maxrows={maxRows}");
            return content;
        }

        public async Task<List<QueryConfigItem>> GetQueryConfigAsync()
        {
            return (await httpClient.GetFromJsonAsync<QueryConfigResult>(EP_ALLQUERIES)).QueryConfigItems;
        }
    }
}



