namespace Clc.Polaris.Api.LiveIntegrationTests.ProtectedReadOnly.SystemAdministration
{
    [TestClass]
    public sealed class SAGetValueByOrgTests : IntegrationTestBase
    {
        [TestMethod]
        [ProtectedReadOnlyLiveTest]
        public async Task SA_GetValueByOrgTest()
        {
            IntegrationTestRequirements.RequireStaffOverrideAccount(PapiSettings);

            var response = await Papi.SA_GetValueByOrgAsync("ORGEMAIL", cancellationToken: TestContext.CancellationToken);
            Assert.AreEqual(Settings.OrgEmail, response.Data.Value);
        }
    }
}
