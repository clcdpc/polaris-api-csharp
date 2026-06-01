using Clc.Polaris.Api.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Clc.Polaris.Api.Tests.Integration;

[TestClass]
[TestCategory("Integration")]
public sealed class PatronAccountAndTitleListIntegrationTests : IntegrationTestBase
{
    [TestMethod]
    public async Task PatronAccountGetAsync_WithConfiguredPatron_ReturnsAccountRows()
    {
        RequirePatronCredentials();

        var response = await Papi.PatronAccountGetAsync(Settings.PatronBarcode, Settings.PatronPin);
        var data = PapiIntegrationAssert.Success(response);

        Assert.IsNotNull(data.PatronAccountGetRows);
    }

    [TestMethod]
    public async Task PatronAccountGetTitleListsAsync_WithConfiguredPatron_ReturnsTitleLists()
    {
        RequirePatronCredentials();

        var response = await Papi.PatronAccountGetTitleListsAsync(Settings.PatronBarcode, Settings.PatronPin);
        var data = PapiIntegrationAssert.Success(response);

        Assert.IsNotNull(data.PatronAccountTitleListsRows);
    }

    [TestMethod]
    public void PatronAccountPayAsync_RequiresDisposableAccountFixtureAndIsDisabledByDefault()
    {
        DocumentScenarioDependentPlaceholder(
            nameof(Papi.PatronAccountPayAsync),
            "paying a patron charge mutates account/payment state and must not target a hard-coded charge id that might exist",
            "a disposable patron barcode/PIN, a disposable charge/fee id, payment method, and an approved cleanup/reversal plan",
            "enable mutating tests, pay only the configured disposable charge fixture, and assert the documented PAPIErrorCode without affecting real patron balances");
    }

    [TestMethod]
    public void PatronAccountPayAllAsync_RequiresDisposableAccountFixtureAndIsDisabledByDefault()
    {
        DocumentScenarioDependentPlaceholder(
            nameof(Papi.PatronAccountPayAllAsync),
            "pay-all mutates patron account/payment state and an excessive amount is not a safe default reachability check",
            "a disposable patron barcode/PIN with fixture charges and an approved payment/reversal plan",
            "enable mutating tests, pay only the prepared disposable account fixture, and assert the documented PAPIErrorCode without affecting real patron balances");
    }

    [TestMethod]
    public void PatronAccountRefundCreditAsync_RequiresDisposableAccountFixtureAndIsDisabledByDefault()
    {
        DocumentScenarioDependentPlaceholder(
            nameof(Papi.PatronAccountRefundCreditAsync),
            "refunding credit mutates patron account/payment state and must not run as a default reachability check",
            "a disposable patron barcode/PIN with disposable account credit and an approved fixture-specific refund plan",
            "enable mutating tests, refund only the prepared disposable credit fixture, and assert the documented PAPIErrorCode without affecting real patron balances");
    }

    [TestMethod]
    public void PatronAccountVoidAsync_RequiresDisposableAccountFixtureAndIsDisabledByDefault()
    {
        DocumentScenarioDependentPlaceholder(
            nameof(Papi.PatronAccountVoidAsync),
            "voiding a transaction mutates patron account/payment state and must not target a hard-coded transaction id that might exist",
            "a disposable patron barcode/PIN and disposable transaction id created specifically for a void test",
            "enable mutating tests, void only the configured disposable transaction fixture, and assert the documented PAPIErrorCode without affecting real patron balances");
    }

    [TestMethod]
    public void PatronTitleListMutations_RequireDisposableTitleListFixturesAndAreDisabledByDefault()
    {
        DocumentScenarioDependentPlaceholder(
            "PatronTitleList add/copy/delete/move methods",
            "title-list add, copy, delete, and move operations mutate patron title-list data and must not target hard-coded list or bib ids that might exist",
            "a disposable patron barcode/PIN, disposable source/destination title lists, and disposable/approved bib/title entries",
            "enable mutating tests, mutate only lists created by the fixture/test, assert documented PAPIErrorCode values, and delete only data the test definitely created");
    }

    [TestMethod]
    public async Task PatronAccountCreateAndDeleteTitleListAsync_WhenMutatingTestsEnabled_CleansUpList()
    {
        RequireMutatingTestsEnabled();
        RequirePatronCredentials();
        if (string.IsNullOrWhiteSpace(Settings.PatronListName))
        {
            Assert.Inconclusive("Mutating title-list flow requires TestSettings:PatronListName.");
        }

        var uniqueListName = $"{Settings.PatronListName}-{Guid.NewGuid():N}";
        int? createdListId = null;
        try
        {
            var createResponse = await Papi.PatronAccountCreateTitleListAsync(Settings.PatronBarcode, uniqueListName, Settings.PatronPin);
            PapiIntegrationAssert.ExactZeroSuccess(createResponse);

            var getResponse = await Papi.PatronAccountGetTitleListsAsync(Settings.PatronBarcode, Settings.PatronPin);
            var getData = PapiIntegrationAssert.Success(getResponse);
            var list = getData.PatronAccountTitleListsRows.SingleOrDefault(l => l.RecordStoreName == uniqueListName);
            Assert.IsNotNull(list, "The title list created by this test should be visible before cleanup.");
            createdListId = list.RecordStoreId;
        }
        finally
        {
            if (createdListId.HasValue)
            {
                var deleteResponse = await Papi.PatronAccountDeleteTitleListAsync(Settings.PatronBarcode, createdListId.Value, Settings.PatronPin);
                PapiIntegrationAssert.Success(deleteResponse);
            }
        }
    }

    [TestMethod]
    public async Task PatronAccountCreateCreditAsync_WhenMutatingTestsEnabled_CreatesCredit()
    {
        RequireMutatingTestsEnabled();
        RequirePatronCredentials();

        var response = await Papi.PatronAccountCreateCreditAsync(Settings.PatronBarcode, .01, PaymentMethod.Cash, WorkstationIdOrConfigured, UserIdOrConfigured, "integration testing");

        PapiIntegrationAssert.Success(response);
    }

    [TestMethod]
    public async Task PatronAccountDepositCreditAsync_WhenMutatingTestsEnabled_DepositsCredit()
    {
        RequireMutatingTestsEnabled();
        RequirePatronCredentials();

        var response = await Papi.PatronAccountDepositCreditAsync(Settings.PatronBarcode, .01, WorkstationIdOrConfigured, UserIdOrConfigured, "integration testing");

        PapiIntegrationAssert.Success(response);
    }
}
