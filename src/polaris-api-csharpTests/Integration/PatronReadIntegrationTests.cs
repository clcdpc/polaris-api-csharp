using Clc.Polaris.Api.Models;
using Clc.Polaris.Api.Tests.Integration.Infrastructure;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Clc.Polaris.Api.Tests.Integration;

[TestClass]
[TestCategory("Integration")]
public sealed class PatronReadIntegrationTests : PapiIntegrationTestBase
{
    [TestMethod]
    public async Task PatronGetBarcodeFromId_WithConfiguredPatron_ReturnsBarcode()
    {
        RequirePatronId();
        RequireNonEmpty(Settings.PatronBarcode, "TestSettings:PatronBarcode");

        var response = await Papi.Patron_GetBarcodeFromIdAsync(Settings.PatronId);
        var data = PapiAssert.HasData(response);
        Assert.AreEqual(Settings.PatronBarcode, data.Barcode);
    }

    [TestMethod]
    public async Task PatronValidate_WithConfiguredPatron_ReturnsPatronId()
    {
        RequirePatronId();
        RequirePatronCredentials();

        var response = await Papi.PatronValidateAsync(Settings.PatronBarcode, Settings.PatronPin);
        var data = PapiAssert.HasData(response);
        Assert.AreEqual(Settings.PatronId, data.PatronID);
    }

    [TestMethod]
    public async Task PatronBasicDataGet_WithConfiguredPatron_ReturnsBasicData()
    {
        RequirePatronId();
        RequirePatronCredentials();

        var response = await Papi.PatronBasicDataGetAsync(Settings.PatronBarcode, Settings.PatronPin, addresses: true);
        var data = PapiAssert.Success(response);
        Assert.IsNotNull(data.PatronBasicData, "Expected patron basic data.");
        Assert.AreEqual(Settings.PatronId, data.PatronBasicData!.PatronID);
    }

    [TestMethod]
    public async Task PatronPreferencesGet_WithConfiguredPatron_ReturnsPreferences()
    {
        RequirePatronId();
        RequirePatronCredentials();

        var response = await Papi.PatronPreferencesGetAsync(Settings.PatronBarcode, Settings.PatronPin);
        var data = PapiAssert.HasData(response);
        Assert.IsNotNull(data.PatronPreferences, "Expected patron preferences data.");
        Assert.AreEqual(Settings.PatronId, data.PatronPreferences!.PatronID);
    }

    [TestMethod]
    public async Task PatronCirculateBlocksGet_WithConfiguredPatron_ReturnsSuccess()
    {
        RequirePatronCredentials();

        var response = await Papi.PatronCirculateBlocksGetAsync(Settings.PatronBarcode, Settings.PatronPin);
        PapiAssert.Success(response);
    }

    [TestMethod]
    public async Task PatronRenewBlocksGet_WithConfiguredPatron_ReturnsSuccess()
    {
        RequirePatronId();

        var response = await Papi.PatronRenewBlocksGetAsync(Settings.PatronId);
        PapiAssert.Success(response);
    }

    [TestMethod]
    public async Task PatronSearch_ByConfiguredPatronId_ReturnsRowCountCode()
    {
        RequirePatronId();

        var response = await Papi.PatronSearchAsync($"PRID={Settings.PatronId}");
        var data = PapiAssert.HasData(response);
        Assert.AreEqual(data.PatronSearchRows.Count, data.PAPIErrorCode);
    }

    [TestMethod]
    public async Task PatronHoldRequestsGet_WithConfiguredPatron_ReturnsSuccess()
    {
        RequirePatronCredentials();

        var response = await Papi.PatronHoldRequestsGetAsync(Settings.PatronBarcode, PatronHoldStatus.all, Settings.PatronPin);
        PapiAssert.Success(response);
    }

    [TestMethod]
    public async Task PatronIllRequestsGet_WithConfiguredPatron_ReturnsSuccess()
    {
        RequirePatronCredentials();

        var response = await Papi.PatronILLRequestsGetAsync(Settings.PatronBarcode, password: Settings.PatronPin);
        PapiAssert.Success(response);
    }

    [TestMethod]
    public async Task PatronItemsOutGet_WithConfiguredPatron_ReturnsSuccess()
    {
        RequirePatronCredentials();

        var response = await Papi.PatronItemsOutGetAsync(Settings.PatronBarcode, password: Settings.PatronPin);
        PapiAssert.Success(response);
    }

    [TestMethod]
    public async Task PatronMessagesGet_WithConfiguredPatron_ReturnsSuccess()
    {
        RequirePatronCredentials();

        var response = await Papi.PatronMessagesGetAsync(Settings.PatronBarcode, password: Settings.PatronPin);
        PapiAssert.Success(response);
    }

    [TestMethod]
    public async Task PatronReadingHistoryGet_WithConfiguredPatron_ReturnsRowCountCode()
    {
        RequirePatronCredentials();

        var response = await Papi.PatronReadingHistoryGetAsync(Settings.PatronBarcode, password: Settings.PatronPin);
        var data = PapiAssert.HasData(response);
        Assert.AreEqual(data.PatronReadingHistoryGetRows.Length, data.PAPIErrorCode);
    }

    [TestMethod]
    public async Task PatronSavedSearchesGet_WithConfiguredPatron_ReturnsSuccess()
    {
        RequirePatronCredentials();

        var response = await Papi.PatronSavedSearchesGetAsync(Settings.PatronBarcode, Settings.PatronPin);
        PapiAssert.Success(response);
    }
}
