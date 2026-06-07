using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

#pragma warning disable CS8602 // Dereference of a possibly null reference.

namespace Clc.Polaris.Api.Tests
{
    [TestClass]
    public sealed class PatronAccountGetTests : IntegrationTestBase
    {
        [TestMethod]
        [ReadOnlyIntegrationCategory]
        public async Task PatronAccountGetTest()
        {
            var response = await Papi.PatronAccountGetAsync(Settings.PatronBarcode, Settings.PatronPin);
            Assert.IsTrue(response.Data.PAPIErrorCode == 0);
            Assert.IsTrue(response.Data.PatronAccountGetRows.Any());
        }
    }
}
