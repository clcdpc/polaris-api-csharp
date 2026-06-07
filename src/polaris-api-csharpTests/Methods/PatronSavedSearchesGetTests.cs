using Microsoft.VisualStudio.TestTools.UnitTesting;

#pragma warning disable CS8602 // Dereference of a possibly null reference.

namespace Clc.Polaris.Api.Tests
{
    [TestClass]
    public sealed class PatronSavedSearchesGetTests : IntegrationTestBase
    {
        [TestMethod]
        [ReadOnlyIntegrationCategory]
        public async Task PatronSavedSearchesGetTest()
        {
            var response = await Papi.PatronSavedSearchesGetAsync(Settings.PatronBarcode, Settings.PatronPin);
            Assert.IsTrue(response.Data.PAPIErrorCode == 0);
        }
    }
}
