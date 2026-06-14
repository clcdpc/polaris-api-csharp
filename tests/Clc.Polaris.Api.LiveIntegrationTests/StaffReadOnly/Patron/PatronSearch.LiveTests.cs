namespace Clc.Polaris.Api.LiveIntegrationTests
{
    [TestClass]
    public sealed class PatronSearchTests : IntegrationTestBase
    {
        [TestMethod]
        [StaffReadOnlyLiveTest]
        public async Task PatronSearchTest()
        {
            IntegrationTestRequirements.RequireStaffOverrideAccount(PapiSettings);

            var response = await Papi.PatronSearchAsync($"PRID={Settings.PatronId}", cancellationToken: TestContext.CancellationToken);
            Assert.HasCount(response.Data.PAPIErrorCode, response.Data.PatronSearchRows);
        }
    }
}
