using GenericDbRestApi.Lib.Types;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Net.Http;
using Xunit;

namespace GenericDbRestApi.ApiTest
{
    public class ClientJsonOutputTest
    {
        public ClientJsonOutputTest()
        {
            // todo: get from config
            authority = "https://localhost:5001/dbapi/";
        }

        [Theory]
        [Trait("Category", "ApiTest")]
        [InlineData("persons", 8000, 6)]
        [InlineData("persons?maxrows=10", 10, 6)]
        [InlineData("persons?maxrows=10&offset=8000", 10, 6)]
        [InlineData("personsbylastname?lastname=Smith", 103, 6)]
        [InlineData("jobcandidates", 13, 16)]

        public async void WhenSimpleQueryItemRequestedThenCorrectNumberOfRowsReturned(string pathAndQuery, int? resultRows, int? resultColumns)
        {
            var clientHandler = new HttpClientHandler()
            {
                ServerCertificateCustomValidationCallback = delegate { return true; },
            };
            var client = new HttpClient(clientHandler);
            var requestUri = authority + pathAndQuery;
            var response = await client.GetAsync(requestUri);
            Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);
            var responseString = await response.Content.ReadAsStringAsync();
            var queryResult = JsonConvert.DeserializeObject<QueryResult>(responseString);
            if(resultRows.HasValue)
            {
                Assert.Equal(resultRows, queryResult.Data.Count);
            }

            if (resultColumns.HasValue)
            {
                Assert.Equal(resultColumns, queryResult.MetaData.Columns.Count);
            }
        }

        private readonly string authority;
    }
}

