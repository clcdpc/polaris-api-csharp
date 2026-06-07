using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

#pragma warning disable CS8602 // Dereference of a possibly null reference.

namespace Clc.Polaris.Api.Tests
{
    [TestClass]
    public sealed class HoldingsGetTests : IntegrationTestBase
    {
        [TestMethod()]
        [ReadOnlyIntegrationCategory]
        public async Task HoldingsGetTest()
        {
            var response = await Papi.HoldingsGetAsync(478907);
            Assert.IsTrue(response.Data.BibHoldingsGetRows.Any());
        }
    }
}
