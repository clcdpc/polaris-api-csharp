namespace Clc.Polaris.Api.LiveIntegrationTests
{
    [TestClass]
    public sealed class PatronRenewBlocksGetTests : IntegrationTestBase
    {
        [TestMethod]
        [StaffReadOnlyLiveTest]
        public async Task PatronRenewBlocksGetTest()
        {
            IntegrationTestRequirements.RequireStaffOverrideAccount(PapiSettings);

            var response = await Papi.PatronRenewBlocksGetAsync(Settings.PatronId, cancellationToken: TestContext.CancellationToken);
            Assert.AreEqual(0, response.Data.PAPIErrorCode);
        }
    }
}
