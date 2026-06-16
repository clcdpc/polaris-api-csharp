namespace Clc.Polaris.Api.LiveIntegrationTests
{
    [TestClass]
    public sealed class SystemAdministrationReadOnlyLiveTests : IntegrationTestBase
    {
        [TestMethod]
        [ReadOnlyLiveTest]
        public async Task SortOptionsGetAsync_ReturnsResponse()
        {
            var response = await Papi.SortOptionsGetAsync(cancellationToken: TestContext.CancellationToken);

            Assert.IsNotNull(response.Data);
        }

        [TestMethod]
        [ReadOnlyLiveTest]
        public async Task SysHoldStatusesGetAsync_ReturnsResponse()
        {
            var response = await Papi.SysHoldStatusesGetAsync(cancellationToken: TestContext.CancellationToken);

            Assert.IsNotNull(response.Data);
        }

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
