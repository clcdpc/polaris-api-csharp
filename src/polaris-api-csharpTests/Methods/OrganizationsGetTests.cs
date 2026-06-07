using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

#pragma warning disable CS8602 // Dereference of a possibly null reference.

namespace Clc.Polaris.Api.Tests
{
    [TestClass]
    public sealed class OrganizationsGetTests : IntegrationTestBase
    {
        [TestMethod]
        [ReadOnlyIntegrationCategory]
        public async Task OrganizationsGetTest()
        {
            var response = await Papi.OrganizationsGetAsync();
            Assert.IsTrue(response.Data.OrganizationsGetRows.Count() == response.Data.PAPIErrorCode);
        }
    }
}
