using Microsoft.VisualStudio.TestTools.UnitTesting;

#pragma warning disable CS8602 // Dereference of a possibly null reference.

namespace Clc.Polaris.Api.Tests
{
    [TestClass]
    public sealed class Patron_GetBarcodeFromIdTests : IntegrationTestBase
    {
        [TestMethod()]
        [ProtectedReadOnlyIntegrationCategory]
        public async Task Patron_GetBarcodeFromIdTest()
        {
            IntegrationTestRequirements.RequireStaffOverrideAccount(PapiSettings);

            var response = await Papi.Patron_GetBarcodeFromIdAsync(Settings.PatronId);
            Assert.IsTrue(response.Data.Barcode == Settings.PatronBarcode);
        }
    }
}
