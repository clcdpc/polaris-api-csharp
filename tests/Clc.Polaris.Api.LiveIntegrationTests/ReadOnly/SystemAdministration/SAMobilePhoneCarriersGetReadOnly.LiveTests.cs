namespace Clc.Polaris.Api.LiveIntegrationTests
{
    [TestClass]
    public sealed class SAMobilePhoneCarriersGetReadOnlyLiveTests : IntegrationTestBase
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
