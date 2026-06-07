using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

#pragma warning disable CS8602 // Dereference of a possibly null reference.

namespace Clc.Polaris.Api.Tests
{
    [TestClass]
    public sealed class Synch_BibsByIdGetTests : IntegrationTestBase
    {
        private const int BibId = 478907;

        [TestMethod]
        [ProtectedReadOnlyIntegrationCategory]
        public async Task Synch_BibsByIdGetTest()
        {
            IntegrationTestRequirements.RequireStaffOverrideAccount(PapiSettings);

            var response = await Papi.Synch_BibsByIdGetAsync(BibId);
            Assert.IsTrue(response.Response.IsSuccessStatusCode);
        }
    }
}
