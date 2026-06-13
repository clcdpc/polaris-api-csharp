namespace Clc.Polaris.Api.Tests
{
    [TestClass]
    public sealed class PatronPreferencesGetTests : IntegrationTestBase
    {
        [TestMethod]
        [ReadOnlyIntegrationTest]
        public async Task PatronPreferencesGetTest()
        {
            var response = await Papi.PatronPreferencesGetAsync(Settings.PatronBarcode, Settings.PatronPin, TestContext.CancellationToken);
            Assert.AreEqual(Settings.PatronId, response.Data.PatronPreferences.PatronID);
        }
    }
}
