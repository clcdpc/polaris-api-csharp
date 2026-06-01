using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Clc.Polaris.Api.Tests.Integration;

[TestClass]
[TestCategory("Integration")]
public sealed class CollectionsAndLookupIntegrationTests : IntegrationTestBase
{
    [TestMethod]
    public async Task CollectionsGetAsync_ReturnsRowsAndRowCountCode()
    {
        RequirePapiConfiguration();

        var response = await Papi.CollectionsGetAsync(OrganizationIdOrConfigured);
        var data = PapiIntegrationAssert.HasPapiData(response);

        PapiIntegrationAssert.RowCountMatchesPapiCode(response, data.CollectionsRows.Count);
    }

    [TestMethod]
    public async Task DatesClosedGetAsync_WithConfiguredOrganization_ReturnsDeserializedRows()
    {
        RequirePapiConfiguration();

        var response = await Papi.DatesClosedGetAsync(OrganizationIdOrConfigured);
        var data = PapiIntegrationAssert.HasData(response);

        Assert.IsNotNull(data.DatesClosedRows);
    }

    [TestMethod]
    public async Task ItemStatusesGetAsync_ReturnsRowsAndRowCountCode()
    {
        RequirePapiConfiguration();

        var response = await Papi.ItemStatusesGetAsync(BranchIdOrConfiguredOrganizationId);
        var data = PapiIntegrationAssert.HasPapiData(response);

        PapiIntegrationAssert.RowCountMatchesPapiCode(response, data.ItemStatusesRows.Count());
    }

    [TestMethod]
    public async Task LimitFiltersGetAsync_ReturnsRowsAndRowCountCode()
    {
        RequirePapiConfiguration();

        var response = await Papi.LimitFiltersGetAsync(BranchIdOrConfiguredOrganizationId);
        var data = PapiIntegrationAssert.HasPapiData(response);

        PapiIntegrationAssert.RowCountMatchesPapiCode(response, data.LimitFiltersRows.Count());
    }

    [TestMethod]
    public async Task MARCTypeOfMaterialsGetAsync_ReturnsRowsAndRowCountCode()
    {
        RequirePapiConfiguration();

        var response = await Papi.MARCTypeOfMaterialsGetAsync(BranchIdOrConfiguredOrganizationId);
        var data = PapiIntegrationAssert.HasPapiData(response);

        PapiIntegrationAssert.RowCountMatchesPapiCode(response, data.MARCTypeOfMaterialsRows.Count());
    }

    [TestMethod]
    public async Task MaterialTypesGetAsync_ReturnsRowsAndRowCountCode()
    {
        RequirePapiConfiguration();

        var response = await Papi.MaterialTypesGetAsync(BranchIdOrConfiguredOrganizationId);
        var data = PapiIntegrationAssert.HasPapiData(response);

        PapiIntegrationAssert.RowCountMatchesPapiCode(response, data.MaterialTypesRows.Count());
    }

    [TestMethod]
    public async Task OrganizationsGetAsync_ReturnsRowsAndRowCountCode()
    {
        RequirePapiConfiguration();

        var response = await Papi.OrganizationsGetAsync();
        var data = PapiIntegrationAssert.HasPapiData(response);

        PapiIntegrationAssert.RowCountMatchesPapiCode(response, data.OrganizationsGetRows.Count);
    }

    [TestMethod]
    public async Task PickupBranchesGetAsync_ReturnsDeserializedPickupBranches()
    {
        RequirePapiConfiguration();

        var response = await Papi.PickupBranchesGetAsync(OrganizationIdOrConfigured);
        var data = PapiIntegrationAssert.HasPapiData(response);

        Assert.IsTrue(data.PAPIErrorCode >= 0, data.ErrorMessage);
        Assert.IsNotNull(data.PickupBranchesRows);
    }

    [TestMethod]
    public async Task ShelfLocationsGetAsync_ReturnsRowsAndRowCountCode()
    {
        RequirePapiConfiguration();

        var response = await Papi.ShelfLocationsGetAsync(BranchIdOrConfiguredOrganizationId);
        var data = PapiIntegrationAssert.HasPapiData(response);

        PapiIntegrationAssert.RowCountMatchesPapiCode(response, data.ShelfLocationsRows.Count());
    }
}
