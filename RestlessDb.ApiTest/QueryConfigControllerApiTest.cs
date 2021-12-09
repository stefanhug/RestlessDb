using RestlessDb.Common.Types;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace RestlessDb.ApiTest
{
    public class QueryConfigControllerApiTest
    {
        [Theory]
        [InlineData("allqueries")]
        public async void WhenSimpleQueryItemRequestedThenCorrectNumberOfRowsReturned(string pathAndQuery)
        {
            var ret = await new ApiTestRequestHandler("https://localhost:5001/dbapiconfig/").GetJson<QueryConfigResult>(pathAndQuery);

            Assert.True(ret.QueryConfigItems.Count > 2);
        }
    }
}
