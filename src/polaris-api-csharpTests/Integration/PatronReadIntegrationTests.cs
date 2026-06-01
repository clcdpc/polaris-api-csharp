using Clc.Polaris.Api.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Clc.Polaris.Api.Tests.Integration;

[TestClass]
[TestCategory("Integration")]
public sealed class PatronReadIntegrationTests : PapiIntegrationTestBase
{
    [TestMethod]
    public async Task PatronValidate_WithConfiguredPatron_ReturnsPatronId()
    {
        RequirePatronCredentials();
        RequirePatronId();

        var response = await Client.PatronValidateAsync(Settings.PatronBarcode, Settings.PatronPin);
        var data = PapiIntegrationAssert.Success(response);
        Assert.AreEqual(Settings.PatronId, data.PatronID);
    }

    [TestMethod]
    public async Task PatronGetBarcodeFromId_WithConfiguredPatron_ReturnsBarcode()
    {
        RequirePatronId();
        RequireScenarioValueForBarcode();

        var response = await Client.Patron_GetBarcodeFromIdAsync(Settings.PatronId);
        var data = PapiIntegrationAssert.HasData(response);
        Assert.AreEqual(Settings.PatronBarcode, data.Barcode);
    }

    [TestMethod]
    public async Task PatronBasicDataGet_WithConfiguredPatron_ReturnsSuccess()
    {
        RequirePatronCredentials();

        var response = await Client.PatronBasicDataGetAsync(Settings.PatronBarcode, Settings.PatronPin);
        PapiIntegrationAssert.Success(response);
    }

    [TestMethod]
    public async Task PatronCirculateBlocksGet_WithConfiguredPatron_ReturnsSuccess()
    {
        RequirePatronCredentials();

        var response = await Client.PatronCirculateBlocksGetAsync(Settings.PatronBarcode, Settings.PatronPin);
        PapiIntegrationAssert.Success(response);
    }

    [TestMethod]
    public async Task PatronHoldRequestsGet_WithConfiguredPatron_ReturnsSuccess()
    {
        RequirePatronCredentials();

        var response = await Client.PatronHoldRequestsGetAsync(Settings.PatronBarcode, PatronHoldStatus.all, Settings.PatronPin);
        PapiIntegrationAssert.Success(response);
    }

    [TestMethod]
    public async Task PatronIllRequestsGet_WithConfiguredPatron_ReturnsSuccess()
    {
        RequirePatronCredentials();

        var response = await Client.PatronILLRequestsGetAsync(Settings.PatronBarcode, password: Settings.PatronPin);
        PapiIntegrationAssert.Success(response);
    }

    [TestMethod]
    public async Task PatronItemsOutGet_WithConfiguredPatron_ReturnsSuccess()
    {
        RequirePatronCredentials();

        var response = await Client.PatronItemsOutGetAsync(Settings.PatronBarcode, password: Settings.PatronPin);
        PapiIntegrationAssert.Success(response);
    }

    [TestMethod]
    public async Task PatronMessagesGet_WithConfiguredPatron_ReturnsSuccess()
    {
        RequirePatronCredentials();

        var response = await Client.PatronMessagesGetAsync(Settings.PatronBarcode, password: Settings.PatronPin);
        PapiIntegrationAssert.Success(response);
    }

    [TestMethod]
    public async Task PatronPreferencesGet_WithConfiguredPatron_ReturnsPatronId()
    {
        RequirePatronCredentials();
        RequirePatronId();

        var response = await Client.PatronPreferencesGetAsync(Settings.PatronBarcode, Settings.PatronPin);
        var data = PapiIntegrationAssert.Success(response);
        Assert.AreEqual(Settings.PatronId, data.PatronPreferences!.PatronID);
    }

    [TestMethod]
    public async Task PatronReadingHistoryGet_WithConfiguredPatron_ReturnsRowCountInPapiErrorCode()
    {
        RequirePatronCredentials();

        var response = await Client.PatronReadingHistoryGetAsync(Settings.PatronBarcode, password: Settings.PatronPin);
        var data = PapiIntegrationAssert.HasData(response);
        PapiIntegrationAssert.PapiCodeEqualsRowCount(data.PAPIErrorCode, data.PatronReadingHistoryGetRows.Length, nameof(data.PatronReadingHistoryGetRows));
    }

    [TestMethod]
    public async Task PatronRenewBlocksGet_WithConfiguredPatron_ReturnsSuccess()
    {
        RequirePatronId();

        var response = await Client.PatronRenewBlocksGetAsync(Settings.PatronId, Settings.BranchId > 0 ? Settings.BranchId : null);
        PapiIntegrationAssert.Success(response);
    }

    [TestMethod]
    public async Task PatronSavedSearchesGet_WithConfiguredPatron_ReturnsSuccess()
    {
        RequirePatronCredentials();

        var response = await Client.PatronSavedSearchesGetAsync(Settings.PatronBarcode, Settings.PatronPin);
        PapiIntegrationAssert.Success(response);
    }

    [TestMethod]
    public async Task PatronSearch_WithConfiguredPatronId_ReturnsMatchingRows()
    {
        RequirePatronId();

        var response = await Client.PatronSearchAsync($"PRID={Settings.PatronId}", orgId: Settings.OrganizationId > 0 ? Settings.OrganizationId : null);
        var data = PapiIntegrationAssert.HasData(response);
        PapiIntegrationAssert.PapiCodeEqualsRowCount(data.PAPIErrorCode, data.PatronSearchRows.Count, nameof(data.PatronSearchRows));
    }

    private void RequireScenarioValueForBarcode()
    {
        RequirePatronCredentials();
    }
}
