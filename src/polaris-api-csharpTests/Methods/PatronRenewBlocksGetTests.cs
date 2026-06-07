using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Clc.Polaris.Api.Tests
{
    [TestClass]
    public sealed class PatronRenewBlocksGetTests : IntegrationTestBase
    {
        [TestMethod]
        [ProtectedReadOnlyIntegrationCategory]
        public async Task PatronRenewBlocksGetTest()
        {
            IntegrationTestRequirements.RequireStaffOverrideAccount(PapiSettings);

            var response = await Papi.PatronRenewBlocksGetAsync(Settings.PatronId);
            Assert.IsTrue(response.Data.PAPIErrorCode == 0);
        }
    }
}
