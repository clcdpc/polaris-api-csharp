namespace Clc.Polaris.Api.Tests
{
    [TestClass]
    public sealed class PatronTitleListCopyAllTitlesTests : IntegrationTestBase
    {
        [TestMethod]
        [MutatingIntegrationTest]
        [DoNotParallelize]
        public async Task PatronTitleListCopyAllTitlesTest()
        {
            var response = await Papi.PatronTitleListCopyAllTitlesAsync(Settings.PatronBarcode, 1234, 1234, Settings.PatronPin, TestContext.CancellationToken);
            Assert.AreEqual(-1, response.Data.PAPIErrorCode);
        }
    }
}
