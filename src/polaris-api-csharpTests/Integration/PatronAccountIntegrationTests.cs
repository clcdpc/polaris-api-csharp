using Clc.Polaris.Api.Models;
using Clc.Polaris.Api.Tests.Integration.Infrastructure;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Clc.Polaris.Api.Tests.Integration;

[TestClass]
[TestCategory("Integration")]
public sealed class PatronAccountIntegrationTests : PapiIntegrationTestBase
{
    [TestMethod]
    public async Task PatronAccountGet_WithConfiguredPatron_ReturnsRows()
    {
        RequirePatronCredentials();

        var response = await Papi.PatronAccountGetAsync(Settings.PatronBarcode, Settings.PatronPin);
        var data = PapiAssert.Success(response);
        Assert.IsNotNull(data.PatronAccountGetRows, "Expected account rows collection to deserialize.");
    }

    [TestMethod]
    public async Task PatronAccountGetTitleLists_WithConfiguredPatron_ReturnsSuccess()
    {
        RequirePatronCredentials();

        var response = await Papi.PatronAccountGetTitleListsAsync(Settings.PatronBarcode, Settings.PatronPin);
        PapiAssert.Success(response);
    }

    [TestMethod]
    public async Task PatronAccountCreateTitleList_ThenDelete_WhenMutatingEnabled()
    {
        RequireMutatingTestsEnabled();
        RequirePatronCredentials();
        RequireNonEmpty(Settings.PatronListName, "TestSettings:PatronListName");

        var uniqueListName = $"{Settings.PatronListName} {Guid.NewGuid():N}";
        int? listId = null;

        try
        {
            var createResponse = await Papi.PatronAccountCreateTitleListAsync(Settings.PatronBarcode, uniqueListName, Settings.PatronPin);
            PapiAssert.Success(createResponse);

            var getResponse = await Papi.PatronAccountGetTitleListsAsync(Settings.PatronBarcode, Settings.PatronPin);
            var getData = PapiAssert.Success(getResponse);
            var createdList = getData.PatronAccountTitleListsRows.Single(row => row.RecordStoreName == uniqueListName);
            listId = createdList.RecordStoreId;
        }
        finally
        {
            if (listId.HasValue)
            {
                var deleteResponse = await Papi.PatronAccountDeleteTitleListAsync(Settings.PatronBarcode, listId.Value, Settings.PatronPin);
                PapiAssert.Success(deleteResponse);
            }
        }
    }

    [TestMethod]
    public async Task PatronAccountPay_WithInvalidTransaction_ReturnsKnownPapiError()
    {
        RequirePatronCredentials();

        var response = await Papi.PatronAccountPayAsync(Settings.PatronBarcode, InvalidPaymentTransactionId, .01, PaymentMethod.Cash, note: "integration reachability test");
        PapiAssert.PapiError(response, -3600);
    }

    [TestMethod]
    public async Task PatronAccountPayAll_WithImpossibleAmount_ReturnsKnownPapiError()
    {
        RequirePatronCredentials();

        var response = await Papi.PatronAccountPayAllAsync(Settings.PatronBarcode, 999999.99, PaymentMethod.Cash, note: "integration reachability test");
        PapiAssert.PapiError(response, -3610);
    }

    [TestMethod]
    public async Task PatronAccountRefundCredit_WithImpossibleAmount_ReturnsKnownPapiError()
    {
        RequirePatronCredentials();

        var response = await Papi.PatronAccountRefundCreditAsync(Settings.PatronBarcode, 999999.99, note: "integration reachability test");
        PapiAssert.PapiError(response, -3606);
    }

    [TestMethod]
    public async Task PatronAccountVoid_WithInvalidTransaction_ReturnsKnownPapiError()
    {
        RequirePatronCredentials();

        var response = await Papi.PatronAccountVoidAsync(Settings.PatronBarcode, InvalidPaymentTransactionId, note: "integration reachability test");
        PapiAssert.PapiError(response, -3606);
    }

    [TestMethod]
    public async Task PatronAccountCreateCredit_WhenMutatingEnabled()
    {
        RequireMutatingTestsEnabled();
        RequireStaffProtectedTestsEnabled();
        RequirePatronCredentials();

        var response = await Papi.PatronAccountCreateCreditAsync(Settings.PatronBarcode, .01, PaymentMethod.Cash, note: "integration mutating test");
        PapiAssert.Success(response);
    }

    [TestMethod]
    public async Task PatronAccountDepositCredit_WhenMutatingEnabled()
    {
        RequireMutatingTestsEnabled();
        RequireStaffProtectedTestsEnabled();
        RequirePatronCredentials();

        var response = await Papi.PatronAccountDepositCreditAsync(Settings.PatronBarcode, .01, note: "integration mutating test");
        PapiAssert.Success(response);
    }
}
