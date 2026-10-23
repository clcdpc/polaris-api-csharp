namespace Clc.Polaris.Api.LiveIntegrationTests.ReadOnly.Patron
{
    [TestClass]
    public sealed class PatronBasicDataGetTests : IntegrationTestBase
    {
        [TestMethod]
        [ReadOnlyLiveTest]
        public async Task PatronBasicDataGetTest()
        {
            var response = await Papi.PatronBasicDataGetAsync(Settings.PatronBarcode, Settings.PatronPin, true, cancellationToken: TestContext.CancellationToken);
            Assert.AreEqual(0, response.Data.PAPIErrorCode);
            Assert.AreEqual(Settings.PatronId, response.Data.PatronBasicData.PatronID);
            Assert.IsNotEmpty(response.Data.PatronBasicData.PatronAddresses);
        }
    }
}
