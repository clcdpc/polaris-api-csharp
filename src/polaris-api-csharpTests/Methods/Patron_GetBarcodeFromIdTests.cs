using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Clc.Polaris.Api.Tests
{
    [TestClass]
    public sealed class Patron_GetBarcodeFromIdTests : IntegrationTestBase
    {
        [TestMethod]
        [ProtectedReadOnlyIntegrationTest]
        public async Task Patron_GetBarcodeFromIdTest()
        {
            IntegrationTestRequirements.RequireStaffOverrideAccount(PapiSettings);

            var response = await Papi.Patron_GetBarcodeFromIdAsync(Settings.PatronId, TestContext.CancellationToken);
            Assert.AreEqual(Settings.PatronBarcode, response.Data.Barcode);
        }
    }
}
