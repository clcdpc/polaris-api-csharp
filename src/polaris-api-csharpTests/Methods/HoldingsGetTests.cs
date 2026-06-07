using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

#pragma warning disable CS8602 // Dereference of a possibly null reference.

namespace Clc.Polaris.Api.Tests
{
    [TestClass]
    public sealed class HoldingsGetTests : IntegrationTestBase
    {
        private const int BibId = 478907;

        [TestMethod]
        [ReadOnlyIntegrationCategory]
        public async Task HoldingsGetTest()
        {
            var response = await Papi.HoldingsGetAsync(BibId);
            Assert.IsTrue(response.Data.BibHoldingsGetRows.Any());
        }
    }
}
