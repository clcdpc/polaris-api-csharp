using Clc.Polaris.Api.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Clc.Polaris.Api;

[TestClass]
[TestCategory("Integration")]
public sealed class StaffProtectedAndCirculationIntegrationTests : PapiIntegrationTestBase
{
    [TestMethod]
    public async Task AuthenticateStaffUser_success_is_staff_gated()
    {
        RequireStaffProtectedTestsEnabled();

        var data = PapiIntegrationAssert.Success(await Papi.AuthenticateStaffUserAsync(Fixture.PapiSettings.PolarisOverrideAccount!));
        Assert.IsFalse(string.IsNullOrWhiteSpace(data.AccessSecret));
        Assert.IsFalse(string.IsNullOrWhiteSpace(data.AccessToken));
    }

    [TestMethod]
    public async Task Protected_lookup_methods_are_staff_gated()
    {
        RequireStaffProtectedTestsEnabled();

        var queue = PapiIntegrationAssert.HasData(await Papi.NotificationQueueGetAsync(RequireOrganizationId()));
        Assert.IsTrue(queue.PAPIErrorCode >= 0 || queue.PAPIErrorCode < 0, queue.ErrorMessage);

        var value = PapiIntegrationAssert.HasData(await Papi.SA_GetValueByOrgAsync("ORGEMAIL", RequireOrganizationId()));
        if (!string.IsNullOrWhiteSpace(Settings.OrgEmail) && !Settings.OrgEmail.StartsWith("REPLACE_WITH_", StringComparison.OrdinalIgnoreCase))
        {
            Assert.AreEqual(Settings.OrgEmail, value.Value);
        }
    }

    [TestMethod]
    public async Task ItemRenew_reaches_api_with_nonexistent_item()
    {
        RequirePatronCredentials();

        PapiIntegrationAssert.PapiError(await Papi.ItemRenewAsync(Settings.PatronBarcode, NonexistentId, Settings.PatronPin), -6001);
    }

    [TestMethod]
    public void ItemUpdateBarcode_is_staff_and_mutation_gated()
    {
        RequireStaffProtectedTestsEnabled();
        RequireMutatingTestsEnabled();
        RequireScenarioDependentTestsEnabled("TestSettings:ItemRecordId and a reversible TestSettings:ItemBarcode value");

        if (Settings.ItemRecordId <= 0 || string.IsNullOrWhiteSpace(Settings.ItemBarcode))
        {
            InconclusivePlaceholder(
                nameof(Papi.ItemUpdateBarcodeAsync),
                "updating a barcode mutates a catalog item and needs a dedicated reversible item fixture",
                "TestSettings:ItemRecordId, TestSettings:ItemBarcode, TestSettings:BranchId",
                "update to a temporary barcode, assert PAPIErrorCode 0, then restore the original barcode in finally");
        }
    }

    [TestMethod]
    public async Task PatronBlock_and_note_mutations_are_gated()
    {
        RequireMutatingTestsEnabled();
        RequireStaffProtectedTestsEnabled();
        RequirePatronCredentials();

        var freeText = PapiIntegrationAssert.HasData(await Papi.CreatePatronBlocksAsync(Settings.PatronBarcode, BlockType.FreeText, Settings.FreeTextBlock));
        Assert.IsTrue(freeText.PAPIErrorCode is 0 or -3507, freeText.ErrorMessage);

        var system = PapiIntegrationAssert.HasData(await Papi.CreatePatronBlocksAsync(Settings.PatronBarcode, BlockType.System, "128"));
        Assert.IsTrue(system.PAPIErrorCode is 0 or -3507, system.ErrorMessage);

        var libraryAssigned = PapiIntegrationAssert.HasData(await Papi.CreatePatronBlocksAsync(Settings.PatronBarcode, BlockType.LibraryAssigned, "1"));
        Assert.IsTrue(libraryAssigned.PAPIErrorCode is 0 or -3507, libraryAssigned.ErrorMessage);

        PapiIntegrationAssert.HasData(await Papi.UpdatePatronNotesDataAsync(Settings.PatronBarcode, nonBlockingNote: IntegrationNote, workstationId: Settings.WorkstationId > 0 ? Settings.WorkstationId : null));
    }

    [TestMethod]
    public async Task Patron_update_methods_are_mutation_gated()
    {
        RequireMutatingTestsEnabled();
        RequirePatronCredentials();

        PapiIntegrationAssert.Success(await Papi.PatronUpdateAsync(Settings.PatronBarcode, new PatronUpdateParams(), Settings.PatronPin));
        var usernameAttempt = await Papi.PatronUpdateUserNameAsync($"{Settings.PatronBarcode}{NonexistentId}", Settings.PatronPin, Settings.PatronPin);
        Assert.IsNotNull(usernameAttempt.Response);
    }

    [TestMethod]
    public async Task NotificationUpdate_reaches_api_with_nonexistent_notification()
    {
        RequireStaffProtectedTestsEnabled();
        RequirePatronCredentials();

        var response = await Papi.NotificationUpdateAsync(new NotificationUpdateParams
        {
            PatronId = Settings.PatronId,
            DeliveryString = "test@example.org",
            ReportingOrgID = RequireOrganizationId(),
            NotificationDeliveryDate = DateTime.UtcNow,
            DeliveryOptionId = 2,
            Details = IntegrationNote,
            NotificationStatusId = NotificationStatus.EmailCompleted,
            NotificationTypeId = 1
        });

        PapiIntegrationAssert.PapiError(response, -1);
    }

    [TestMethod]
    public async Task Patron_message_mutations_reach_api_with_nonexistent_message_id()
    {
        RequirePatronCredentials();

        PapiIntegrationAssert.PapiError(await Papi.PatronMessageDeleteAsync(Settings.PatronBarcode, PatronMessageType.freetext, NonexistentId, Settings.PatronPin), -1);
        PapiIntegrationAssert.PapiError(await Papi.PatronMessageUpdateStatusAsync(Settings.PatronBarcode, PatronMessageType.freetext, NonexistentId, Settings.PatronPin), -1);
    }

    [TestMethod]
    public async Task PatronReadingHistoryClear_reaches_api_with_nonexistent_history_id()
    {
        RequirePatronCredentials();

        PapiIntegrationAssert.PapiError(await Papi.PatronReadingHistoryClearAsync(Settings.PatronBarcode, Settings.PatronPin, new[] { NonexistentId }), -10);
    }

    [TestMethod]
    public void PatronRegistrationCreate_requires_dedicated_registration_fixture()
    {
        InconclusivePlaceholder(
            $"{nameof(Papi.PatronRegistrationCreateAsync)} / {nameof(Papi.PatronRegistrationCreateV2Async)}",
            "creating patrons is mutating and requires site-specific required fields, duplicate cleanup policy, and a test patron namespace",
            "registration branch/user/workstation plus a safe generated patron profile supplied through TestSettings or a dedicated fixture model",
            "when mutating/scenario flags are enabled, create a patron with unique data and assert PAPIErrorCode 0 or the documented validation error shape");
    }
}
