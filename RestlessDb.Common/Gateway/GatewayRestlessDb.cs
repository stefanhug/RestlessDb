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
        private const string EP_ALLFORMATTERS = "/dbapiconfig/formatters";


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

            await CheckReturnCode(HttpStatusCode.Created, "POST", response);

            string result = response.Content.ReadAsStringAsync().Result;
            var ret = JsonConvert.DeserializeObject<QueryItem>(result);

            return ret;
        }

        public async Task<QueryItem> UpdateQueryItemAsync(QueryItem queryItem)
        {
            var json = JsonConvert.SerializeObject(queryItem);
            var data = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await httpClient.PutAsync(EP_QUERYITEM, data);

            await CheckReturnCode(HttpStatusCode.OK, "PUT", response);

            string result = response.Content.ReadAsStringAsync().Result;
            var ret = JsonConvert.DeserializeObject<QueryItem>(result);

            return ret;
        }

        public async Task<bool> DeleteQueryItemAsync(string itemName)
        {
            var response = await httpClient.DeleteAsync($"{EP_QUERYITEM}/{itemName}");
            await CheckReturnCode(HttpStatusCode.OK, "DELETE", response);
            return true;
        }

        public async Task<HttpResponseMessage> FetchQueryContentAsync(string queryItemName, string format, int offset, int maxRows, Dictionary<string, string> parameters)
        {
            var paramsQueryString = string.Empty;

            if (parameters?.Count > 0)
            {
                paramsQueryString = string.Join("",parameters.Select(kvp => $"&{kvp.Key}={kvp.Value}"));
            }

            var response = await httpClient.GetAsync($"dbapi/{queryItemName}?outputformat={format}&offset={offset}&maxrows={maxRows}{paramsQueryString}");
            await CheckReturnCode(HttpStatusCode.OK, "FetchQueryContentAsync", response);
            return response;
        }

        public async Task<List<QueryMetaData>> GetQueryConfigAsync()
        {
            var response = await httpClient.GetAsync(EP_ALLQUERIES);
            await CheckReturnCode(HttpStatusCode.OK, "GetQueryConfigAsync", response);
            string result = response.Content.ReadAsStringAsync().Result;
            var ret = JsonConvert.DeserializeObject<List<QueryMetaData>>(result);
            return ret;
        }
        
        public async Task<HttpResponseMessage> GetClientResponse(string pathAndQuery)
        {
            var response = await httpClient.GetAsync(pathAndQuery);
            return response;
        }

        public async Task<List<FormatterInfo>> GetAllFormatters()
        {
            //the System.Text json deserializer needs special handling for ENUMS, Newtonsoft does it OOB
            var response = await httpClient.GetAsync(EP_ALLFORMATTERS);
            await CheckReturnCode(HttpStatusCode.OK, "GetAllFormatters", response);
            string result = response.Content.ReadAsStringAsync().Result;
            var ret = JsonConvert.DeserializeObject<List<FormatterInfo>>(result);
            return ret;
        }

        private static async Task CheckReturnCode(HttpStatusCode expectedHttpStatusCode, string actionName, HttpResponseMessage message)
        {
            if (message.StatusCode != expectedHttpStatusCode)
            {
                throw new HttpRequestException($"Unexpected HTTP returncode {message.StatusCode} for {actionName} message.\r\n{await message.Content.ReadAsStringAsync()}", null, message.StatusCode);
            }

            return;
        }
    }
}



