namespace Clc.Polaris.Api.LiveIntegrationTests
{
    [TestClass]
    public sealed class PatronPreferencesGetTests : IntegrationTestBase
    {
        [TestMethod]
        [ReadOnlyLiveTest]
        public async Task PatronPreferencesGetTest()
        {
            var response = await Papi.PatronPreferencesGetAsync(Settings.PatronBarcode, Settings.PatronPin, TestContext.CancellationToken);
            Assert.AreEqual(Settings.PatronId, response.Data.PatronPreferences.PatronID);
        }
    }
}
