using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Clc.Polaris.Api.Tests
{
    [TestClass]
    public sealed class ItemRenewTests : IntegrationTestBase
    {
        [TestMethod]
        [MutatingIntegrationCategory]
        [DoNotParallelize]
        public async Task ItemRenewTest()
        {
            var response = await Papi.ItemRenewAsync(Settings.PatronBarcode, 1234, Settings.PatronPin);
            Assert.IsTrue(response.Data.PAPIErrorCode == -6001);
        }
    }
}
