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
            "paying a charge mutates patron account/payment state and a hard-coded charge ID might exist in a live Polaris database",
            "IntegrationTestOptions:EnableMutatingIntegrationTests=true, disposable patron credentials, and a configured disposable charge fixture",
            "call PatronAccountPayAsync only for disposable charge data and assert the documented payment response");
    }

    [TestMethod]
    public void PatronAccountPayAllAsync_RequiresDisposableAccountFixtureAndIsDisabledByDefault()
    {
        DocumentScenarioDependentPlaceholder(
            nameof(Papi.PatronAccountPayAllAsync),
            "pay-all mutates patron account/payment state and should not run against a real patron without disposable account fixtures",
            "IntegrationTestOptions:EnableMutatingIntegrationTests=true, disposable patron credentials, and configured disposable account balance data",
            "call PatronAccountPayAllAsync only for disposable account data and assert the documented response without affecting real balances");
    }

    [TestMethod]
    public void PatronAccountRefundCreditAsync_RequiresDisposableAccountFixtureAndIsDisabledByDefault()
    {
        DocumentScenarioDependentPlaceholder(
            nameof(Papi.PatronAccountRefundCreditAsync),
            "refund-credit mutates patron account/payment state and should not run against a real patron without disposable credit fixtures",
            "IntegrationTestOptions:EnableMutatingIntegrationTests=true, disposable patron credentials, and configured disposable credit balance data",
            "call PatronAccountRefundCreditAsync only for disposable credit data and assert the documented response without affecting real balances");
    }

    [TestMethod]
    public void PatronAccountVoidAsync_RequiresDisposableAccountFixtureAndIsDisabledByDefault()
    {
        DocumentScenarioDependentPlaceholder(
            nameof(Papi.PatronAccountVoidAsync),
            "voiding a payment mutates patron account/payment state and a hard-coded transaction ID might exist in a live Polaris database",
            "IntegrationTestOptions:EnableMutatingIntegrationTests=true, disposable patron credentials, and a configured disposable transaction fixture",
            "call PatronAccountVoidAsync only for disposable transaction data and assert the documented response");
    }

    [TestMethod]
    public void PatronTitleListMethods_RequireDisposableTitleListFixturesAndAreDisabledByDefault()
    {
        DocumentScenarioDependentPlaceholder(
            "PatronTitleList add/copy/delete/move methods",
            "most title-list operations mutate patron title-list content and hard-coded list or bib IDs might exist in a live Polaris database",
            "IntegrationTestOptions:EnableMutatingIntegrationTests=true, disposable patron credentials, and configured disposable source/destination title-list plus bib fixtures",
            "call each title-list method only against disposable title lists and assert documented PAPIErrorCode values while preserving pre-existing lists");
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
