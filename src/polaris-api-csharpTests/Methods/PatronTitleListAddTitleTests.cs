using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

#pragma warning disable CS8602 // Dereference of a possibly null reference.

namespace Clc.Polaris.Api.Tests
{
    [TestClass]
    public sealed class PatronTitleListAddTitleTests : IntegrationTestBase
    {
        [TestMethod]
        [MutatingIntegrationCategory]
        [DoNotParallelize]
        public async Task PatronTitleListAddTitleTest()
        {
            var response = await Papi.PatronTitleListAddTitleAsync(Settings.PatronBarcode, 1234, 1234, Settings.PatronPin);
            Assert.IsTrue(response.Data.PAPIErrorCode == -1);
        }
    }
}
