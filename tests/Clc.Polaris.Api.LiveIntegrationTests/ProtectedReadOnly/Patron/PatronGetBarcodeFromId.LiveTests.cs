namespace Clc.Polaris.Api.LiveIntegrationTests
{
    [TestClass]
    public sealed class Patron_GetBarcodeFromIdTests : IntegrationTestBase
    {
        [TestMethod]
        [ProtectedReadOnlyLiveTest]
        public async Task Patron_GetBarcodeFromIdTest()
        {
            IntegrationTestRequirements.RequireStaffOverrideAccount(PapiSettings);

            var response = await Papi.Patron_GetBarcodeFromIdAsync(Settings.PatronId, TestContext.CancellationToken);
            Assert.AreEqual(Settings.PatronBarcode, response.Data.Barcode);
        }
    }
}
