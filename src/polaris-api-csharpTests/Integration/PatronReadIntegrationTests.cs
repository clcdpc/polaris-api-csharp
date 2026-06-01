using Clc.Polaris.Api.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Clc.Polaris.Api;

[TestClass]
[TestCategory("Integration")]
public sealed class PatronReadIntegrationTests : PapiIntegrationTestBase
{
    [TestMethod]
    public async Task PatronBasicDataGet_returns_configured_patron()
    {
        RequirePatronCredentials();

        var data = PapiIntegrationAssert.Success(await Papi.PatronBasicDataGetAsync(Settings.PatronBarcode, Settings.PatronPin, addresses: true));
        Assert.AreEqual(Settings.PatronId, data.PatronBasicData!.PatronID);
        Assert.IsNotNull(data.PatronBasicData!.PatronAddresses);
    }

    [TestMethod]
    public async Task PatronValidate_returns_configured_patron()
    {
        RequirePatronCredentials();

        var data = PapiIntegrationAssert.Success(await Papi.PatronValidateAsync(Settings.PatronBarcode, Settings.PatronPin));
        Assert.AreEqual(Settings.PatronId, data.PatronID);
    }

    [TestMethod]
    public async Task PatronGetBarcodeFromId_returns_configured_barcode()
    {
        RequirePatronCredentials();

        var data = PapiIntegrationAssert.Success(await Papi.Patron_GetBarcodeFromIdAsync(Settings.PatronId));
        Assert.AreEqual(Settings.PatronBarcode, data.Barcode);
    }

    [TestMethod]
    public async Task PatronReadCollections_return_success_or_row_count()
    {
        RequirePatronCredentials();

        PapiIntegrationAssert.Success(await Papi.PatronCirculateBlocksGetAsync(Settings.PatronBarcode, Settings.PatronPin));
        PapiIntegrationAssert.Success(await Papi.PatronILLRequestsGetAsync(Settings.PatronBarcode, password: Settings.PatronPin));
        PapiIntegrationAssert.Success(await Papi.PatronItemsOutGetAsync(Settings.PatronBarcode, password: Settings.PatronPin));
        PapiIntegrationAssert.Success(await Papi.PatronMessagesGetAsync(Settings.PatronBarcode, password: Settings.PatronPin));
        PapiIntegrationAssert.Success(await Papi.PatronRenewBlocksGetAsync(Settings.PatronId));
        PapiIntegrationAssert.Success(await Papi.PatronSavedSearchesGetAsync(Settings.PatronBarcode, Settings.PatronPin));

        var readingHistory = await Papi.PatronReadingHistoryGetAsync(Settings.PatronBarcode, password: Settings.PatronPin);
        PapiIntegrationAssert.RowCountEqualsPapiCode(readingHistory, PapiIntegrationAssert.HasData(readingHistory).PatronReadingHistoryGetRows.Count());

        var holdRequests = await Papi.PatronHoldRequestsGetAsync(Settings.PatronBarcode, PatronHoldStatus.all, Settings.PatronPin);
        var holdData = PapiIntegrationAssert.HasData(holdRequests);
        Assert.IsTrue(holdData.PAPIErrorCode >= 0, holdData.ErrorMessage);
    }

    [TestMethod]
    public async Task PatronPreferences_returns_configured_patron()
    {
        RequirePatronCredentials();

        var data = PapiIntegrationAssert.HasData(await Papi.PatronPreferencesGetAsync(Settings.PatronBarcode, Settings.PatronPin));
        Assert.AreEqual(Settings.PatronId, data.PatronPreferences!.PatronID);
    }

    [TestMethod]
    public async Task PatronCodesGet_returns_deserialized_rows()
    {
        var data = PapiIntegrationAssert.Success(await Papi.PatronCodesGetAsync(RequireBranchId()));
        Assert.IsNotNull(data.PatronCodesRows);
    }

    [TestMethod]
    public async Task PatronSearch_by_configured_id_returns_row_count_as_papi_code()
    {
        RequirePatronCredentials();

        var response = await Papi.PatronSearchAsync($"PRID={Settings.PatronId}", orgId: RequireOrganizationId());
        PapiIntegrationAssert.RowCountEqualsPapiCode(response, PapiIntegrationAssert.HasData(response).PatronSearchRows.Count);
    }

    [TestMethod]
    public async Task AuthenticatePatron_success_is_enabled_only_with_patron_credentials()
    {
        RequirePatronCredentials();

        var data = PapiIntegrationAssert.Success(await Papi.AuthenticatePatronAsync(Settings.PatronBarcode, Settings.PatronPin));
        Assert.AreEqual(Settings.PatronId, data.PatronID);
    }

    [TestMethod]
    public void AuthenticatePatron_failure_is_guarded_to_avoid_account_lockout()
    {
        if (!Options.EnableAuthenticationFailureTests)
        {
            Assert.Inconclusive("Failed-authentication tests are disabled by default because repeated bad PINs can lock accounts. Set IntegrationTestOptions:EnableAuthenticationFailureTests=true only against a safe fixture account.");
        }

        InconclusivePlaceholder(
            nameof(Papi.AuthenticatePatronAsync),
            "a safe lockout-resistant patron account and an explicitly approved invalid PIN are required",
            "TestSettings:PatronBarcode, TestSettings:PatronPin plus an intentionally invalid PIN supplied out-of-band",
            "assert a documented authentication PAPIErrorCode without repeating enough attempts to lock the account");
    }
}
