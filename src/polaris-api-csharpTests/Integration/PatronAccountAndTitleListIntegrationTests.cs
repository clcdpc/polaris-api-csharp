using Clc.Polaris.Api.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Clc.Polaris.Api.Tests.Integration;

[TestClass]
[TestCategory("Integration")]
public sealed class PatronAccountAndTitleListIntegrationTests : PapiIntegrationTestBase
{
    [TestMethod]
    public async Task PatronAccountGet_WithConfiguredPatron_ReturnsSuccess()
    {
        RequirePatronCredentials();

        var response = await Client.PatronAccountGetAsync(Settings.PatronBarcode, Settings.PatronPin);
        PapiIntegrationAssert.Success(response);
    }

    [TestMethod]
    public async Task PatronAccountGetTitleLists_WithConfiguredPatron_ReturnsSuccess()
    {
        RequirePatronCredentials();

        var response = await Client.PatronAccountGetTitleListsAsync(Settings.PatronBarcode, Settings.PatronPin);
        PapiIntegrationAssert.Success(response);
    }

    [TestMethod]
    public async Task PatronAccountCreateCredit_WhenMutatingEnabled_CreatesSmallCredit()
    {
        RequireMutatingTestsEnabled();
        RequirePatronCredentials();

        var response = await Client.PatronAccountCreateCreditAsync(Settings.PatronBarcode, .01, PaymentMethod.Cash, Settings.EffectiveWorkstationId, Settings.EffectiveUserId, "PAPI integration test");
        PapiIntegrationAssert.Success(response);
    }

    [TestMethod]
    public async Task PatronAccountDepositCredit_WhenMutatingEnabled_CreatesSmallDeposit()
    {
        RequireMutatingTestsEnabled();
        RequirePatronCredentials();

        var response = await Client.PatronAccountDepositCreditAsync(Settings.PatronBarcode, .01, Settings.EffectiveWorkstationId, Settings.EffectiveUserId, "PAPI integration test");
        PapiIntegrationAssert.Success(response);
    }

    [TestMethod]
    public async Task PatronAccountPay_WithInvalidTransaction_ReturnsKnownError()
    {
        RequirePatronCredentials();

        var response = await Client.PatronAccountPayAsync(Settings.PatronBarcode, NonexistentId, .01, PaymentMethod.Cash, Settings.EffectiveWorkstationId, Settings.EffectiveUserId, "PAPI integration test");
        PapiIntegrationAssert.ErrorCode(response, -3600);
    }

    [TestMethod]
    public async Task PatronAccountPayAll_WithExcessiveAmount_ReturnsKnownError()
    {
        RequirePatronCredentials();

        var response = await Client.PatronAccountPayAllAsync(Settings.PatronBarcode, 999999.99, PaymentMethod.Cash, Settings.EffectiveWorkstationId, Settings.EffectiveUserId, "PAPI integration test");
        PapiIntegrationAssert.ErrorCode(response, -3610);
    }

    [TestMethod]
    public async Task PatronAccountRefundCredit_WithExcessiveAmount_ReturnsKnownError()
    {
        RequirePatronCredentials();

        var response = await Client.PatronAccountRefundCreditAsync(Settings.PatronBarcode, 999999.99, Settings.EffectiveWorkstationId, Settings.EffectiveUserId, "PAPI integration test");
        PapiIntegrationAssert.ErrorCode(response, -3606);
    }

    [TestMethod]
    public async Task PatronAccountVoid_WithInvalidPaymentTransaction_ReturnsKnownError()
    {
        RequirePatronCredentials();

        var response = await Client.PatronAccountVoidAsync(Settings.PatronBarcode, NonexistentId, Settings.EffectiveWorkstationId, Settings.EffectiveUserId, "PAPI integration test");
        PapiIntegrationAssert.ErrorCode(response, -3600);
    }

    [TestMethod]
    public async Task PatronTitleListCreateGetDelete_WhenMutatingEnabled_RoundTripsAndCleansUp()
    {
        RequireMutatingTestsEnabled();
        RequirePatronCredentials();
        if (string.IsNullOrWhiteSpace(Settings.PatronListName) || Settings.PatronListName.StartsWith("REPLACE_", StringComparison.OrdinalIgnoreCase))
        {
            Assert.Inconclusive("TestSettings:PatronListName must identify a disposable list name for this mutating test.");
        }

        int? listId = null;
        try
        {
            var createResponse = await Client.PatronAccountCreateTitleListAsync(Settings.PatronBarcode, Settings.PatronListName, Settings.PatronPin);
            PapiIntegrationAssert.ErrorCodeIn(createResponse, 0, -1);

            var getResponse = await Client.PatronAccountGetTitleListsAsync(Settings.PatronBarcode, Settings.PatronPin);
            var getData = PapiIntegrationAssert.Success(getResponse);
            var list = getData.PatronAccountTitleListsRows.SingleOrDefault(l => l.RecordStoreName == Settings.PatronListName);
            Assert.IsNotNull(list, "Created title list should be returned by PatronAccountGetTitleLists.");
            listId = list!.RecordStoreId;
        }
        finally
        {
            if (listId.HasValue)
            {
                var deleteResponse = await Client.PatronAccountDeleteTitleListAsync(Settings.PatronBarcode, listId.Value, Settings.PatronPin);
                PapiIntegrationAssert.Success(deleteResponse);
            }
        }
    }

    [TestMethod]
    public async Task PatronTitleListMethods_WithInvalidListIds_ReturnKnownErrors()
    {
        RequirePatronCredentials();

        PapiIntegrationAssert.ErrorCode(await Client.PatronTitleListAddTitleAsync(Settings.PatronBarcode, NonexistentId, NonexistentId, Settings.PatronPin), -1);
        PapiIntegrationAssert.ErrorCode(await Client.PatronTitleListCopyAllTitlesAsync(Settings.PatronBarcode, NonexistentId, NonexistentLargeId, Settings.PatronPin), -1);
        PapiIntegrationAssert.ErrorCode(await Client.PatronTitleListCopyTitleAsync(Settings.PatronBarcode, NonexistentId, NonexistentId, NonexistentLargeId, Settings.PatronPin), -1);
        PapiIntegrationAssert.ErrorCode(await Client.PatronTitleListDeleteAllTitlesAsync(Settings.PatronBarcode, NonexistentId, Settings.PatronPin), -1);
        PapiIntegrationAssert.ErrorCode(await Client.PatronTitleListDeleteTitleAsync(Settings.PatronBarcode, NonexistentId, NonexistentId, Settings.PatronPin), -1);
        var getTitles = await Client.PatronTitleListGetTitlesAsync(Settings.PatronBarcode, NonexistentId, password: Settings.PatronPin);
        Assert.IsNotNull(getTitles.Data);
        Assert.AreEqual(-1, getTitles.Data!.PAPIErrorCode);
        PapiIntegrationAssert.ErrorCode(await Client.PatronTitleListMoveTitleAsync(Settings.PatronBarcode, NonexistentId, NonexistentId, NonexistentLargeId, Settings.PatronPin), -1);
    }
}
