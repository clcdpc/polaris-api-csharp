namespace Clc.Polaris.Api.Tests
{
    [TestClass]
    public sealed class PatronTitleListAddTitleTests : IntegrationTestBase
    {
        [TestMethod]
        [MutatingIntegrationTest]
        [DoNotParallelize]
        public async Task PatronTitleListAddTitleTest()
        {
            var response = await Papi.PatronTitleListAddTitleAsync(Settings.PatronBarcode, 1234, 1234, Settings.PatronPin, TestContext.CancellationToken);
            Assert.AreEqual(-1, response.Data.PAPIErrorCode);
        }
    }
}
