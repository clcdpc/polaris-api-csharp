using Clc.Polaris.Api.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Clc.Polaris.Api;

[TestClass]
[TestCategory("Integration")]
public sealed class PatronAccountAndHoldIntegrationTests : PapiIntegrationTestBase
{
    [TestMethod]
    public async Task PatronAccountGet_returns_success_for_configured_patron()
    {
        RequirePatronCredentials();

        var data = PapiIntegrationAssert.Success(await Papi.PatronAccountGetAsync(Settings.PatronBarcode, Settings.PatronPin));
        Assert.IsNotNull(data.PatronAccountGetRows);
    }

    [TestMethod]
    public async Task PatronAccount_payment_methods_reach_api_with_nonexistent_transactions()
    {
        RequirePatronCredentials();

        PapiIntegrationAssert.PapiError(await Papi.PatronAccountPayAsync(Settings.PatronBarcode, NonexistentId, .01, PaymentMethod.Cash, note: IntegrationNote), -3600);
        PapiIntegrationAssert.PapiError(await Papi.PatronAccountPayAllAsync(Settings.PatronBarcode, 999999.99, PaymentMethod.Cash, note: IntegrationNote), -3610);
        PapiIntegrationAssert.PapiError(await Papi.PatronAccountRefundCreditAsync(Settings.PatronBarcode, 999999.99, note: IntegrationNote), -3606);
        PapiIntegrationAssert.PapiError(await Papi.PatronAccountVoidAsync(Settings.PatronBarcode, NonexistentId, note: IntegrationNote), -3606);
    }

    [TestMethod]
    public async Task PatronAccount_credit_mutations_are_gated()
    {
        RequireMutatingTestsEnabled();
        RequirePatronCredentials();

        PapiIntegrationAssert.Success(await Papi.PatronAccountCreateCreditAsync(Settings.PatronBarcode, .01, PaymentMethod.Cash));
        PapiIntegrationAssert.Success(await Papi.PatronAccountDepositCreditAsync(Settings.PatronBarcode, .01, note: IntegrationNote));
    }

    [TestMethod]
    public async Task TitleList_create_get_delete_flow_is_gated_and_cleans_up()
    {
        RequireMutatingTestsEnabled();
        RequirePatronCredentials();

        var listName = $"{Settings.PatronListName} {Guid.NewGuid():N}";
        int? listId = null;
        try
        {
            var create = PapiIntegrationAssert.HasData(await Papi.PatronAccountCreateTitleListAsync(Settings.PatronBarcode, listName, Settings.PatronPin));
            Assert.IsTrue(create.PAPIErrorCode is 0 or -1, create.ErrorMessage);

            var lists = PapiIntegrationAssert.Success(await Papi.PatronAccountGetTitleListsAsync(Settings.PatronBarcode, Settings.PatronPin));
            listId = lists.PatronAccountTitleListsRows.Single(l => l.RecordStoreName == listName).RecordStoreId;
        }
        finally
        {
            if (listId.HasValue)
            {
                PapiIntegrationAssert.Success(await Papi.PatronAccountDeleteTitleListAsync(Settings.PatronBarcode, listId.Value, Settings.PatronPin));
            }
        }
    }

    [TestMethod]
    public async Task TitleList_methods_reach_api_with_nonexistent_list_ids()
    {
        RequirePatronCredentials();

        PapiIntegrationAssert.PapiError(await Papi.PatronTitleListAddTitleAsync(Settings.PatronBarcode, NonexistentId, NonexistentId, Settings.PatronPin), -1);
        PapiIntegrationAssert.PapiError(await Papi.PatronTitleListCopyAllTitlesAsync(Settings.PatronBarcode, NonexistentId, NonexistentId, Settings.PatronPin), -1);
        PapiIntegrationAssert.PapiError(await Papi.PatronTitleListCopyTitleAsync(Settings.PatronBarcode, NonexistentId, NonexistentId, NonexistentId, Settings.PatronPin), -1);
        PapiIntegrationAssert.PapiError(await Papi.PatronTitleListDeleteAllTitlesAsync(Settings.PatronBarcode, NonexistentId, Settings.PatronPin), -1);
        PapiIntegrationAssert.PapiError(await Papi.PatronTitleListDeleteTitleAsync(Settings.PatronBarcode, NonexistentId, NonexistentId, Settings.PatronPin), -1);
        var titles = PapiIntegrationAssert.HasData(await Papi.PatronTitleListGetTitlesAsync(Settings.PatronBarcode, NonexistentId, password: Settings.PatronPin));
        Assert.AreEqual(-1, titles.PAPIErrorCode, titles.ErrorMessage?.ToString());
        PapiIntegrationAssert.PapiError(await Papi.PatronTitleListMoveTitleAsync(Settings.PatronBarcode, NonexistentId, NonexistentId, NonexistentId, Settings.PatronPin), -1);
    }

    [TestMethod]
    public async Task HoldRequest_methods_reach_api_with_nonexistent_request_or_bib_ids()
    {
        RequirePatronCredentials();
        var branchId = RequireBranchId();

        PapiIntegrationAssert.PapiError(await Papi.HoldRequestCancelAsync(Settings.PatronBarcode, NonexistentId, Settings.PatronPin), -4201);
        PapiIntegrationAssert.PapiError(await Papi.HoldRequestCreateAsync(new HoldRequestCreateParams(Settings.PatronId, NonexistentId, branchId, branchId)), -4006);
        PapiIntegrationAssert.PapiError(await Papi.HoldRequestCreateAsync(Settings.PatronId, NonexistentId, branchId, requestingOrgId: branchId), -4006);
        PapiIntegrationAssert.PapiError(await Papi.HoldRequestReactivateAsync(Settings.PatronBarcode, Settings.PatronPin, NonexistentId, DateTime.UtcNow), -4201);
        PapiIntegrationAssert.PapiError(await Papi.HoldRequestReplyAsync(new HoldRequestCreateResult { RequestGuid = Guid.Empty }, branchId, HoldRequestReplyAnswer.Yes, HoldRequestReplyState.AcceptEvenWithExistingHolds), -4101);
        PapiIntegrationAssert.PapiError(await Papi.HoldRequestSuspendAsync(Settings.PatronBarcode, NonexistentId, DateTime.UtcNow, Settings.PatronPin), -4201);
        PapiIntegrationAssert.PapiError(await Papi.UpdatePickupBranchIDAsync(Settings.PatronBarcode, NonexistentId, branchId, Settings.PatronPin), -4201);
    }

    [TestMethod]
    public async Task HoldRequestGetList_returns_success_for_configured_branch()
    {
        var data = PapiIntegrationAssert.Success(await Papi.HoldRequestGetListAsync(RequireBranchId()));
        Assert.IsNotNull(data.RequestPicklistRows);
    }
}
