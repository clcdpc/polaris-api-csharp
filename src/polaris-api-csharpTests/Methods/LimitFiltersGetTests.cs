using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

#pragma warning disable CS8602 // Dereference of a possibly null reference.

namespace Clc.Polaris.Api.Tests
{
    [TestClass]
    public sealed class LimitFiltersGetTests : IntegrationTestBase
    {
        [TestMethod]
        [ReadOnlyIntegrationCategory]
        public async Task LimitFiltersGetTest()
        {
            var response = (await Papi.LimitFiltersGetAsync()).Data;
            Assert.IsTrue(response.LimitFiltersRows.Count() == response.PAPIErrorCode);
        }
    }
}
