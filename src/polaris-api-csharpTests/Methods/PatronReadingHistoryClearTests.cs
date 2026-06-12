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
            var response = await Papi.PatronReadingHistoryClearAsync(Settings.PatronBarcode, Settings.PatronPin, [1234], TestContext.CancellationToken);
            Assert.AreEqual(-10, response.Data.PAPIErrorCode);
        }
    }
}
