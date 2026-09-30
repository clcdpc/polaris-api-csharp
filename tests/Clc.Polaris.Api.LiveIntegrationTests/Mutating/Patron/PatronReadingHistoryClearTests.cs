namespace Clc.Polaris.Api.LiveIntegrationTests.Mutating.Patron
{
    [TestClass]
    public sealed class PatronReadingHistoryClearTests : IntegrationTestBase
    {
        [TestMethod]
        [MutatingLiveTest]
        [DoNotParallelize]
        public async Task PatronReadingHistoryClearTest()
        {
            var response = await Papi.PatronReadingHistoryClearAsync(Settings.PatronBarcode, Settings.PatronPin, [1234], TestContext.CancellationToken);
            Assert.AreEqual(-10, response.Data.PAPIErrorCode);
        }
    }
}
