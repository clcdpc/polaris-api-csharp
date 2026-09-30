namespace Clc.Polaris.Api.LiveIntegrationTests.Mutating.Notifications
{
    [TestClass]
    public sealed class PatronMessageUpdateStatusTests : IntegrationTestBase
    {
        [TestMethod]
        [MutatingLiveTest]
        [DoNotParallelize]
        public async Task PatronMessageUpdateStatusTest()
        {
            var response = await Papi.PatronMessageUpdateStatusAsync(Settings.PatronBarcode, PatronMessageType.freetext, 1234, Settings.PatronPin, TestContext.CancellationToken);
            Assert.AreEqual(-1, response.Data.PAPIErrorCode);
        }
    }
}
