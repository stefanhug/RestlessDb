using Newtonsoft.Json;
using RestlessDb.Common.Types;
using Xunit;

namespace RestlessDb.ApiTest
{
    [Trait("Category", "ApiTest")]
    public class QueryControllerJsonOutputTest
    {
        [Theory]
        [InlineData("persons", 8000, 6)]
        [InlineData("persons?maxrows=10", 10, 6)]
        [InlineData("persons?maxrows=10&offset=8000", 10, 6)]
        [InlineData("personsbylastname?lastname=Smith", 103, 6)]
        [InlineData("jobcandidates", 13, 16)]
        public async void WhenSimpleQueryItemRequestedThenCorrectNumberOfRowsReturned(string pathAndQuery, int? resultRows, int? resultColumns)
        {
            var response = await new ApiTestRequestHandler().GetClientResponse(pathAndQuery);

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
            var response = await new ApiTestRequestHandler().GetClientResponse(pathAndQuery);
            Assert.Equal(System.Net.HttpStatusCode.NotFound, response.StatusCode);
            Assert.Equal("text/plain", response.Content.Headers.ContentType.MediaType);
        }

        [Theory]
        [Trait("Category", "ApiTest")]
        [InlineData("persons?unusedparam=foo")]
        [InlineData("personsbylastname")]
        public async void WhenWrongParamsAreGivenThenBadRequestIsReturned(string pathAndQuery)
        {
            var response = await new ApiTestRequestHandler().GetClientResponse(pathAndQuery);
            Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.StatusCode);
            Assert.Equal("text/plain", response.Content.Headers.ContentType.MediaType);
        }

        
        [Theory]
        [Trait("Category", "ApiTest")]
        [InlineData("salesorders?maxrows=10", 10, 4, 1)]
        [InlineData("salesorders?offset=10&maxrows=10", 10, 4, 1)]
        public async void WhenHierachicalQueryItemRequestedThenCorrectNumberOfRowsAndChildrenReturned(string pathAndQuery, int? resultRows, int? topResultColumns, int? firstLevelChildren)
        {
            var response = await new ApiTestRequestHandler().GetClientResponse(pathAndQuery);

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
    }
}

