using Microsoft.VisualStudio.TestTools.UnitTesting;

#pragma warning disable CS8602 // Dereference of a possibly null reference.

namespace Clc.Polaris.Api.Tests
{
    [TestClass]
    public sealed class PatronReadingHistoryClearTests : IntegrationTestBase
    {
        [TestMethod]
        [MutatingIntegrationCategory]
        [DoNotParallelize]
        public async Task PatronReadingHistoryClearTest()
        {
            var response = await Papi.PatronReadingHistoryClearAsync(Settings.PatronBarcode, Settings.PatronPin, new[] { 1234 });
            Assert.IsTrue(response.Data.PAPIErrorCode == -10);
        }
    }
}
