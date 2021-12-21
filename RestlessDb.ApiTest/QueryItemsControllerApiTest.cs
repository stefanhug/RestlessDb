using RestlessDb.Common.Types;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace RestlessDb.ApiTest
{
    public class QueryItemsControllerApiTest
    {
        [Fact]
        [Trait("Category", "ApiTest")]
        public async void WhenAllGetQueryItemsThenItemsReturned()
        {
            var gw = TestGatewayBuilder.GetGateway();
            var ret = await gw.FetchQueryItemsAsync();
            Assert.True(ret.Count > 2);
        }

        [Fact]
        [Trait("Category", "ApiTest")]
        [InlineData("dbapi/admin/queryitems/Persons")]
        public async void WhenSingleGetQueryItemsThenItemReturned()
        {
            var gw = TestGatewayBuilder.GetGateway();
            var ret = await gw.FetchQueryItemAsync("Persons");
            Assert.Equal("Persons", ret.Label);
        }

        [Fact]
        [Trait("Category", "ApiTest")]
        // still not convinced that only one assert and one action per integration test. How to test insert/update/delete then?
        public async void WhenDoingCreateDeleteThenItemNumberIsTheSame()
        {
            var gw = TestGatewayBuilder.GetGateway();
            var ret = await gw.FetchQueryItemsAsync();
            var numberofitems = ret.Count;
            var newQueryItem = new QueryItem()
            {
                Id = null,
                Name = "TestPutRequestQryItem_271",
                Label = "TestPutRequestQryItem_271_Lbl",
                Description = "TestPutRequestQryItem_271_Description",
                Sql = "select * from INFORMATION_SCHEMA.Tables order by TABLE_NAME",
                Pos = 0,
                Parent = null
            };
            
            // ins
            var retItem = await gw.InsertQueryItemAsync(newQueryItem);
            Assert.NotNull(retItem.Id);
            var newNumberOfItems = (await gw.FetchQueryItemsAsync()).Count;
            Assert.Equal(numberofitems + 1, newNumberOfItems);

            //upd
            newQueryItem.Label += "UPD";
            retItem = await gw.UpdateQueryItemAsync(newQueryItem);
            Assert.Equal(newQueryItem.Label, retItem.Label);

            //del
            await gw.DeleteQueryItemAsync(newQueryItem.Name);
            newNumberOfItems = (await gw.FetchQueryItemsAsync()).Count;
            Assert.Equal(numberofitems, newNumberOfItems);
        }
    }
}
