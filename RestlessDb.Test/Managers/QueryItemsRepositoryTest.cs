using Microsoft.Extensions.Logging;
using Moq;
using RestlessDb.DataLayer;
using RestlessDb.Repositories;
using Xunit;

namespace GenericDbRestApi.Test.DataLayer
{
    [Trait("Category", "UnitTest")]
    public class QueryItemsRepositoryTest
    {
        [Fact]
        public void WhenLoadQueryItemThenCorrectQueryItemExtWithChildrenReturned()
        {
            var moqHelper = new GenericSqlHelperMoq();
            var genericSqlHelperMoq = moqHelper.SetupMoq4GenericSqlHelper();
            var sut = new QueryItemsRepository(genericSqlHelperMoq, new Mock<ILogger<QueryItemsRepository>>().Object);
            var queryItemExt = sut.GetQueryItemExt("salesorders");

            Assert.True(queryItemExt.Children.Count == 1);
            Assert.Equal(moqHelper.SalesOrderColumns, queryItemExt.Columns);
            Assert.Equal(moqHelper.SalesOrderDetailColumns, queryItemExt.Children[0].Columns);
        }
    }
}