using RestlessDb.Formatters;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Xunit;

namespace GenericDbRestApi.Test.Formatters
{
    [Trait("Category", "UnitTest")]
    public class DbQueryHtmlTest
    {
        const string TEST_RESOURCE_NAME = "TestRazorTemplate.cshtml";

        [Fact]
        public void WhenTemplateAndDataIsGivenThenCorrectHtmlCreated()
        {
            var inputData = new QueryResultTestProvider().CreateBasicQueryResult();
            var fullResourceName = this.GetType().Assembly.GetManifestResourceNames().First(n => n.Contains(TEST_RESOURCE_NAME));
            var testTemplateStream = this.GetType().Assembly.GetManifestResourceStream(fullResourceName);
            var reader = new StreamReader(testTemplateStream);
            var testTemplate = reader.ReadToEnd();
            var sut = new DbQueryHtml(inputData, testTemplate);
            var resultString = sut.GetAsString();
            Assert.True(resultString.Length > 0);

            var document = XDocument.Parse(resultString);
            Assert.True(document.Descendants("table").Count() == 1);
            Assert.True(document.Descendants("tr").Count() == inputData.Data.Count);
        }

        [Fact]
        public void WhenTemplateAndDataWithNullValuesIsGivenThenCorrectHtmlCreated()
        {
            var inputData = new QueryResultTestProvider().CreateBasicQueryResultWithNullValues();
            var fullResourceName = this.GetType().Assembly.GetManifestResourceNames().First(n => n.Contains(TEST_RESOURCE_NAME));
            var testTemplateStream = this.GetType().Assembly.GetManifestResourceStream(fullResourceName);
            var reader = new StreamReader(testTemplateStream);
            var testTemplate = reader.ReadToEnd();
            var sut = new DbQueryHtml(inputData, testTemplate);
            var resultString = sut.GetAsString();
            Assert.True(resultString.Length > 0);

            var document = XDocument.Parse(resultString);
            Assert.True(document.Descendants("table").Count() == 1);
            Assert.True(document.Descendants("tr").Count() == inputData.Data.Count);
        }
    }
}
