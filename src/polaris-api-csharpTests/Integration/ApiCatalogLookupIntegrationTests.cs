using Clc.Polaris.Api.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Clc.Polaris.Api;

[TestClass]
[TestCategory("Integration")]
public sealed class ApiCatalogLookupIntegrationTests : PapiIntegrationTestBase
{
    [TestMethod]
    public async Task ApiKeyValidate_returns_success_code()
    {
        PapiIntegrationAssert.Success(await Papi.ApiKeyValidateAsync());
    }

    [TestMethod]
    public async Task ApiVersionGet_returns_version_payload()
    {
        var data = PapiIntegrationAssert.Success(await Papi.ApiVersionGetAsync());
        Assert.IsFalse(string.IsNullOrWhiteSpace(data.ToString()));
    }

    [TestMethod]
    public async Task BibGet_configured_bib_returns_title_and_expected_route()
    {
        RequireBibScenario();

        var response = await Papi.BibGetAsync(Settings.BibId, Settings.BranchId);
        var data = PapiIntegrationAssert.Success(response);
        Assert.IsFalse(string.IsNullOrWhiteSpace(data.Title));
        PapiIntegrationAssert.RequestUriContains(response, $"100/{Settings.BranchId}/bib");
    }

    [TestMethod]
    public async Task BibGet_nonexistent_bib_returns_documented_error_shape()
    {
        var response = await Papi.BibGetAsync(NonexistentId, RequireOrganizationId());
        PapiIntegrationAssert.PapiError(response, -1);
    }

    [TestMethod]
    public async Task BibSearch_keyword_returns_deserialized_rows()
    {
        var data = PapiIntegrationAssert.HasData(await Papi.BibSearchAsync(new BibSearchOptions { Term = "dogs", PageSize = 10 }));
        Assert.IsTrue(data.PAPIErrorCode >= 0, data.ErrorMessage?.ToString());
        Assert.IsNotNull(data.BibSearchRows);
    }

    [TestMethod]
    public async Task CollectionsGet_returns_row_count_as_papi_code()
    {
        var response = await Papi.CollectionsGetAsync();
        var data = PapiIntegrationAssert.HasData(response);
        PapiIntegrationAssert.RowCountEqualsPapiCode(response, data.CollectionsRows.Count);
    }

    [TestMethod]
    public async Task DatesClosedGet_configured_org_returns_deserialized_rows()
    {
        var data = PapiIntegrationAssert.HasData(await Papi.DatesClosedGetAsync(RequireOrganizationId()));
        Assert.IsNotNull(data.DatesClosedRows);
    }

    [TestMethod]
    public void HeadingsSearch_is_not_implemented_by_client()
    {
        _ = Papi;
        Assert.ThrowsException<NotImplementedException>(() => Papi.HeadingsSearchAsync(NonexistentId));
    }

    [TestMethod]
    public async Task HoldingsGet_configured_bib_returns_holdings_rows()
    {
        RequireBibScenario();

        var data = PapiIntegrationAssert.Success(await Papi.HoldingsGetAsync(Settings.BibId));
        Assert.IsNotNull(data.BibHoldingsGetRows);
    }

    [TestMethod]
    public async Task LimitFiltersGet_returns_row_count_as_papi_code()
    {
        var response = await Papi.LimitFiltersGetAsync();
        var data = PapiIntegrationAssert.HasData(response);
        PapiIntegrationAssert.RowCountEqualsPapiCode(response, data.LimitFiltersRows.Count());
    }

    [TestMethod]
    public async Task MarcMaterialAndItemStatus_lookups_return_row_count_as_papi_code()
    {
        var branchId = RequireBranchId();

        var marc = await Papi.MARCTypeOfMaterialsGetAsync();
        PapiIntegrationAssert.RowCountEqualsPapiCode(marc, PapiIntegrationAssert.HasData(marc).MARCTypeOfMaterialsRows.Count());

        var materialTypes = await Papi.MaterialTypesGetAsync(branchId);
        PapiIntegrationAssert.RowCountEqualsPapiCode(materialTypes, PapiIntegrationAssert.HasData(materialTypes).MaterialTypesRows.Length);

        var itemStatuses = await Papi.ItemStatusesGetAsync(branchId);
        PapiIntegrationAssert.RowCountEqualsPapiCode(itemStatuses, PapiIntegrationAssert.HasData(itemStatuses).ItemStatusesRows.Count());
    }

    [TestMethod]
    public async Task OrganizationsPickupBranchesAndShelves_return_deserialized_rows()
    {
        var branchId = RequireBranchId();

        var organizations = await Papi.OrganizationsGetAsync();
        PapiIntegrationAssert.RowCountEqualsPapiCode(organizations, PapiIntegrationAssert.HasData(organizations).OrganizationsGetRows.Count());

        var pickupBranches = PapiIntegrationAssert.HasData(await Papi.PickupBranchesGetAsync(RequireOrganizationId()));
        Assert.IsNotNull(pickupBranches.PickupBranchesRows);

        var shelves = await Papi.ShelfLocationsGetAsync(branchId);
        PapiIntegrationAssert.RowCountEqualsPapiCode(shelves, PapiIntegrationAssert.HasData(shelves).ShelfLocationsRows.Count());
    }

    [TestMethod]
    public async Task SynchBibsById_configured_bib_returns_deserialized_payload()
    {
        RequireBibScenario();

        var data = PapiIntegrationAssert.HasData(await Papi.Synch_BibsByIdGetAsync(Settings.BibId));
        Assert.IsTrue(data.PAPIErrorCode >= 0, data.ErrorMessage?.ToString());
    }
}
