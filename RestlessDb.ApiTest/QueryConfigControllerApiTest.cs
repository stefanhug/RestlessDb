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

        [Fact]
        public async void WhenAllFormattersRequestedThenCorrectFormattersReturned()
        {
            var gw = TestGatewayBuilder.GetGateway();
            var ret = await gw.GetAllFormatters();

            Assert.Equal(5, ret.Count);
        }
    }
}
