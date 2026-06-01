using Clc.Polaris.Api.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Net;

namespace Clc.Polaris.Api.Tests.Integration;

[TestClass]
[TestCategory("Integration")]
public sealed class PatronMutationIntegrationTests : PapiIntegrationTestBase
{
    [TestMethod]
    public async Task CreatePatronBlocks_WhenMutatingEnabled_CreatesFreeTextBlockOrReportsDuplicate()
    {
        RequireMutatingTestsEnabled();
        RequirePatronCredentials();
        if (string.IsNullOrWhiteSpace(Settings.FreeTextBlock) || Settings.FreeTextBlock.StartsWith("REPLACE_", StringComparison.OrdinalIgnoreCase))
        {
            Assert.Inconclusive("TestSettings:FreeTextBlock must identify a disposable block value for this mutating test.");
        }

        var response = await Client.CreatePatronBlocksAsync(Settings.PatronBarcode, BlockType.FreeText, Settings.FreeTextBlock, Settings.EffectiveUserId, Settings.EffectiveWorkstationId);
        PapiIntegrationAssert.ErrorCodeIn(response, 0, -3507);
    }

    [TestMethod]
    public async Task CreatePatronBlocks_WithInvalidLibraryAssignedBlock_ReturnsKnownError()
    {
        RequirePatronCredentials();
        RequireStaffProtectedTestsEnabled();

        var response = await Client.CreatePatronBlocksAsync(Settings.PatronBarcode, BlockType.LibraryAssigned, NonexistentLargeId.ToString(), Settings.EffectiveUserId, Settings.EffectiveWorkstationId);
        PapiIntegrationAssert.ErrorCode(response, -3506);
    }

    [TestMethod]
    public async Task NotificationUpdate_WithIncompleteNotification_ReturnsKnownError()
    {
        RequirePatronId();
        RequireStaffProtectedTestsEnabled();

        var response = await Client.NotificationUpdateAsync(new NotificationUpdateParams
        {
            PatronId = Settings.PatronId,
            DeliveryString = "test@example.org",
            ReportingOrgID = Settings.EffectiveOrganizationId,
            NotificationDeliveryDate = DateTime.UtcNow,
            DeliveryOptionId = 2,
            Details = "PAPI integration reachability test",
            NotificationStatusId = NotificationStatus.EmailCompleted,
            NotificationTypeId = 1
        });
        PapiIntegrationAssert.ErrorCode(response, -1);
    }

    [TestMethod]
    public async Task NotificationQueueGet_WhenStaffEnabled_ReturnsDeserializedResponse()
    {
        RequireStaffProtectedTestsEnabled();

        var response = await Client.NotificationQueueGetAsync(Settings.EffectiveOrganizationId);
        PapiIntegrationAssert.HasData(response);
    }

    [TestMethod]
    public async Task PatronMessageDelete_WithInvalidMessage_ReturnsKnownError()
    {
        RequirePatronCredentials();

        var response = await Client.PatronMessageDeleteAsync(Settings.PatronBarcode, PatronMessageType.freetext, NonexistentId, Settings.PatronPin);
        PapiIntegrationAssert.ErrorCode(response, -1);
    }

    [TestMethod]
    public async Task PatronMessageUpdateStatus_WithInvalidMessage_ReturnsKnownError()
    {
        RequirePatronCredentials();

        var response = await Client.PatronMessageUpdateStatusAsync(Settings.PatronBarcode, PatronMessageType.freetext, NonexistentId, Settings.PatronPin);
        PapiIntegrationAssert.ErrorCode(response, -1);
    }

    [TestMethod]
    public async Task PatronReadingHistoryClear_WithInvalidEntry_ReturnsKnownError()
    {
        RequirePatronCredentials();

        var response = await Client.PatronReadingHistoryClearAsync(Settings.PatronBarcode, Settings.PatronPin, new[] { NonexistentId });
        PapiIntegrationAssert.ErrorCode(response, -10);
    }

    [TestMethod]
    public async Task PatronUpdate_WithEmptyUpdate_WhenMutatingEnabled_ReturnsSuccess()
    {
        RequireMutatingTestsEnabled();
        RequirePatronCredentials();

        var response = await Client.PatronUpdateAsync(Settings.PatronBarcode, new PatronUpdateParams(), Settings.PatronPin);
        PapiIntegrationAssert.Success(response);
    }

    [TestMethod]
    public void PatronUpdateUserName_WithUnsafeScenario_IsPlaceholder()
    {
        RequireScenarioDependentTestsEnabled(
            nameof(Client.PatronUpdateUserNameAsync),
            "a disposable patron and a configured replacement username that can be restored",
            "perform update/restore in try/finally and assert PAPIErrorCode=0 for both calls");
        Assert.Inconclusive("Changing patron usernames is intentionally not performed without an explicit reversible fixture.");
    }

    [TestMethod]
    public void PatronRegistrationCreate_RequiresScenarioData()
    {
        RequireScenarioDependentTestsEnabled(
            nameof(Client.PatronRegistrationCreateAsync),
            "registration profile values accepted by the target Polaris environment plus cleanup guidance",
            "create a disposable patron and assert the returned patron id/barcode, then remove or quarantine through local cleanup process");
        Assert.Inconclusive("Patron registration requires environment-specific policy data and is not guessed by the integration suite.");
    }

    [TestMethod]
    public void PatronRegistrationCreateV2_RequiresScenarioData()
    {
        RequireScenarioDependentTestsEnabled(
            nameof(Client.PatronRegistrationCreateV2Async),
            "registration V2 data accepted by the target Polaris environment plus cleanup guidance",
            "create a disposable patron and assert the returned patron id/barcode, then remove or quarantine through local cleanup process");
        Assert.Inconclusive("Patron registration V2 requires environment-specific policy data and is not guessed by the integration suite.");
    }

    [TestMethod]
    public async Task UpdatePatronNotesData_WithInvalidPatron_WhenStaffEnabled_ReturnsKnownError()
    {
        RequireStaffProtectedTestsEnabled();

        var response = await Client.UpdatePatronNotesDataAsync($"PAPI-NONEXISTENT-{NonexistentLargeId}", nonBlockingNote: "PAPI integration reachability test", workstationId: Settings.EffectiveWorkstationId);
        var data = PapiIntegrationAssert.HasData(response);
        Assert.IsTrue(data.PAPIErrorCode < 0, $"Expected invalid patron notes update to return a negative PAPI error code, but received {data.PAPIErrorCode}: {data.ErrorMessage}");
    }
}
