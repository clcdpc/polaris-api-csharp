using Microsoft.VisualStudio.TestTools.UnitTesting;

#pragma warning disable CS8602 // Dereference of a possibly null reference.

namespace Clc.Polaris.Api.Tests
{
    [TestClass]
    public sealed class PatronPreferencesGetTests : IntegrationTestBase
    {
        [TestMethod]
        [ReadOnlyIntegrationCategory]
        public async Task PatronPreferencesGetTest()
        {
            var response = await Papi.PatronPreferencesGetAsync(Settings.PatronBarcode, Settings.PatronPin);
            Assert.IsTrue(response.Data.PatronPreferences.PatronID == Settings.PatronId);
        }
    }
}
