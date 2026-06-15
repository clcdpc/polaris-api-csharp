namespace Clc.Polaris.Api.LiveIntegrationTests
{
    [TestClass]
    public sealed class PatronStatisticalClassesGetTests : IntegrationTestBase
    {
        [TestMethod]
        [ReadOnlyLiveTest]
        public async Task PatronStatisticalClassesGetAsync_ReturnsRows()
        {
            var response = await Papi.PatronStatisticalClassesGetAsync(cancellationToken: TestContext.CancellationToken);

            Assert.AreEqual(0, response.Data.PAPIErrorCode);
            Assert.IsNotEmpty(response.Data.PatronStatisticalClassesRows);
        }
    }
}
