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

        var response = await Papi.CreatePatronBlocksAsync(Settings.PatronBarcode, BlockType.System, "128", UserIdOrConfigured, WorkstationIdOrConfigured);
        var data = PapiIntegrationAssert.HasPapiData(response);

        CollectionAssert.Contains(new[] { 0, -3507 }, data.PAPIErrorCode);
    }

    [TestMethod]
    public async Task CreatePatronBlocksAsync_LibraryAssignedBlock_WhenMutatingTestsEnabled_ReturnsSuccessOrDuplicateBlock()
    {
        RequireMutatingTestsEnabled();

        var response = await Papi.CreatePatronBlocksAsync(Settings.PatronBarcode, BlockType.LibraryAssigned, "1", UserIdOrConfigured, WorkstationIdOrConfigured);
        var data = PapiIntegrationAssert.HasPapiData(response);

        CollectionAssert.Contains(new[] { 0, -3507 }, data.PAPIErrorCode);
    }

    [TestMethod]
    public async Task PatronMessageDeleteAsync_WithNonexistentMessage_ReturnsDocumentedError()
    {
        RequirePatronCredentials();

        var response = await Papi.PatronMessageDeleteAsync(Settings.PatronBarcode, PatronMessageType.freetext, NonexistentNotificationId, Settings.PatronPin);

        PapiIntegrationAssert.PapiError(response, -1);
    }

    [TestMethod]
    public async Task PatronMessageUpdateStatusAsync_WithNonexistentMessage_ReturnsDocumentedError()
    {
        RequirePatronCredentials();

        var response = await Papi.PatronMessageUpdateStatusAsync(Settings.PatronBarcode, PatronMessageType.freetext, NonexistentNotificationId, Settings.PatronPin);

        PapiIntegrationAssert.PapiError(response, -1);
    }

    [TestMethod]
    public async Task PatronReadingHistoryClearAsync_WithNonexistentTitle_ReturnsDocumentedError()
    {
        RequirePatronCredentials();

        var response = await Papi.PatronReadingHistoryClearAsync(Settings.PatronBarcode, new[] { NonexistentBibId });

        PapiIntegrationAssert.PapiError(response, -10);
    }

    [TestMethod]
    public async Task PatronUpdateAsync_WhenMutatingTestsEnabled_AllowsEmptyNoOpUpdate()
    {
        RequireMutatingTestsEnabled();

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
        RequireScenarioDependentTestsEnabled(
            nameof(Papi.PatronRegistrationCreateAsync),
            "a complete PatronRegistrationParams fixture for a disposable patron registration profile",
            "create a disposable patron, assert a non-negative PAPIErrorCode and returned patron id/barcode, then clean up manually if the site supports it");
    }

    [TestMethod]
    public void PatronRegistrationCreateV2Async_RequiresScenarioDataAndIsDisabledByDefault()
    {
        RequireScenarioDependentTestsEnabled(
            nameof(Papi.PatronRegistrationCreateV2Async),
            "a complete PatronRegistrationData fixture for a disposable patron registration profile",
            "create a disposable patron with the v2 payload and assert a non-negative PAPIErrorCode plus returned patron id/barcode");
    }

    [TestMethod]
    public async Task NotificationUpdateAsync_WithNonexistentNotification_ReturnsDocumentedError()
    {
        RequirePatronId();

        var response = await Papi.NotificationUpdateAsync(new NotificationUpdateParams
        {
            PatronId = Settings.PatronId,
            DeliveryString = "test@example.org",
            ReportingOrgID = OrganizationIdOrConfigured,
            NotificationDeliveryDate = DateTime.UtcNow,
            DeliveryOptionId = 2,
            Details = "integration test reachability",
            NotificationStatusId = NotificationStatus.EmailCompleted,
            NotificationTypeId = NonexistentNotificationId
        });

        PapiIntegrationAssert.PapiError(response, -1);
    }

    [TestMethod]
    public async Task UpdatePatronNotesDataAsync_WhenMutatingTestsEnabled_UpdatesConfiguredPatronNotes()
    {
        RequireMutatingTestsEnabled();

        var response = await Papi.UpdatePatronNotesDataAsync(Settings.PatronBarcode, nonBlockingNote: "PAPI integration test note", updateMode: UpdateNoteMode.Prepend, workstationId: WorkstationIdOrConfigured);

        PapiIntegrationAssert.Success(response);
    }
}
