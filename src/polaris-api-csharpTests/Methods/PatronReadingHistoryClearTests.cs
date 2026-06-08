using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Clc.Polaris.Api.Tests
{
    [TestClass]
    public sealed class PatronReadingHistoryClearTests : IntegrationTestBase
    {
        [TestMethod]
        [MutatingIntegrationTest]
        [DoNotParallelize]
        public async Task PatronReadingHistoryClearTest()
        {
            var response = await Papi.PatronReadingHistoryClearAsync(Settings.PatronBarcode, Settings.PatronPin, new[] { 1234 });
            Assert.IsTrue(response.Data.PAPIErrorCode == -10);
        }
    }
}
