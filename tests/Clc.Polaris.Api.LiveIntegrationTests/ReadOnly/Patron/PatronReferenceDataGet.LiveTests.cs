namespace Clc.Polaris.Api.LiveIntegrationTests
{
    [TestClass]
    public sealed class PatronReferenceDataGetTests : IntegrationTestBase
    {
        [TestMethod]
        [ReadOnlyLiveTest]
        public async Task PatronLanguagesGetAsync_ReturnsSuccessfulResponseAndRowsCollection()
        {
            var response = await Papi.PatronLanguagesGetAsync(cancellationToken: TestContext.CancellationToken);

            Assert.IsTrue(response.Response.IsSuccessStatusCode);
            Assert.IsNotNull(response.Data);
            Assert.AreEqual(0, response.Data.PAPIErrorCode);
            Assert.IsNotNull(response.Data.PatronLanguagesRows);
        }

        [TestMethod]
        [ReadOnlyLiveTest]
        public async Task PatronStatisticalClassesGetAsync_ReturnsSuccessfulResponseAndRowsCollection()
        {
            var response = await Papi.PatronStatisticalClassesGetAsync(cancellationToken: TestContext.CancellationToken);

            Assert.IsTrue(response.Response.IsSuccessStatusCode);
            Assert.IsNotNull(response.Data);
            Assert.AreEqual(0, response.Data.PAPIErrorCode);
            Assert.IsNotNull(response.Data.PatronStatisticalClassesRows);
        }

        [TestMethod]
        [ReadOnlyLiveTest]
        public async Task PatronUdfConfigsGetAsync_ReturnsSuccessfulResponseAndRowsCollection()
        {
            var response = await Papi.PatronUdfConfigsGetAsync(cancellationToken: TestContext.CancellationToken);

            Assert.IsTrue(response.Response.IsSuccessStatusCode);
            Assert.IsNotNull(response.Data);
            Assert.AreEqual(0, response.Data.PAPIErrorCode);
            Assert.IsNotNull(response.Data.PatronUdfConfigsRows);
        }

        [TestMethod]
        [ReadOnlyLiveTest]
        public async Task PickupAreasGetAsync_ReturnsSuccessfulResponseAndRowsCollection()
        {
            var response = await Papi.PickupAreasGetAsync(cancellationToken: TestContext.CancellationToken);

            Assert.IsTrue(response.Response.IsSuccessStatusCode);
            Assert.IsNotNull(response.Data);
            Assert.AreEqual(0, response.Data.PAPIErrorCode);
            Assert.IsNotNull(response.Data.PickupAreasRows);
        }
    }
}
