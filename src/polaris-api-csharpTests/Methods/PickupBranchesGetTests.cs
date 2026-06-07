using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

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
