namespace Clc.Polaris.Api.Tests
{
    [TestClass]
    public sealed class PickupBranchesGetTests : IntegrationTestBase
    {
        [TestMethod]
        [ReadOnlyIntegrationTest]
        public async Task PickupBranchesGetTest()
        {
            var response = await Papi.PickupBranchesGetAsync(cancellationToken: TestContext.CancellationToken);
            Assert.IsNotEmpty(response.Data.PickupBranchesRows);
        }
    }
}
