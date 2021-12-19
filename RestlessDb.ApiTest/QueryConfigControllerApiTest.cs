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
        [InlineData("dbapiconfig/allqueries")]
        public async void WhenSimpleQueryItemRequestedThenCorrectNumberOfRowsReturned(string pathAndQuery)
        {
            var ret = await new ApiTestRequestHandler().GetJson<QueryConfigResult>(pathAndQuery);

            Assert.True(ret.QueryConfigItems.Count > 2);
        }
    }
}
