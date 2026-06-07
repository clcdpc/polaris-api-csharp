using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

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
