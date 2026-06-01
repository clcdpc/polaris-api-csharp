using Clc.Polaris.Api.Tests.Integration.Infrastructure;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Clc.Polaris.Api.Tests.Integration;

[TestClass]
[TestCategory("Integration")]
public sealed class LookupTablesIntegrationTests : PapiIntegrationTestBase
{
    [TestMethod]
    public async Task CollectionsGet_ReturnsRowsAndRowCountCode()
    {
        var response = await Papi.CollectionsGetAsync();
        var data = PapiAssert.RowCountMatchesPapiCode(response, PapiAssert.HasData(response).CollectionsRows.Count);
        Assert.IsTrue(data.CollectionsRows.Count > 0, "Expected configured Polaris site to expose collections.");
    }

    [TestMethod]
    public async Task DatesClosedGet_WithConfiguredOrganization_ReturnsDeserializedRows()
    {
        RequireOrganizationId();

        var response = await Papi.DatesClosedGetAsync(EffectiveOrganizationId);
        var data = PapiAssert.HasData(response);
        Assert.IsNotNull(data.DatesClosedRows, "Expected dates-closed rows collection to deserialize.");
    }

    [TestMethod]
    public async Task ItemStatusesGet_WithConfiguredBranch_ReturnsRowsAndRowCountCode()
    {
        RequireBranchId();

        var response = await Papi.ItemStatusesGetAsync(EffectiveBranchId);
        var data = PapiAssert.HasData(response);
        Assert.AreEqual(data.ItemStatusesRows.Length, data.PAPIErrorCode);
    }

    [TestMethod]
    public async Task LimitFiltersGet_ReturnsRowsAndRowCountCode()
    {
        var response = await Papi.LimitFiltersGetAsync();
        var data = PapiAssert.HasData(response);
        Assert.AreEqual(data.LimitFiltersRows.Length, data.PAPIErrorCode);
    }

    [TestMethod]
    public async Task MarcTypeOfMaterialsGet_ReturnsRowsAndRowCountCode()
    {
        var response = await Papi.MARCTypeOfMaterialsGetAsync();
        var data = PapiAssert.HasData(response);
        Assert.AreEqual(data.MARCTypeOfMaterialsRows.Length, data.PAPIErrorCode);
    }

    [TestMethod]
    public async Task MaterialTypesGet_ReturnsRowsAndRowCountCode()
    {
        var response = await Papi.MaterialTypesGetAsync();
        var data = PapiAssert.HasData(response);
        Assert.AreEqual(data.MaterialTypesRows.Length, data.PAPIErrorCode);
    }

    [TestMethod]
    public async Task OrganizationsGet_ReturnsRowsAndRowCountCode()
    {
        var response = await Papi.OrganizationsGetAsync();
        var data = PapiAssert.HasData(response);
        Assert.AreEqual(data.OrganizationsGetRows.Count, data.PAPIErrorCode);
    }

    [TestMethod]
    public async Task PatronCodesGet_ReturnsSuccessAndRows()
    {
        var response = await Papi.PatronCodesGetAsync();
        var data = PapiAssert.Success(response);
        Assert.IsTrue(data.PatronCodesRows.Count > 0, "Expected patron code rows.");
    }

    [TestMethod]
    public async Task PickupBranchesGet_ReturnsRows()
    {
        var response = await Papi.PickupBranchesGetAsync();
        var data = PapiAssert.HasData(response);
        Assert.IsTrue(data.PickupBranchesRows.Count > 0, "Expected pickup branch rows.");
    }

    [TestMethod]
    public async Task ShelfLocationsGet_WithConfiguredBranch_ReturnsRowsAndRowCountCode()
    {
        RequireBranchId();

        var response = await Papi.ShelfLocationsGetAsync(EffectiveBranchId);
        var data = PapiAssert.HasData(response);
        Assert.AreEqual(data.ShelfLocationsRows.Length, data.PAPIErrorCode);
    }
}
