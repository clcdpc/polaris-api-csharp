using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Clc.Polaris.Api.Tests
{
    [TestClass]
    public sealed class SA_GetValueByOrgTests : IntegrationTestBase
    {
        [TestMethod]
        [ProtectedReadOnlyIntegrationTest]
        public async Task SA_GetValueByOrgTest()
        {
            IntegrationTestRequirements.RequireStaffOverrideAccount(PapiSettings);

            var response = await Papi.SA_GetValueByOrgAsync("ORGEMAIL", cancellationToken: TestContext.CancellationToken);
            Assert.AreEqual(Settings.OrgEmail, response.Data.Value);
        }
    }
}
