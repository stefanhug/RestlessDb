using System.Net.Http;
using System.Threading.Tasks;
using System.Net.Http.Json;
using System.Text;
using Newtonsoft.Json;
using System.Net;
using System;
using RestlessDb.Common.Gateway;

namespace RestlessDb.ApiTest
{
    public static class TestGatewayBuilder
    {
        private const string authority = "https://localhost:33333/";

        public static GatewayRestlessDb GetGateway()
        {
            var httpClient = new HttpClient { BaseAddress = new Uri(authority) };
            return new GatewayRestlessDb(httpClient);
        }
    }
}
