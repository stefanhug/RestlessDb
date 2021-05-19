using RestlessDb.DataLayer;
using Xunit;

namespace GenericDbRestApi.Test.DataLayer
{
    [Trait("Category", "UnitTest")]
    public class QueryItemProviderTest
    {
        [Fact]
        public void WhensSalesOrderLoadedThenCorrectQueryItemReturned()
        {
            var moqHelper = new GenericSqlHelperMoq();
            var genericSqlHelperMoq = moqHelper.SetupMoq4GenericSqlHelper();
            var sut = new QueryItemProvider(genericSqlHelperMoq);

            var sutResult = sut.LoadQueryItem("salesorders");

            Assert.True(sutResult.ChildItems.Count == 1);
            Assert.True(sutResult.Columns.Count == moqHelper.SalesOrderColumns.Count);
            Assert.True(sutResult.ChildItems[0].Columns.Count == moqHelper.SalesOrderDetailColumns.Count);
        }       
    }
}