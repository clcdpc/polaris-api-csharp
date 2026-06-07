using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

#pragma warning disable CS8602 // Dereference of a possibly null reference.

namespace Clc.Polaris.Api.Tests
{
    [TestClass]
    public sealed class PatronBasicDataGetTests : IntegrationTestBase
    {
        [TestMethod]
        [ReadOnlyIntegrationCategory]
        public async Task PatronBasicDataGetTest()
        {
            var response = await Papi.PatronBasicDataGetAsync(Settings.PatronBarcode, Settings.PatronPin, true);
            Assert.IsTrue(response.Data.PAPIErrorCode == 0);
            Assert.IsTrue(response.Data.PatronBasicData.PatronID == Settings.PatronId);
            Assert.IsTrue(response.Data.PatronBasicData.PatronAddresses.Any());
        }
    }
}
