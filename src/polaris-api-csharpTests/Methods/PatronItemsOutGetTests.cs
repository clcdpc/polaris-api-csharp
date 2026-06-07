using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

#pragma warning disable CS8602 // Dereference of a possibly null reference.

namespace Clc.Polaris.Api.Tests
{
    [TestClass]
    public sealed class PatronItemsOutGetTests : IntegrationTestBase
    {
        [TestMethod]
        [ReadOnlyIntegrationCategory]
        public async Task PatronItemsOutGetTest()
        {
            var response = await Papi.PatronItemsOutGetAsync(Settings.PatronBarcode, password: Settings.PatronPin);
            Assert.IsTrue(response.Data.PAPIErrorCode == 0);
            //Assert.IsTrue(response.Data.PatronItemsOutGetRows.Any());
        }
    }
}
