namespace Clc.Polaris.Api.Tests
{
    [TestClass]
    public sealed class HoldingsGetTests : IntegrationTestBase
    {
        [TestMethod]
        [ReadOnlyIntegrationTest]
        public async Task HoldingsGetTest()
        {
            var response = await Papi.HoldingsGetAsync(478907, TestContext.CancellationToken);
            Assert.IsNotEmpty(response.Data.BibHoldingsGetRows);
        }

        [TestMethod]
        [ReadOnlyIntegrationTest]
        public async Task HoldingsGetAsync_ReturnsHoldingsForConfiguredBib()
        {
            var bibId = RequireConfiguredBib();

            var response = await Papi.HoldingsGetAsync(bibId, TestContext.CancellationToken);

            Assert.AreEqual(0, response.Data.PAPIErrorCode);
            Assert.IsNotEmpty(response.Data.BibHoldingsGetRows);
        }
    }
}
