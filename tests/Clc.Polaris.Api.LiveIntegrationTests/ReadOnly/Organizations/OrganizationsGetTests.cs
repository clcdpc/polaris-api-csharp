namespace Clc.Polaris.Api.LiveIntegrationTests.ReadOnly.Organizations
{
    [TestClass]
    public sealed class OrganizationsGetTests : IntegrationTestBase
    {
        [TestMethod]
        [ReadOnlyLiveTest]
        public async Task OrganizationsGetTest()
        {
            var response = await Papi.OrganizationsGetAsync(cancellationToken: TestContext.CancellationToken);
            Assert.HasCount(response.Data.PAPIErrorCode, response.Data.OrganizationsGetRows);
        }
    }
}
