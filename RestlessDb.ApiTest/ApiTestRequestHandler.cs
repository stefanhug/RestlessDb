using System.Net.Http;
using System.Threading.Tasks;
using System.Net.Http.Json;
using System.Text;
using Newtonsoft.Json;
using System.Net;
using System;

namespace RestlessDb.ApiTest
{
    public class ApiTestRequestHandler
    {
        private readonly string authority;

        public ApiTestRequestHandler() : this("https://localhost:5001/") { }
 
        public ApiTestRequestHandler(string authority)
        {
            this.authority = authority;
        }
        public  async Task<HttpResponseMessage> GetClientResponse(string pathAndQuery)
        {
            var clientHandler = new HttpClientHandler()
            {
                ServerCertificateCustomValidationCallback = delegate { return true; },
            };
            var client = new HttpClient(clientHandler);
            var requestUri = authority + pathAndQuery;
            var response = await client.GetAsync(requestUri);
            return response;
        }

        public async Task<T> GetJson<T>(string pathAndQuery)
        {
            var clientHandler = new HttpClientHandler();
            var client = new HttpClient(clientHandler);
            var requestUri = authority + pathAndQuery;
            var ret =  await client.GetFromJsonAsync<T>(requestUri);
            return ret;
        }

        public async Task<T> PostJson<T>(string pathAndQuery, object payload)
        {
            var clientHandler = new HttpClientHandler();
            var client = new HttpClient(clientHandler);
            var requestUri = authority + pathAndQuery;
  
            var json = JsonConvert.SerializeObject(payload);
            var data = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await client.PostAsync(requestUri, data);

            if (response.StatusCode != HttpStatusCode.Created)
            {
                throw new Exception($"Unexpected HTTP returncode {response.StatusCode} for POST message");
            }

            string result = response.Content.ReadAsStringAsync().Result;
            T ret = JsonConvert.DeserializeObject<T>(result);

            return ret;
        }

        public async Task<T> PutJson<T>(string pathAndQuery, object payload)
        {
            var clientHandler = new HttpClientHandler();
            var client = new HttpClient(clientHandler);
            var requestUri = authority + pathAndQuery;

            var json = JsonConvert.SerializeObject(payload);
            var data = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await client.PutAsync(requestUri, data);
            if (response.StatusCode != HttpStatusCode.OK)
            {
                throw new Exception($"Unexpected HTTP returncode {response.StatusCode} for PUT message");
            }

            string result = response.Content.ReadAsStringAsync().Result;
            T ret = JsonConvert.DeserializeObject<T>(result);

            return ret;
        }

        public async Task<bool> Delete(string pathAndQuery)
        {
            var clientHandler = new HttpClientHandler();
            var client = new HttpClient(clientHandler);
            var requestUri = authority + pathAndQuery;
           
            var response = await client.DeleteAsync(requestUri);

            if (response.StatusCode != HttpStatusCode.OK)
            {
                throw new Exception($"Unexpected HTTP returncode {response.StatusCode} for DELETE message");
            }

            return true;
        }
    }
}
