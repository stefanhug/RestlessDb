using System.Net.Http;
using System.Threading.Tasks;
using System.Net.Http.Json;


namespace RestlessDb.ApiTest
{
    public class ApiTestRequestHandler
    {
        private readonly string authority;

        public ApiTestRequestHandler() : this("https://localhost:5001/dbapi/") { }
 
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
            //{
            //    ServerCertificateCustomValidationCallback = delegate { return true; },
            //};
            var client = new HttpClient(clientHandler);
            var requestUri = authority + pathAndQuery;
            var ret =  await client.GetFromJsonAsync<T>(requestUri);
            return ret;
        }
    }
}
