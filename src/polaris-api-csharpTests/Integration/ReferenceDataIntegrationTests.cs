using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Clc.Polaris.Api.Tests.Integration;

[TestClass]
[TestCategory("Integration")]
public sealed class ReferenceDataIntegrationTests : PapiIntegrationTestBase
{
    [TestMethod]
    public async Task CollectionsGet_ReturnsRowCountInPapiErrorCode()
    {
        var response = await Client.CollectionsGetAsync();
        var data = PapiIntegrationAssert.HasData(response);
        PapiIntegrationAssert.PapiCodeEqualsRowCount(data.PAPIErrorCode, data.CollectionsRows.Count, nameof(data.CollectionsRows));
    }

    [TestMethod]
    public async Task DatesClosedGet_WithConfiguredOrganization_ReturnsDeserializedRows()
    {
        var response = await Client.DatesClosedGetAsync(Settings.EffectiveOrganizationId);
        var data = PapiIntegrationAssert.HasData(response);
        Assert.IsNotNull(data.DatesClosedRows);
    }

    [TestMethod]
    public async Task ItemStatusesGet_ReturnsRowCountInPapiErrorCode()
    {
        var response = await Client.ItemStatusesGetAsync(Settings.BranchId > 0 ? Settings.BranchId : null);
        var data = PapiIntegrationAssert.HasData(response);
        PapiIntegrationAssert.PapiCodeEqualsRowCount(data.PAPIErrorCode, data.ItemStatusesRows.Length, nameof(data.ItemStatusesRows));
    }

    [TestMethod]
    public async Task LimitFiltersGet_ReturnsRowCountInPapiErrorCode()
    {
        var response = await Client.LimitFiltersGetAsync(Settings.BranchId > 0 ? Settings.BranchId : null);
        var data = PapiIntegrationAssert.HasData(response);
        PapiIntegrationAssert.PapiCodeEqualsRowCount(data.PAPIErrorCode, data.LimitFiltersRows.Length, nameof(data.LimitFiltersRows));
    }

    [TestMethod]
    public async Task MarcTypeOfMaterialsGet_ReturnsRowCountInPapiErrorCode()
    {
        var response = await Client.MARCTypeOfMaterialsGetAsync(Settings.BranchId > 0 ? Settings.BranchId : null);
        var data = PapiIntegrationAssert.HasData(response);
        PapiIntegrationAssert.PapiCodeEqualsRowCount(data.PAPIErrorCode, data.MARCTypeOfMaterialsRows.Length, nameof(data.MARCTypeOfMaterialsRows));
    }

    [TestMethod]
    public async Task MaterialTypesGet_ReturnsDeserializedRows()
    {
        var response = await Client.MaterialTypesGetAsync(Settings.BranchId > 0 ? Settings.BranchId : null);
        var data = PapiIntegrationAssert.HasData(response);
        Assert.IsTrue(data.PAPIErrorCode >= 0, $"Expected non-negative row count, but received {data.PAPIErrorCode}: {data.ErrorMessage}");
    }

    [TestMethod]
    public async Task OrganizationsGet_ReturnsRowCountInPapiErrorCode()
    {
        var response = await Client.OrganizationsGetAsync();
        var data = PapiIntegrationAssert.HasData(response);
        PapiIntegrationAssert.PapiCodeEqualsRowCount(data.PAPIErrorCode, data.OrganizationsGetRows.Count, nameof(data.OrganizationsGetRows));
    }

    [TestMethod]
    public async Task PatronCodesGet_ReturnsRows()
    {
        var response = await Client.PatronCodesGetAsync(Settings.BranchId > 0 ? Settings.BranchId : null);
        var data = PapiIntegrationAssert.Success(response);
        Assert.IsNotNull(data.PatronCodesRows);
    }

    [TestMethod]
    public async Task PickupBranchesGet_ReturnsRows()
    {
        var response = await Client.PickupBranchesGetAsync(Settings.OrganizationId > 0 ? Settings.OrganizationId : null);
        var data = PapiIntegrationAssert.HasData(response);
        Assert.IsTrue(data.PickupBranchesRows.Count > 0 || data.PAPIErrorCode == 0);
    }

    [TestMethod]
    public async Task ShelfLocationsGet_ReturnsRowCountInPapiErrorCode()
    {
        var response = await Client.ShelfLocationsGetAsync(Settings.BranchId > 0 ? Settings.BranchId : null);
        var data = PapiIntegrationAssert.HasData(response);
        PapiIntegrationAssert.PapiCodeEqualsRowCount(data.PAPIErrorCode, data.ShelfLocationsRows.Length, nameof(data.ShelfLocationsRows));
    }
}
