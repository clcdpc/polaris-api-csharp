namespace Clc.Polaris.Api.Tests
{
    [TestClass]
    public sealed class ConfiguredReadOnlyLookupTests : IntegrationTestBase
    {
        [TestMethod]
        [ReadOnlyIntegrationTest]
        public async Task ConfiguredBibGet_DefaultBranchReturnsConfiguredBib()
        {
            var bibId = RequireConfiguredBib();

            var response = await Papi.BibGetAsync(bibId, cancellationToken: TestContext.CancellationToken);

            Assert.AreEqual(0, response.Data.PAPIErrorCode);
            Assert.AreEqual(bibId, response.Data.ControlNumber);
        }

        [TestMethod]
        [ReadOnlyIntegrationTest]
        public async Task ConfiguredBibGet_ExplicitBranchReturnsConfiguredBib()
        {
            var bibId = RequireConfiguredBib();
            var branchId = RequireConfiguredBranch();

            var response = await Papi.BibGetAsync(bibId, branchId, TestContext.CancellationToken);

            Assert.AreEqual(0, response.Data.PAPIErrorCode);
            Assert.AreEqual(bibId, response.Data.ControlNumber);
        }

        [TestMethod]
        [ReadOnlyIntegrationTest]
        public async Task BibSearch_KeywordWithSpacesAndPagingReturnsRows()
        {
            var branchId = RequireConfiguredBranch();

            var response = await Papi.BibSearchAsync(new BibSearchOptions
            {
                Term = "the book",
                Page = 1,
                PageSize = 5,
                Branch = branchId,
            }, TestContext.CancellationToken);

            Assert.AreEqual(response.Data.BibSearchRows.Count, response.Data.PAPIErrorCode);
            Assert.IsNotEmpty(response.Data.BibSearchRows);
        }

        [TestMethod]
        [ProtectedReadOnlyIntegrationTest]
        public async Task ConfiguredSynchBibsById_ReturnsConfiguredBib()
        {
            IntegrationTestRequirements.RequireStaffOverrideAccount(PapiSettings);
            var bibId = RequireConfiguredBib();

            var response = await Papi.Synch_BibsByIdGetAsync([bibId], includeItems: true, cancellationToken: TestContext.CancellationToken);

            Assert.AreEqual(0, response.Data.PAPIErrorCode);
            Assert.IsNotNull(response.Data.GetBibsByIDRows.SingleOrDefault(row => row.BibliographicRecordID == bibId));
        }

        [TestMethod]
        [ReadOnlyIntegrationTest]
        public async Task ConfiguredHoldingsGet_ReturnsHoldingsForConfiguredBib()
        {
            var bibId = RequireConfiguredBib();

            var response = await Papi.HoldingsGetAsync(bibId, TestContext.CancellationToken);

            Assert.AreEqual(0, response.Data.PAPIErrorCode);
            Assert.IsNotEmpty(response.Data.BibHoldingsGetRows);
        }

        [TestMethod]
        [ReadOnlyIntegrationTest]
        public async Task ConfiguredPickupBranchesGet_ReturnsConfiguredPickupBranchWhenProvided()
        {
            var pickupBranchId = Settings.PickupBranchId ?? RequireConfiguredBranch();

            var response = await Papi.PickupBranchesGetAsync(7, TestContext.CancellationToken);

            Assert.AreEqual(0, response.Data.PAPIErrorCode);
            Assert.Contains(pickupBranchId, response.Data.PickupBranches);
        }

        [TestMethod]
        [ReadOnlyIntegrationTest]
        public async Task ConfiguredBranchLookupEndpoints_ReturnRows()
        {
            var branchId = RequireConfiguredBranch();

            var shelfLocations = await Papi.ShelfLocationsGetAsync(branchId, TestContext.CancellationToken);
            Assert.HasCount(shelfLocations.Data.PAPIErrorCode, shelfLocations.Data.ShelfLocationsRows);

            var materialTypes = await Papi.MaterialTypesGetAsync(branchId, TestContext.CancellationToken);
            Assert.HasCount(materialTypes.Data.PAPIErrorCode, materialTypes.Data.MaterialTypesRows);

            var limitFilters = await Papi.LimitFiltersGetAsync(branchId, TestContext.CancellationToken);
            Assert.HasCount(limitFilters.Data.PAPIErrorCode, limitFilters.Data.LimitFiltersRows);

            var collections = await Papi.CollectionsGetAsync(branchId, TestContext.CancellationToken);
            Assert.HasCount(collections.Data.PAPIErrorCode, collections.Data.CollectionsRows);
        }

        [TestMethod]
        [ReadOnlyIntegrationTest]
        public async Task ConfiguredDatesClosedGet_ReturnsRowsForConfiguredBranch()
        {
            var branchId = RequireConfiguredBranch();

            var response = await Papi.DatesClosedGetAsync(branchId, TestContext.CancellationToken);

            Assert.IsNotNull(response.Data);
            Assert.IsNotNull(response.Data.DatesClosedRows);
        }
    }
}
