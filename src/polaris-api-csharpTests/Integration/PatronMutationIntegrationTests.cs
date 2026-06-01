using Clc.Polaris.Api.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Net;

namespace Clc.Polaris.Api.Tests.Integration;

[TestClass]
[TestCategory("Integration")]
public sealed class PatronMutationIntegrationTests : IntegrationTestBase
{
    [TestMethod]
    public async Task CreatePatronBlocksAsync_FreeText_WhenMutatingTestsEnabled_ReturnsSuccessOrDuplicateBlock()
    {
        RequireMutatingTestsEnabled();
        RequireStaffProtectedTestsEnabled();
        RequirePatronCredentials();
        if (string.IsNullOrWhiteSpace(Settings.FreeTextBlock))
        {
            Assert.Inconclusive("Mutating patron block tests require TestSettings:FreeTextBlock.");
        }

        var response = await Papi.CreatePatronBlocksAsync(Settings.PatronBarcode, BlockType.FreeText, Settings.FreeTextBlock, UserIdOrConfigured, WorkstationIdOrConfigured);
        var data = PapiIntegrationAssert.HasPapiData(response);

        CollectionAssert.Contains(new[] { 0, -3507 }, data.PAPIErrorCode);
    }

    [TestMethod]
    public async Task CreatePatronBlocksAsync_SystemBlock_WhenMutatingTestsEnabled_ReturnsSuccessOrDuplicateBlock()
    {
        RequireMutatingTestsEnabled();
        RequireStaffProtectedTestsEnabled();
        RequirePatronCredentials();

        var response = await Papi.CreatePatronBlocksAsync(Settings.PatronBarcode, BlockType.System, "128", UserIdOrConfigured, WorkstationIdOrConfigured);
        var data = PapiIntegrationAssert.HasPapiData(response);

        CollectionAssert.Contains(new[] { 0, -3507 }, data.PAPIErrorCode);
    }

    [TestMethod]
    public async Task CreatePatronBlocksAsync_LibraryAssignedBlock_WhenMutatingTestsEnabled_ReturnsSuccessOrDuplicateBlock()
    {
        RequireMutatingTestsEnabled();
        RequireStaffProtectedTestsEnabled();
        RequirePatronCredentials();

        var response = await Papi.CreatePatronBlocksAsync(Settings.PatronBarcode, BlockType.LibraryAssigned, "1", UserIdOrConfigured, WorkstationIdOrConfigured);
        var data = PapiIntegrationAssert.HasPapiData(response);

        CollectionAssert.Contains(new[] { 0, -3507 }, data.PAPIErrorCode);
    }

    [TestMethod]
    public void PatronMessageDeleteAsync_RequiresDisposableMessageFixtureAndIsDisabledByDefault()
    {
        DocumentScenarioDependentPlaceholder(
            nameof(Papi.PatronMessageDeleteAsync),
            "deleting a patron message mutates patron notification/message state and a hard-coded notification ID might exist in a live Polaris database",
            "IntegrationTestOptions:EnableMutatingIntegrationTests=true, disposable patron credentials, and a configured disposable patron message fixture",
            "call PatronMessageDeleteAsync only for a disposable message and assert the documented response");
    }

    [TestMethod]
    public void PatronMessageUpdateStatusAsync_RequiresDisposableMessageFixtureAndIsDisabledByDefault()
    {
        DocumentScenarioDependentPlaceholder(
            nameof(Papi.PatronMessageUpdateStatusAsync),
            "updating patron message status mutates patron notification/message state and a hard-coded notification ID might exist in a live Polaris database",
            "IntegrationTestOptions:EnableMutatingIntegrationTests=true, disposable patron credentials, and a configured disposable patron message fixture",
            "call PatronMessageUpdateStatusAsync only for a disposable message and assert the documented response");
    }

    [TestMethod]
    public void PatronReadingHistoryClearAsync_RequiresDisposableReadingHistoryFixtureAndIsDisabledByDefault()
    {
        DocumentScenarioDependentPlaceholder(
            nameof(Papi.PatronReadingHistoryClearAsync),
            "clearing reading-history entries mutates patron history and a hard-coded bib ID might exist in a live Polaris database",
            "IntegrationTestOptions:EnableMutatingIntegrationTests=true, disposable patron credentials, and configured disposable reading-history title fixture",
            "call PatronReadingHistoryClearAsync only for disposable reading-history data and assert the documented response");
    }

    [TestMethod]
    public async Task PatronUpdateAsync_WhenMutatingTestsEnabled_AllowsEmptyNoOpUpdate()
    {
        RequireMutatingTestsEnabled();
        RequirePatronCredentials();

        var response = await Papi.PatronUpdateAsync(Settings.PatronBarcode, new PatronUpdateParams(), Settings.PatronPin);

        PapiIntegrationAssert.Success(response);
    }

    [TestMethod]
    public async Task PatronUpdateUserNameAsync_WithUnsafeBarcode_ReturnsUnauthorizedWithoutChangingPatron()
    {
        RequireMutatingTestsEnabled();
        RequirePatronCredentials();

        var response = await Papi.PatronUpdateUserNameAsync(Settings.PatronBarcode + "-nonexistent", Settings.PatronPin, Settings.PatronPin);

        Assert.IsNotNull(response.Response);
        Assert.AreEqual(HttpStatusCode.Unauthorized, response.Response!.StatusCode);
    }

    [TestMethod]
    public void PatronRegistrationCreateAsync_RequiresScenarioDataAndIsDisabledByDefault()
    {
        DocumentScenarioDependentPlaceholder(
            nameof(Papi.PatronRegistrationCreateAsync),
            "patron registration creates patron data and requires a complete site-specific disposable registration profile",
            "a complete PatronRegistrationParams fixture for a disposable patron registration profile",
            "create a disposable patron, assert a non-negative PAPIErrorCode and returned patron id/barcode, then clean up manually if the site supports it");
    }

    [TestMethod]
    public void PatronRegistrationCreateV2Async_RequiresScenarioDataAndIsDisabledByDefault()
    {
        DocumentScenarioDependentPlaceholder(
            nameof(Papi.PatronRegistrationCreateV2Async),
            "patron registration v2 creates patron data and requires a complete site-specific disposable registration profile",
            "a complete PatronRegistrationData fixture for a disposable patron registration profile",
            "create a disposable patron with the v2 payload and assert a non-negative PAPIErrorCode plus returned patron id/barcode");
    }

    [TestMethod]
    public void NotificationUpdateAsync_RequiresDisposableNotificationFixtureAndIsDisabledByDefault()
    {
        DocumentScenarioDependentPlaceholder(
            nameof(Papi.NotificationUpdateAsync),
            "updating notification state mutates patron notification data and a hard-coded notification type/id might exist in a live Polaris database",
            "IntegrationTestOptions:EnableMutatingIntegrationTests=true and a configured disposable notification fixture for a disposable patron",
            "call NotificationUpdateAsync only for disposable notification data and assert the documented response");
    }

    [TestMethod]
    public async Task UpdatePatronNotesDataAsync_WhenMutatingTestsEnabled_UpdatesConfiguredPatronNotes()
    {
        RequireMutatingTestsEnabled();
        RequireStaffProtectedTestsEnabled();
        RequirePatronCredentials();

        var response = await Papi.UpdatePatronNotesDataAsync(Settings.PatronBarcode, nonBlockingNote: "PAPI integration test note", updateMode: UpdateNoteMode.Prepend, workstationId: WorkstationIdOrConfigured);

        PapiIntegrationAssert.Success(response);
    }
}
