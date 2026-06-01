using Clc.Polaris.Api.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Clc.Polaris.Api.Tests.Integration;

[TestClass]
[TestCategory("Integration")]
public sealed class PatronReadIntegrationTests : IntegrationTestBase
{
    [TestMethod]
    public async Task PatronValidateAsync_WithConfiguredPatron_ReturnsPatronId()
    {
        RequirePatronCredentials();
        RequirePatronId();

        var response = await Papi.PatronValidateAsync(Settings.PatronBarcode, Settings.PatronPin);
        var data = PapiIntegrationAssert.Success(response);

        Assert.AreEqual(Settings.PatronId, data.PatronID);
    }

    [TestMethod]
    public async Task Patron_GetBarcodeFromIdAsync_WithConfiguredPatron_ReturnsBarcode()
    {
        RequireStaffProtectedTestsEnabled();
        RequirePatronId();
        RequirePatronCredentials();

        var response = await Papi.Patron_GetBarcodeFromIdAsync(Settings.PatronId);
        var data = PapiIntegrationAssert.HasData(response);

        Assert.AreEqual(Settings.PatronBarcode, data.Barcode);
    }

    [TestMethod]
    public async Task PatronBasicDataGetAsync_WithConfiguredPatron_ReturnsPatronData()
    {
        RequirePatronCredentials();
        RequirePatronId();

        var response = await Papi.PatronBasicDataGetAsync(Settings.PatronBarcode, Settings.PatronPin, true);
        var data = PapiIntegrationAssert.Success(response);

        Assert.IsNotNull(data.PatronBasicData);
        Assert.AreEqual(Settings.PatronId, data.PatronBasicData.PatronID);
    }

    [TestMethod]
    public async Task PatronCirculateBlocksGetAsync_WithConfiguredPatron_ReturnsSuccessShape()
    {
        RequirePatronCredentials();

        var response = await Papi.PatronCirculateBlocksGetAsync(Settings.PatronBarcode, Settings.PatronPin);
        PapiIntegrationAssert.Success(response);
    }

    [TestMethod]
    public async Task PatronHoldRequestsGetAsync_WithConfiguredPatron_ReturnsSuccessShape()
    {
        RequirePatronCredentials();

        var response = await Papi.PatronHoldRequestsGetAsync(Settings.PatronBarcode, PatronHoldStatus.all, Settings.PatronPin);
        var data = PapiIntegrationAssert.Success(response);

        Assert.IsNotNull(data.PatronHoldRequestsGetRows);
    }

    [TestMethod]
    public async Task PatronILLRequestsGetAsync_WithConfiguredPatron_ReturnsSuccessShape()
    {
        RequirePatronCredentials();

        var response = await Papi.PatronILLRequestsGetAsync(Settings.PatronBarcode, password: Settings.PatronPin);
        var data = PapiIntegrationAssert.Success(response);

        Assert.IsNotNull(data.PatronILLRequestsGetRows);
    }

    [TestMethod]
    public async Task PatronItemsOutGetAsync_WithConfiguredPatron_ReturnsSuccessShape()
    {
        RequirePatronCredentials();

        var response = await Papi.PatronItemsOutGetAsync(Settings.PatronBarcode, password: Settings.PatronPin);
        var data = PapiIntegrationAssert.Success(response);

        Assert.IsNotNull(data.PatronItemsOutGetRows);
    }

    [TestMethod]
    public async Task PatronMessagesGetAsync_WithConfiguredPatron_ReturnsSuccessShape()
    {
        RequirePatronCredentials();

        var response = await Papi.PatronMessagesGetAsync(Settings.PatronBarcode, password: Settings.PatronPin);
        var data = PapiIntegrationAssert.Success(response);

        Assert.IsNotNull(data.PatronMessagesGetRows);
    }

    [TestMethod]
    public async Task PatronPreferencesGetAsync_WithConfiguredPatron_ReturnsPatronPreferences()
    {
        RequirePatronCredentials();
        RequirePatronId();

        var response = await Papi.PatronPreferencesGetAsync(Settings.PatronBarcode, Settings.PatronPin);
        var data = PapiIntegrationAssert.HasPapiData(response);

        Assert.IsNotNull(data.PatronPreferences);
        Assert.AreEqual(Settings.PatronId, data.PatronPreferences.PatronID);
    }

    [TestMethod]
    public async Task PatronReadingHistoryGetAsync_WithConfiguredPatron_ReturnsRowsAndRowCountCode()
    {
        RequirePatronCredentials();

        var response = await Papi.PatronReadingHistoryGetAsync(Settings.PatronBarcode, password: Settings.PatronPin);
        var data = PapiIntegrationAssert.HasPapiData(response);

        PapiIntegrationAssert.RowCountMatchesPapiCode(response, data.PatronReadingHistoryGetRows.Count());
    }

    [TestMethod]
    public async Task PatronRenewBlocksGetAsync_WithConfiguredPatron_ReturnsSuccessShape()
    {
        RequireStaffProtectedTestsEnabled();
        RequirePatronId();

        var response = await Papi.PatronRenewBlocksGetAsync(Settings.PatronId, BranchIdOrConfiguredOrganizationId);
        PapiIntegrationAssert.Success(response);
    }

    [TestMethod]
    public async Task PatronSavedSearchesGetAsync_WithConfiguredPatron_ReturnsSuccessShape()
    {
        RequirePatronCredentials();

        var response = await Papi.PatronSavedSearchesGetAsync(Settings.PatronBarcode, Settings.PatronPin);
        PapiIntegrationAssert.Success(response);
    }

    [TestMethod]
    public async Task PatronSearchAsync_WithConfiguredPatronId_ReturnsMatchingRows()
    {
        RequireStaffProtectedTestsEnabled();
        RequirePatronId();

        var response = await Papi.PatronSearchAsync($"PRID={Settings.PatronId}", orgId: OrganizationIdOrConfigured);
        var data = PapiIntegrationAssert.HasPapiData(response);

        PapiIntegrationAssert.RowCountMatchesPapiCode(response, data.PatronSearchRows.Count);
    }
}
