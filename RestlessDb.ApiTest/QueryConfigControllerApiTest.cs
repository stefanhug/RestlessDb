using RestlessDb.Common.Types;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace RestlessDb.ApiTest
{
    public class QueryConfigControllerApiTest
    {
        [Fact]
        public async void WhenSimpleQueryItemRequestedThenCorrectNumberOfRowsReturned()
        {
            var gw = TestGatewayBuilder.GetGateway();
            var ret = await gw.GetQueryConfigAsync();
            
            Assert.True(ret.Count > 2);
        }
    }
}
