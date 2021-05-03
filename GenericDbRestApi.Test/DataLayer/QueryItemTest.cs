using GenericDbRestApi.Lib.DataLayer;
using Xunit;

namespace GenericDbRestApi.Test.DataLayer
{
    public class QueryItemTest
    {
        [Fact]
        public void WhenQueryItemGivenThenCorrectConversionToMetaDataReturned()
        {
            var moqHelper = new GenericSqlHelperMoq();
            var genericSqlHelperMoq = moqHelper.SetupMoq4GenericSqlHelper();
            var provider = new QueryItemProvider(genericSqlHelperMoq);
            var queryItem = provider.LoadQueryItem("salesorders");
            var sut = queryItem.AsQueryMetaData();


            Assert.True(sut.Children.Count == 1);
            Assert.True(sut.Columns.Count == moqHelper.SalesOrderColumns.Count);
            Assert.True(sut.Children[0].Columns.Count == moqHelper.SalesOrderDetailColumns.Count);
        }
    }
}