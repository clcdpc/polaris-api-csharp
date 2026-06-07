using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

#pragma warning disable CS8602 // Dereference of a possibly null reference.

namespace Clc.Polaris.Api.Tests
{
    [TestClass]
    public sealed class DatesClosedGetTests : IntegrationTestBase
    {
        [TestMethod]
        [ReadOnlyIntegrationCategory]
        public async Task DatesClosedGetTest()
        {
            var response = await Papi.DatesClosedGetAsync(7);
            Assert.IsTrue(response.Data.DatesClosedRows.Any());
        }
    }
}
