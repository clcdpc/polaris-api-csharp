using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Clc.Polaris.Api.Tests
{
    [TestClass]
    public sealed class OrganizationsGetTests : IntegrationTestBase
    {
        [TestMethod]
        [ReadOnlyIntegrationTest]
        public async Task OrganizationsGetTest()
        {
            var response = await Papi.OrganizationsGetAsync();
            Assert.IsTrue(response.Data.OrganizationsGetRows.Count() == response.Data.PAPIErrorCode);
        }
    }
}
