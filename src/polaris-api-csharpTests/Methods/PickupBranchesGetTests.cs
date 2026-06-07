using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

#pragma warning disable CS8602 // Dereference of a possibly null reference.

namespace Clc.Polaris.Api.Tests
{
    [TestClass]
    public sealed class PickupBranchesGetTests : IntegrationTestBase
    {
        [TestMethod]
        [ReadOnlyIntegrationCategory]
        public async Task PickupBranchesGetTest()
        {
            var response = await Papi.PickupBranchesGetAsync();
            Assert.IsTrue(response.Data.PickupBranchesRows.Any());
        }
    }
}
