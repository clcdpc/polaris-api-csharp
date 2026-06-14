namespace Clc.Polaris.Api.LiveIntegrationTests
{
    [TestClass]
    public sealed class SA_GetValueByOrgTests : IntegrationTestBase
    {
        [TestMethod]
        [StaffReadOnlyLiveTest]
        public async Task SA_GetValueByOrgTest()
        {
            IntegrationTestRequirements.RequireStaffOverrideAccount(PapiSettings);

            var response = await Papi.SA_GetValueByOrgAsync("ORGEMAIL", cancellationToken: TestContext.CancellationToken);
            Assert.AreEqual(Settings.OrgEmail, response.Data.Value);
        }
    }
}
