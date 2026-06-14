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
            Assert.AreEqual(response.Data.BibSearchRows.Count, response.Data.PAPIErrorCode);
            Assert.AreEqual("dogs ", response.Data.WordList);
            Assert.IsGreaterThan(10000, response.Data.TotalRecordsFound);
        }

        [TestMethod]
        [ReadOnlyIntegrationTest]
        public async Task BibSearchAsync_KeywordWithSpacesPagingAndBranchReturnsRows()
        {
            var branchId = RequireConfiguredBranch();

            var response = await Papi.BibSearchAsync(new BibSearchOptions
            {
                Term = "the book",
                Page = 1,
                PageSize = 5,
                Branch = branchId,
            }, TestContext.CancellationToken);

            Assert.AreEqual(response.Data.BibSearchRows.Count, response.Data.PAPIErrorCode);
            Assert.IsNotEmpty(response.Data.BibSearchRows);
        }
    }
}
