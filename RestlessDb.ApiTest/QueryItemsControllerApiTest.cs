using RestlessDb.Common.Types;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace RestlessDb.ApiTest
{
    public class QueryItemsControllerApiTest
    {
        [Theory]
        [Trait("Category", "ApiTest")]
        [InlineData("dbapi/admin/queryitems")]
        public async void WhenAllGetQueryItemsThenItemsReturned(string pathAndQuery)
        {
            var ret = await new ApiTestRequestHandler().GetJson<List<QueryItem>>(pathAndQuery);
            Assert.True(ret.Count > 2);
        }

        [Theory]
        [Trait("Category", "ApiTest")]
        [InlineData("dbapi/admin/queryitems/Persons")]
        public async void WhenSingleGetQueryItemsThenItemReturned(string pathAndQuery)
        {
            var ret = await new ApiTestRequestHandler().GetJson<QueryItem>(pathAndQuery);
            Assert.Equal("Persons", ret.Label);
        }

        [Theory]
        [Trait("Category", "ApiTest")]
        [InlineData("dbapi/admin/queryitems")]
        // still not convinced that only one assert and one action per integration test. How to test insert/update/delete then?
        public async void WhenDoingCreateDeleteThenItemNumberIsTheSame(string pathAndQuery)
        {
            var ret = await new ApiTestRequestHandler().GetJson<List<QueryItem>>(pathAndQuery);
            var numberofitems = ret.Count;
            var newQueryItem = new QueryItem()
            {
                Id = null,
                Name = "TestPutRequestQryItem_271",
                Label = "TestPutRequestQryItem_271_Lbl",
                Description = "TestPutRequestQryItem_271_Description",
                Sql = "select * from dual",
                Pos = 0,
                Parent = null
            };
            
            // ins
            var retItem = await new ApiTestRequestHandler().PostJson<QueryItem>(pathAndQuery, newQueryItem);
            Assert.NotNull(retItem.Id);
            var newNumberOfItems = (await new ApiTestRequestHandler().GetJson<List<QueryItem>>(pathAndQuery)).Count;
            Assert.Equal(numberofitems + 1, newNumberOfItems);



            //upd
            newQueryItem.Label += "UPD";
            retItem = await new ApiTestRequestHandler().PutJson<QueryItem>(pathAndQuery, newQueryItem);
            Assert.Equal(newQueryItem.Label, retItem.Label);

            //del
            await new ApiTestRequestHandler().Delete($"{pathAndQuery}/{newQueryItem.Name}");
            newNumberOfItems = (await new ApiTestRequestHandler().GetJson<List<QueryItem>>(pathAndQuery)).Count;
            Assert.Equal(numberofitems, newNumberOfItems);
        }



    }
}
