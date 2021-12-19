using Newtonsoft.Json;
using RestlessDb.Common.Types;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace RestlessDb.Client.Model
{
    public class ClientModel
    {
        private string QUERYITEM_URL = "/dbapi/admin/queryitems";
        private readonly HttpClient httpClient;

        private List<QueryConfigItem> queryConfigItems;
        public List<QueryConfigItem> QueryConfigItems
        {
            get => queryConfigItems;
        }

        public string ErrorMessage { get; private set;  } = null;

        public ClientModel(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        public async Task<List<QueryConfigItem>> GetQueryConfigItemsAsync()
        {
            await CheckInitAsync();
            return queryConfigItems;
        }

        public async Task<QueryConfigItem> GetConfigItemAsync(string name)
        {
            await CheckInitAsync();
            return (await GetQueryConfigItemsAsync()).Find(i => i.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase));
        }

        public async Task<List<QueryItem>> FetchQueryItemsAsync()
        {
            var responseString = await httpClient.GetStringAsync(QUERYITEM_URL);
            var ret = JsonConvert.DeserializeObject<List<QueryItem>>(responseString);
            return ret;
        }

        public async Task<QueryItem> FetchQueryItemAsync(string itemName)
        {
            var responseString = await httpClient.GetStringAsync($"{QUERYITEM_URL}/{itemName}");
            var ret = JsonConvert.DeserializeObject<QueryItem>(responseString);
            return ret;
        }

        public async Task<QueryItem> InsertQueryItemAsync(QueryItem queryItem)
        {
            var json = JsonConvert.SerializeObject(queryItem);
            var data = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await httpClient.PostAsync(QUERYITEM_URL, data);

            if (response.StatusCode != HttpStatusCode.Created)
            {
                throw new Exception($"Unexpected HTTP returncode {response.StatusCode} for POST message");
            }

            string result = response.Content.ReadAsStringAsync().Result;
            var ret = JsonConvert.DeserializeObject<QueryItem>(result);
            InvalidateCaches();

            return ret;
        }

        public async Task<QueryItem> UpdateQueryItemAsync(QueryItem queryItem)
        {
            var json = JsonConvert.SerializeObject(queryItem);
            var data = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await httpClient.PutAsync(QUERYITEM_URL, data);

            if (response.StatusCode != HttpStatusCode.OK)
            {
                throw new Exception($"Unexpected HTTP returncode {response.StatusCode} for PUT message");
            }

            string result = response.Content.ReadAsStringAsync().Result;
            var ret = JsonConvert.DeserializeObject<QueryItem>(result);
            InvalidateCaches();

            return ret;
        }

        public async Task<bool> DeleteQueryItemAsync(string itemName)
        {
            var response = await httpClient.DeleteAsync($"{QUERYITEM_URL}/{itemName}");

            if (response.StatusCode != HttpStatusCode.OK)
            {
                throw new Exception($"Unexpected HTTP returncode {response.StatusCode} for POST message");
            }

            InvalidateCaches();

            return true;
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
                if (queryConfigItems != null)
                {
                    return;
                }
                queryConfigItems = (await httpClient.GetFromJsonAsync<QueryConfigResult>("dbapiconfig/allqueries")).QueryConfigItems;
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

        private void InvalidateCaches()
        {
            queryConfigItems = null;
        }
    }
}
