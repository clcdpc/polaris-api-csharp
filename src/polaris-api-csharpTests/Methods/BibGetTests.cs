namespace Clc.Polaris.Api.Tests
{
    [TestClass]
    public sealed class BibGetTests : IntegrationTestBase
    {
        [TestMethod]
        [ReadOnlyIntegrationTest]
        public async Task BibGetAsync_DefaultBranchReturnsConfiguredBib()
        {
            var bibId = RequireConfiguredBib();

            var response = await Papi.BibGetAsync(bibId, cancellationToken: TestContext.CancellationToken);

            Assert.AreEqual(0, response.Data.PAPIErrorCode);
            Assert.AreEqual(bibId, response.Data.ControlNumber);
            Assert.IsFalse(string.IsNullOrWhiteSpace(response.Data.Title));
            Assert.Contains($"/100/{Papi.OrganizationId}/bib", response.Response.RequestMessage.RequestUri.ToString());
        }

        [TestMethod]
        [ReadOnlyIntegrationTest]
        public async Task BibGetAsync_ExplicitBranchReturnsConfiguredBib()
        {
            var bibId = RequireConfiguredBib();
            var branchId = RequireConfiguredBranch();

            var response = await Papi.BibGetAsync(bibId, branchId, TestContext.CancellationToken);

            Assert.AreEqual(0, response.Data.PAPIErrorCode);
            Assert.AreEqual(bibId, response.Data.ControlNumber);
            Assert.IsFalse(string.IsNullOrWhiteSpace(response.Data.Title));
            Assert.Contains($"/100/{branchId}/bib", response.Response.RequestMessage.RequestUri.ToString());
        }
    }
}
