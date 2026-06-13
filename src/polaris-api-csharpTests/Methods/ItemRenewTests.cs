namespace Clc.Polaris.Api.Tests
{
    [TestClass]
    public sealed class ItemRenewTests : IntegrationTestBase
    {
        [TestMethod]
        [MutatingIntegrationTest]
        [DoNotParallelize]
        public async Task ItemRenewTest()
        {
            var response = await Papi.ItemRenewAsync(Settings.PatronBarcode, 1234, Settings.PatronPin, cancellationToken: TestContext.CancellationToken);
            Assert.AreEqual(-6001, response.Data.PAPIErrorCode);
        }
    }
}
