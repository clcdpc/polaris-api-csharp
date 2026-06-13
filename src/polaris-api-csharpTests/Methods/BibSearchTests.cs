namespace Clc.Polaris.Api.Tests
{
    [TestClass]
    public sealed class BibSearchTests : IntegrationTestBase
    {
        [TestMethod]
        [ReadOnlyIntegrationTest]
        public async Task BibSearchTest()
        {
            var response = await Papi.BibSearchAsync(new BibSearchOptions { Term = "dogs", PageSize = 10 }, TestContext.CancellationToken);
            Assert.AreEqual(10, response.Data.PAPIErrorCode);
            Assert.AreEqual("dogs ", response.Data.WordList);
            Assert.IsGreaterThan(10000, response.Data.TotalRecordsFound);
        }
    }
}
