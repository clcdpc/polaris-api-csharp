namespace Clc.Polaris.Api.LiveIntegrationTests
{
    [TestClass]
    public sealed class PatronSavedSearchesGetTests : IntegrationTestBase
    {
        [TestMethod]
        [ReadOnlyLiveTest]
        public async Task PatronSavedSearchesGetTest()
        {
            var response = await Papi.PatronSavedSearchesGetAsync(Settings.PatronBarcode, Settings.PatronPin, TestContext.CancellationToken);
            Assert.AreEqual(0, response.Data.PAPIErrorCode);
        }
    }
}
