using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Clc.Polaris.Api.Tests
{
    [TestClass]
    public sealed class SA_GetValueByOrgTests : IntegrationTestBase
    {
        [TestMethod]
        [ProtectedReadOnlyIntegrationCategory]
        public async Task SA_GetValueByOrgTest()
        {
            IntegrationTestRequirements.RequireStaffOverrideAccount(PapiSettings);

            var response = await Papi.SA_GetValueByOrgAsync("ORGEMAIL");
            Assert.IsTrue(response.Data.Value == Settings.OrgEmail);
        }
    }
}
