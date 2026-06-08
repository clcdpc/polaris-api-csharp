using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Clc.Polaris.Api.Tests
{
    [TestClass]
    public sealed class PatronBasicDataGetTests : IntegrationTestBase
    {
        [TestMethod]
        [ReadOnlyIntegrationTest]
        public async Task PatronBasicDataGetTest()
        {
            var response = await Papi.PatronBasicDataGetAsync(Settings.PatronBarcode, Settings.PatronPin, true);
            Assert.IsTrue(response.Data.PAPIErrorCode == 0);
            Assert.IsTrue(response.Data.PatronBasicData.PatronID == Settings.PatronId);
            Assert.IsTrue(response.Data.PatronBasicData.PatronAddresses.Any());
        }
    }
}
