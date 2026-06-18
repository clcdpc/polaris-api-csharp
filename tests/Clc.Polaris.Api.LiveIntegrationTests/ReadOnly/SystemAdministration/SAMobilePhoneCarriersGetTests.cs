namespace Clc.Polaris.Api.LiveIntegrationTests.ReadOnly.SystemAdministration
{
    [TestClass]
    public sealed class SAMobilePhoneCarriersGetTests : IntegrationTestBase
    {
        [TestMethod]
        [ReadOnlyLiveTest]
        public async Task SAMobilePhoneCarriersGetAsync_ReturnsResponse()
        {
            IntegrationTestRequirements.RequireStaffOverrideAccount(PapiSettings);

            var response = await Papi.SAMobilePhoneCarriersGetAsync(cancellationToken: TestContext.CancellationToken);

            Assert.IsNotNull(response.Data);
        }
    }
}
