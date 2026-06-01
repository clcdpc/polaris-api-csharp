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
    public async Task PatronAccountPayAsync_WithNonexistentCharge_ReturnsDocumentedError()
    {
        RequirePatronCredentials();

        var response = await Papi.PatronAccountPayAsync(Settings.PatronBarcode, NonexistentNotificationId, .01, PaymentMethod.Cash, note: "integration testing");

        PapiIntegrationAssert.PapiError(response, -3600);
    }

    [TestMethod]
    public async Task PatronAccountPayAllAsync_WithExcessiveAmount_ReturnsDocumentedError()
    {
        RequirePatronCredentials();

        var response = await Papi.PatronAccountPayAllAsync(Settings.PatronBarcode, 999999.99, PaymentMethod.Cash, note: "integration testing");

        PapiIntegrationAssert.PapiError(response, -3610);
    }

    [TestMethod]
    public async Task PatronAccountRefundCreditAsync_WithExcessiveAmount_ReturnsDocumentedError()
    {
        RequirePatronCredentials();

        var response = await Papi.PatronAccountRefundCreditAsync(Settings.PatronBarcode, 999999.99, note: "integration testing");

        PapiIntegrationAssert.PapiError(response, -3606);
    }

    [TestMethod]
    public async Task PatronAccountVoidAsync_WithNonexistentTransaction_ReturnsDocumentedError()
    {
        RequirePatronCredentials();

        var response = await Papi.PatronAccountVoidAsync(Settings.PatronBarcode, NonexistentNotificationId, note: "integration testing");

        PapiIntegrationAssert.PapiError(response, -3606);
    }

    [TestMethod]
    public async Task PatronTitleListMethods_WithNonexistentLists_ReturnDocumentedErrors()
    {
        RequirePatronCredentials();

        PapiIntegrationAssert.PapiError(await Papi.PatronTitleListAddTitleAsync(Settings.PatronBarcode, NonexistentTitleListId, NonexistentBibId, Settings.PatronPin), -1);
        PapiIntegrationAssert.PapiError(await Papi.PatronTitleListCopyAllTitlesAsync(Settings.PatronBarcode, NonexistentTitleListId, NonexistentTitleListId + 1, Settings.PatronPin), -1);
        PapiIntegrationAssert.PapiError(await Papi.PatronTitleListCopyTitleAsync(Settings.PatronBarcode, NonexistentTitleListId, 1, NonexistentTitleListId + 1, Settings.PatronPin), -1);
        PapiIntegrationAssert.PapiError(await Papi.PatronTitleListDeleteAllTitlesAsync(Settings.PatronBarcode, NonexistentTitleListId, Settings.PatronPin), -1);
        PapiIntegrationAssert.PapiError(await Papi.PatronTitleListDeleteTitleAsync(Settings.PatronBarcode, NonexistentTitleListId, 1, Settings.PatronPin), -1);
        PapiIntegrationAssert.PapiError(await Papi.PatronTitleListGetTitlesAsync(Settings.PatronBarcode, NonexistentTitleListId, password: Settings.PatronPin), -1);
        PapiIntegrationAssert.PapiError(await Papi.PatronTitleListMoveTitleAsync(Settings.PatronBarcode, NonexistentTitleListId, 1, NonexistentTitleListId + 1, Settings.PatronPin), -1);
    }

    [TestMethod]
    public async Task PatronAccountCreateAndDeleteTitleListAsync_WhenMutatingTestsEnabled_CleansUpList()
    {
        RequireMutatingTestsEnabled();
        if (string.IsNullOrWhiteSpace(Settings.PatronListName))
        {
            Assert.Inconclusive("Mutating title-list flow requires TestSettings:PatronListName.");
        }

        int? createdListId = null;
        try
        {
            var createResponse = await Papi.PatronAccountCreateTitleListAsync(Settings.PatronBarcode, Settings.PatronListName, Settings.PatronPin);
            var createData = PapiIntegrationAssert.HasPapiData(createResponse);
            Assert.IsTrue(createData.PAPIErrorCode == 0 || createData.PAPIErrorCode == -1, createData.ErrorMessage);

            var getResponse = await Papi.PatronAccountGetTitleListsAsync(Settings.PatronBarcode, Settings.PatronPin);
            var getData = PapiIntegrationAssert.Success(getResponse);
            var list = getData.PatronAccountTitleListsRows.SingleOrDefault(l => l.RecordStoreName == Settings.PatronListName);
            Assert.IsNotNull(list, "The created or pre-existing test title list should be visible before cleanup.");
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

        var response = await Papi.PatronAccountCreateCreditAsync(Settings.PatronBarcode, .01, PaymentMethod.Cash, WorkstationIdOrConfigured, UserIdOrConfigured, "integration testing");

        PapiIntegrationAssert.Success(response);
    }

    [TestMethod]
    public async Task PatronAccountDepositCreditAsync_WhenMutatingTestsEnabled_DepositsCredit()
    {
        RequireMutatingTestsEnabled();

        var response = await Papi.PatronAccountDepositCreditAsync(Settings.PatronBarcode, .01, WorkstationIdOrConfigured, UserIdOrConfigured, "integration testing");

        PapiIntegrationAssert.Success(response);
    }
}
