namespace Clc.Polaris.Api.Tests
{
    [TestClass]
    public sealed class PatronMessageDeleteTests : IntegrationTestBase
    {
        [TestMethod]
        [MutatingIntegrationTest]
        [DoNotParallelize]
        public async Task PatronMessageDeleteTest()
        {
            var response = await Papi.PatronMessageDeleteAsync(Settings.PatronBarcode, PatronMessageType.freetext, 1234, Settings.PatronPin, TestContext.CancellationToken);
            Assert.AreEqual(-1, response.Data.PAPIErrorCode);
        }
    }
}
