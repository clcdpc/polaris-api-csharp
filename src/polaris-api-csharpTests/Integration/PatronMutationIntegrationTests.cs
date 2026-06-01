using Clc.Polaris.Api.Models;
using Clc.Polaris.Api.Tests.Integration.Infrastructure;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Clc.Polaris.Api.Tests.Integration;

[TestClass]
[TestCategory("Integration")]
public sealed class PatronMutationIntegrationTests : PapiIntegrationTestBase
{
    [TestMethod]
    public async Task CreatePatronFreeTextBlock_WhenMutatingEnabled_ReturnsSuccessOrAlreadyExists()
    {
        RequireMutatingTestsEnabled();
        RequireStaffProtectedTestsEnabled();
        RequirePatronCredentials();
        RequireNonEmpty(Settings.FreeTextBlock, "TestSettings:FreeTextBlock");

        var response = await Papi.CreatePatronBlocksAsync(Settings.PatronBarcode, BlockType.FreeText, Settings.FreeTextBlock);
        var data = PapiAssert.HasData(response);
        CollectionAssert.Contains(new[] { 0, -3507 }, data.PAPIErrorCode, "Expected success or documented already-present block result.");
    }

    [TestMethod]
    public async Task CreatePatronSystemBlock_WhenMutatingEnabled_ReturnsSuccessOrAlreadyExists()
    {
        RequireMutatingTestsEnabled();
        RequireStaffProtectedTestsEnabled();
        RequirePatronCredentials();

        var response = await Papi.CreatePatronBlocksAsync(Settings.PatronBarcode, BlockType.System, "128");
        var data = PapiAssert.HasData(response);
        CollectionAssert.Contains(new[] { 0, -3507 }, data.PAPIErrorCode, "Expected success or documented already-present block result.");
    }

    [TestMethod]
    public async Task CreatePatronLibraryAssignedBlock_WhenMutatingEnabled_ReturnsSuccessOrAlreadyExists()
    {
        RequireMutatingTestsEnabled();
        RequireStaffProtectedTestsEnabled();
        RequirePatronCredentials();

        var response = await Papi.CreatePatronBlocksAsync(Settings.PatronBarcode, BlockType.LibraryAssigned, "1");
        var data = PapiAssert.HasData(response);
        CollectionAssert.Contains(new[] { 0, -3507 }, data.PAPIErrorCode, "Expected success or documented already-present block result.");
    }

    [TestMethod]
    public async Task NotificationUpdate_WithInvalidNotification_ReturnsKnownPapiError()
    {
        RequirePatronId();
        RequireBranchId();

        var response = await Papi.NotificationUpdateAsync(new NotificationUpdateParams
        {
            PatronId = Settings.PatronId,
            DeliveryString = "integration-test@example.org",
            ReportingOrgID = EffectiveBranchId,
            NotificationDeliveryDate = DateTime.UtcNow,
            DeliveryOptionId = 2,
            Details = "integration reachability test",
            NotificationStatusId = NotificationStatus.EmailCompleted,
            NotificationTypeId = 1
        });

        PapiAssert.PapiError(response, -1);
    }

    [TestMethod]
    public async Task PatronMessageDelete_WithInvalidMessage_ReturnsKnownPapiError()
    {
        RequirePatronCredentials();

        var response = await Papi.PatronMessageDeleteAsync(Settings.PatronBarcode, PatronMessageType.freetext, InvalidMessageId, Settings.PatronPin);
        PapiAssert.PapiError(response, -1);
    }

    [TestMethod]
    public async Task PatronMessageUpdateStatus_WithInvalidMessage_ReturnsKnownPapiError()
    {
        RequirePatronCredentials();

        var response = await Papi.PatronMessageUpdateStatusAsync(Settings.PatronBarcode, PatronMessageType.freetext, InvalidMessageId, Settings.PatronPin);
        PapiAssert.PapiError(response, -1);
    }

    [TestMethod]
    public async Task PatronReadingHistoryClear_WithInvalidEntry_ReturnsKnownPapiError()
    {
        RequirePatronCredentials();

        var response = await Papi.PatronReadingHistoryClearAsync(Settings.PatronBarcode, Settings.PatronPin, new[] { InvalidRecordId });
        PapiAssert.PapiError(response, -10);
    }

    [TestMethod]
    public async Task PatronUpdate_WithEmptyPatch_WhenMutatingEnabled_ReturnsSuccess()
    {
        RequireMutatingTestsEnabled();
        RequirePatronCredentials();

        var response = await Papi.PatronUpdateAsync(Settings.PatronBarcode, new PatronUpdateParams(), Settings.PatronPin);
        PapiAssert.Success(response);
    }

    [TestMethod]
    public void PatronUpdateUserName_RequiresDisposablePatronFixture()
    {
        RequireScenarioDependentTestsEnabled("PatronUpdateUserNameAsync changes login data and requires an explicit disposable patron fixture plus cleanup/restore assertions.");
    }

    [TestMethod]
    public void PatronRegistrationCreate_RequiresDisposableRegistrationFixture()
    {
        RequireScenarioDependentTestsEnabled("PatronRegistrationCreateAsync requires site-specific registration policy fields and a cleanup strategy for newly created patron records.");
    }

    [TestMethod]
    public void PatronRegistrationCreateV2_RequiresDisposableRegistrationFixture()
    {
        RequireScenarioDependentTestsEnabled("PatronRegistrationCreateV2Async requires site-specific registration policy fields and a cleanup strategy for newly created patron records.");
    }

    [TestMethod]
    public async Task UpdatePatronNotesData_WhenMutatingEnabled_ReturnsDeserializedResponse()
    {
        RequireMutatingTestsEnabled();
        RequireStaffProtectedTestsEnabled();
        RequirePatronCredentials();

        var response = await Papi.UpdatePatronNotesDataAsync(Settings.PatronBarcode, nonBlockingNote: "PAPI integration test note", updateMode: UpdateNoteMode.Append, workstationId: EffectiveWorkstationId);
        PapiAssert.HasData(response);
    }
}
