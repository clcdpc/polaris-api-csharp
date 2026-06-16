namespace Clc.Polaris.Api.LiveIntegrationTests.ReadOnly.Organizations
{
    [TestClass]
    public sealed class PickupBranchesGetTests : IntegrationTestBase
    {
        [TestMethod]
        [ReadOnlyLiveTest]
        public async Task PickupBranchesGetTest()
        {
            var response = await Papi.PickupBranchesGetAsync(cancellationToken: TestContext.CancellationToken);
            Assert.IsNotEmpty(response.Data.PickupBranchesRows);
        }

        [TestMethod]
        [ReadOnlyLiveTest]
        public async Task PickupBranchesGetAsync_ReturnsConfiguredPickupBranchWhenProvided()
        {
            var branchId = RequireConfiguredBranch();
            var expectedPickupBranchId = Settings.PickupBranchId ?? branchId;

            var response = await Papi.PickupBranchesGetAsync(branchId, TestContext.CancellationToken);

            Assert.AreEqual(0, response.Data.PAPIErrorCode);
            Assert.Contains(expectedPickupBranchId, response.Data.PickupBranches);
        }
    }
}
