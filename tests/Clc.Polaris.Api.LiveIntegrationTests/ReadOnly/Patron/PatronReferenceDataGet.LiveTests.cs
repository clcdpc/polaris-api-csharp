namespace Clc.Polaris.Api.LiveIntegrationTests
{
    [TestClass]
    public sealed class PatronReferenceDataGetTests : IntegrationTestBase
    {
        [TestMethod]
        [ReadOnlyLiveTest]
        public async Task PatronLanguagesGetAsync_ReturnsRows()
        {
            var response = await Papi.PatronLanguagesGetAsync(cancellationToken: TestContext.CancellationToken);

            Assert.AreEqual(0, response.Data.PAPIErrorCode);
            Assert.IsNotEmpty(response.Data.PatronLanguagesRows);
        }

        [TestMethod]
        [ReadOnlyLiveTest]
        public async Task PatronStatisticalClassesGetAsync_ReturnsRows()
        {
            var response = await Papi.PatronStatisticalClassesGetAsync(cancellationToken: TestContext.CancellationToken);

            Assert.AreEqual(0, response.Data.PAPIErrorCode);
            Assert.IsNotEmpty(response.Data.PatronStatisticalClassesRows);
        }

        [TestMethod]
        [ReadOnlyLiveTest]
        public async Task PatronUdfConfigsGetAsync_ReturnsRows()
        {
            var response = await Papi.PatronUdfConfigsGetAsync(cancellationToken: TestContext.CancellationToken);

            Assert.AreEqual(0, response.Data.PAPIErrorCode);
            Assert.IsNotEmpty(response.Data.PatronUdfConfigsRows);
        }

        [TestMethod]
        [ReadOnlyLiveTest]
        public async Task PickupAreasGetAsync_ReturnsRows()
        {
            var response = await Papi.PickupAreasGetAsync(cancellationToken: TestContext.CancellationToken);

            Assert.AreEqual(0, response.Data.PAPIErrorCode);
            Assert.IsNotEmpty(response.Data.PickupAreasRows);
        }
    }
}
