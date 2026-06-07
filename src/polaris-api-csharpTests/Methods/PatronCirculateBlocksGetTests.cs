using Microsoft.VisualStudio.TestTools.UnitTesting;

#pragma warning disable CS8602 // Dereference of a possibly null reference.

namespace Clc.Polaris.Api.Tests
{
    [TestClass]
    public sealed class PatronCirculateBlocksGetTests : IntegrationTestBase
    {
        [TestMethod()]
        [ReadOnlyIntegrationCategory]
        public async Task PatronCirculateBlocksGetTest()
        {
            var response = await Papi.PatronCirculateBlocksGetAsync(Settings.PatronBarcode, Settings.PatronPin);
            Assert.IsTrue(response.Data.PAPIErrorCode == 0);
        }
    }
}
