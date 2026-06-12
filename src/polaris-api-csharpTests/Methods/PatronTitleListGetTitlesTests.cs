using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Clc.Polaris.Api.Tests
{
    [TestClass]
    public sealed class PatronTitleListGetTitlesTests : IntegrationTestBase
    {
        [TestMethod]
        [ReadOnlyIntegrationTest]
        public async Task PatronTitleListGetTitlesTest()
        {
            var response = await Papi.PatronTitleListGetTitlesAsync(Settings.PatronBarcode, 1234, password: Settings.PatronPin, cancellationToken: TestContext.CancellationToken);
            Assert.AreEqual(-1, response.Data.PAPIErrorCode);
        }
    }
}
