using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Clc.Polaris.Api.Tests
{
    [TestClass]
    public sealed class HoldRequestCancelTests : IntegrationTestBase
    {
        [TestMethod]
        [MutatingIntegrationTest]
        [DoNotParallelize]
        public async Task HoldRequestCancelTest()
        {
            var response = await Papi.HoldRequestCancelAsync(Settings.PatronBarcode, 1234, Settings.PatronPin, cancellationToken: TestContext.CancellationToken);
            Assert.AreEqual(-4201, response.Data.PAPIErrorCode);
        }
    }
}
