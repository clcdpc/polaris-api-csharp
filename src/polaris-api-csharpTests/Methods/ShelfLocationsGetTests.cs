using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

#pragma warning disable CS8602 // Dereference of a possibly null reference.

namespace Clc.Polaris.Api.Tests
{
    [TestClass]
    public sealed class ShelfLocationsGetTests : IntegrationTestBase
    {
        [TestMethod]
        [ReadOnlyIntegrationCategory]
        public async Task ShelfLocationsGetTest()
        {
            var response = await Papi.ShelfLocationsGetAsync(7);
            Assert.IsTrue(response.Data.PAPIErrorCode == response.Data.ShelfLocationsRows.Count());
        }
    }
}
