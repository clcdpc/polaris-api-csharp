using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Clc.Polaris.Api.Tests
{
    [TestClass]
    public sealed class PatronTitleListCopyTitleTests : IntegrationTestBase
    {
        [TestMethod]
        [MutatingIntegrationCategory]
        [DoNotParallelize]
        public async Task PatronTitleListCopyTitleTest()
        {
            var response = await Papi.PatronTitleListCopyTitleAsync(Settings.PatronBarcode, 1234, 1234, 1234, Settings.PatronPin);
            Assert.IsTrue(response.Data.PAPIErrorCode == -1);
        }
    }
}
