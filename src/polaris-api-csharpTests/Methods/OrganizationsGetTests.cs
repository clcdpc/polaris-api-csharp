namespace Clc.Polaris.Api.Tests
{
    [TestClass]
    public sealed class OrganizationsGetTests : IntegrationTestBase
    {
        [TestMethod]
        [ReadOnlyIntegrationTest]
        public async Task OrganizationsGetTest()
        {
            var response = await Papi.OrganizationsGetAsync(cancellationToken: TestContext.CancellationToken);
            Assert.HasCount(response.Data.PAPIErrorCode, response.Data.OrganizationsGetRows);
        }
    }
}
