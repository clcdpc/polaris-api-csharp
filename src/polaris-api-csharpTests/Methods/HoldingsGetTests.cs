using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Clc.Polaris.Api.Tests
{
    [TestClass]
    public sealed class HoldingsGetTests : IntegrationTestBase
    {
        [TestMethod]
        [ReadOnlyIntegrationCategory]
        public async Task HoldingsGetTest()
        {
            var response = await Papi.HoldingsGetAsync(478907);
            Assert.IsTrue(response.Data.BibHoldingsGetRows.Any());
        }
    }
}
