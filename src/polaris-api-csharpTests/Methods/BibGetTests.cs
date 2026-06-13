namespace Clc.Polaris.Api.Tests
{
    [TestClass]
    public sealed class BibGetTests : IntegrationTestBase
    {
        [TestMethod]
        [ReadOnlyIntegrationTest]
        public async Task BibGetTest()
        {
            var response = await Papi.BibGetAsync(478907, cancellationToken: TestContext.CancellationToken);
            Assert.AreEqual(0, response.Data.PAPIErrorCode);
            Assert.IsFalse(string.IsNullOrWhiteSpace(response.Data.Title));
            Assert.Contains("100/1/bib", response.Response.RequestMessage.RequestUri.ToString());
        }

        [TestMethod]
        [ReadOnlyIntegrationTest]
        public async Task BibGetTest_PassBranchId()
        {
            var response = await Papi.BibGetAsync(478907, 7, TestContext.CancellationToken);
            Assert.AreEqual(0, response.Data.PAPIErrorCode);
            Assert.IsFalse(string.IsNullOrWhiteSpace(response.Data.Title));
            Assert.Contains("100/7/bib", response.Response.RequestMessage.RequestUri.ToString());
        }
    }
}
