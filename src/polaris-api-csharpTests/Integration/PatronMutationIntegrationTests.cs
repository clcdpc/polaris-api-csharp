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
        RequirePatronCredentials();

        var response = await Papi.CreatePatronBlocksAsync(Settings.PatronBarcode, BlockType.System, "128", UserIdOrConfigured, WorkstationIdOrConfigured);
        var data = PapiIntegrationAssert.HasPapiData(response);

        CollectionAssert.Contains(new[] { 0, -3507 }, data.PAPIErrorCode);
    }

    [TestMethod]
    public async Task CreatePatronBlocksAsync_LibraryAssignedBlock_WhenMutatingTestsEnabled_ReturnsSuccessOrDuplicateBlock()
    {
        RequireMutatingTestsEnabled();
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
            "deleting a patron message mutates patron data and must not target a hard-coded message/notification id that might exist",
            "a disposable patron barcode/PIN and a disposable patron message id created specifically for deletion",
            "enable mutating tests, delete only the disposable message fixture, and assert the documented PAPIErrorCode without affecting real patron messages");
    }

    [TestMethod]
    public void PatronMessageUpdateStatusAsync_RequiresDisposableMessageFixtureAndIsDisabledByDefault()
    {
        DocumentScenarioDependentPlaceholder(
            nameof(Papi.PatronMessageUpdateStatusAsync),
            "updating patron message status mutates patron data and must not target a hard-coded message/notification id that might exist",
            "a disposable patron barcode/PIN and a disposable patron message id created specifically for status updates",
            "enable mutating tests, update only the disposable message fixture, and assert the documented PAPIErrorCode without affecting real patron messages");
    }

    [TestMethod]
    public void PatronReadingHistoryClearAsync_RequiresDisposableReadingHistoryFixtureAndIsDisabledByDefault()
    {
        DocumentScenarioDependentPlaceholder(
            nameof(Papi.PatronReadingHistoryClearAsync),
            "clearing reading history mutates patron history and must not target a hard-coded bib id that might exist",
            "a disposable patron barcode/PIN with disposable reading-history entries that are safe to clear",
            "enable mutating tests, clear only configured disposable reading-history entries, and assert the documented PAPIErrorCode without affecting real patron history");
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
            "patron registration creates a new patron record and requires a complete site-specific disposable registration profile",
            "a complete PatronRegistrationParams fixture for a disposable patron registration profile",
            "create a disposable patron, assert a non-negative PAPIErrorCode and returned patron id/barcode, then clean up manually if the site supports it");
    }

    [TestMethod]
    public void PatronRegistrationCreateV2Async_RequiresScenarioDataAndIsDisabledByDefault()
    {
        DocumentScenarioDependentPlaceholder(
            nameof(Papi.PatronRegistrationCreateV2Async),
            "patron registration v2 creates a new patron record and requires a complete site-specific disposable registration profile",
            "a complete PatronRegistrationData fixture for a disposable patron registration profile",
            "create a disposable patron with the v2 payload and assert a non-negative PAPIErrorCode plus returned patron id/barcode");
    }

    [TestMethod]
    public void NotificationUpdateAsync_RequiresDisposableNotificationFixtureAndIsDisabledByDefault()
    {
        DocumentScenarioDependentPlaceholder(
            nameof(Papi.NotificationUpdateAsync),
            "updating notification data mutates patron notification state and must not target hard-coded notification values that might exist",
            "a disposable patron id and notification fixture values approved for update testing",
            "enable mutating tests, update only the disposable notification fixture, and assert the documented PAPIErrorCode without affecting real patron notification data");
    }

    [TestMethod]
    public async Task UpdatePatronNotesDataAsync_WhenMutatingTestsEnabled_UpdatesConfiguredPatronNotes()
    {
        RequireMutatingTestsEnabled();
        RequirePatronCredentials();

        var response = await Papi.UpdatePatronNotesDataAsync(Settings.PatronBarcode, nonBlockingNote: "PAPI integration test note", updateMode: UpdateNoteMode.Prepend, workstationId: WorkstationIdOrConfigured);

        PapiIntegrationAssert.Success(response);
    }
}
