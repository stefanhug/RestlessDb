using RestlessDb.Types;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Xunit;

namespace GenericDbRestApi.ApiTest
{
    [Trait("Category", "ApiTest")]
    public class ClientJsonOutputTest
    {
        public ClientJsonOutputTest()
        {
            // todo: get from config
            authority = "https://localhost:5001/dbapi/";
        }

        [Theory]
        [InlineData("persons", 8000, 6)]
        [InlineData("persons?maxrows=10", 10, 6)]
        [InlineData("persons?maxrows=10&offset=8000", 10, 6)]
        [InlineData("personsbylastname?lastname=Smith", 103, 6)]
        [InlineData("jobcandidates", 13, 16)]
        public async void WhenSimpleQueryItemRequestedThenCorrectNumberOfRowsReturned(string pathAndQuery, int? resultRows, int? resultColumns)
        {
            var response = await GetClientResponse(pathAndQuery);

            Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);
            Assert.Equal("application/json", response.Content.Headers.ContentType.MediaType);

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

        [Theory]
        [Trait("Category", "ApiTest")]
        [InlineData("personss")]
        [InlineData("personss?maxrows=10")]
        public async void WhenNonExistingQueryIsRequestedThenNotFoundIsReturned(string pathAndQuery)
        {
            var response = await GetClientResponse(pathAndQuery);
            Assert.Equal(System.Net.HttpStatusCode.NotFound, response.StatusCode);
            Assert.Equal("text/plain", response.Content.Headers.ContentType.MediaType);
        }

        [Theory]
        [Trait("Category", "ApiTest")]
        [InlineData("persons?unusedparam=foo")]
        [InlineData("personsbylastname")]
        public async void WhenWrongParamsAreGivenThenBadRequestIsReturned(string pathAndQuery)
        {
            var response = await GetClientResponse(pathAndQuery);
            Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.StatusCode);
            Assert.Equal("text/plain", response.Content.Headers.ContentType.MediaType);
        }


        private async Task<HttpResponseMessage> GetClientResponse(string pathAndQuery)
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

        [Theory]
        [Trait("Category", "ApiTest")]
        [InlineData("salesorders?maxrows=10", 10, 4, 1)]
        [InlineData("salesorders?offset=10&maxrows=10", 10, 4, 1)]
        public async void WhenHierachicalQueryItemRequestedThenCorrectNumberOfRowsAndChildrenReturned(string pathAndQuery, int? resultRows, int? topResultColumns, int? firstLevelChildren)
        {
            var response = await GetClientResponse(pathAndQuery);

            Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);
            Assert.Equal("application/json", response.Content.Headers.ContentType.MediaType);

            var responseString = await response.Content.ReadAsStringAsync();
            var queryResult = JsonConvert.DeserializeObject<QueryResult>(responseString);
            if (resultRows.HasValue)
            {
                Assert.Equal(resultRows, queryResult.Data.Count);
            }

            if (topResultColumns.HasValue)
            {
                Assert.Equal(topResultColumns, queryResult.MetaData.Columns.Count);
            }
            if (firstLevelChildren.HasValue)
            {
                Assert.Equal(firstLevelChildren, queryResult.MetaData.Children.Count);
            }
        }

        private readonly string authority;
    }
}

