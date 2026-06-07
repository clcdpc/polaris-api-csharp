using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Clc.Polaris.Api.Tests
{
    [TestClass]
    public sealed class PatronTitleListMoveTitleTests : IntegrationTestBase
    {
        [TestMethod]
        [MutatingIntegrationCategory]
        [DoNotParallelize]
        public async Task PatronTitleListMoveTitleTest()
        {
            var response = await Papi.PatronTitleListMoveTitleAsync(Settings.PatronBarcode, 1234, 1234, 1234, Settings.PatronPin);
            Assert.IsTrue(response.Data.PAPIErrorCode == -1);
        }
    }
}
