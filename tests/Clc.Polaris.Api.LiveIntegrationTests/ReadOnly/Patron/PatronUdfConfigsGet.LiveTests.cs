namespace Clc.Polaris.Api.LiveIntegrationTests
{
    [TestClass]
    public sealed class PatronUdfConfigsGetTests : IntegrationTestBase
    {
        [TestMethod]
        [ReadOnlyLiveTest]
        public async Task PatronUdfConfigsGetAsync_ReturnsRows()
        {
            var response = await Papi.PatronUdfConfigsGetAsync(cancellationToken: TestContext.CancellationToken);

            Assert.AreEqual(0, response.Data.PAPIErrorCode);
            Assert.IsNotEmpty(response.Data.PatronUdfConfigsRows);
        }
    }
}
