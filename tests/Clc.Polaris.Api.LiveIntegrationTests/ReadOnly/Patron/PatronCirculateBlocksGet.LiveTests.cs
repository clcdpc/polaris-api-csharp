namespace Clc.Polaris.Api.LiveIntegrationTests
{
    [TestClass]
    public sealed class PatronCirculateBlocksGetTests : IntegrationTestBase
    {
        [TestMethod]
        [ReadOnlyLiveTest]
        public async Task PatronCirculateBlocksGetTest()
        {
            var response = await Papi.PatronCirculateBlocksGetAsync(Settings.PatronBarcode, Settings.PatronPin, TestContext.CancellationToken);
            Assert.AreEqual(0, response.Data.PAPIErrorCode);
        }
    }
}
