namespace Clc.Polaris.Api.LiveIntegrationTests
{
    [TestClass]
    public sealed class PatronMessageDeleteTests : IntegrationTestBase
    {
        [TestMethod]
        [MutatingLiveTest]
        [DoNotParallelize]
        public async Task PatronMessageDeleteTest()
        {
            var response = await Papi.PatronMessageDeleteAsync(Settings.PatronBarcode, PatronMessageType.freetext, 1234, Settings.PatronPin, TestContext.CancellationToken);
            Assert.AreEqual(-1, response.Data.PAPIErrorCode);
        }
    }
}
