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
    [DoNotParallelize]
    public async Task PatronTitleListSurface_WhenMutatingTestsEnabled_CleansUpCreatedLists()
    {
        RequireMutatingTestsEnabled();
        RequirePatronCredentials();
        if (string.IsNullOrWhiteSpace(Settings.PatronListName))
        {
            Assert.Inconclusive("Mutating title-list workflow requires TestSettings:PatronListName so created lists can be uniquely named for this test run.");
        }

        var cleanup = new IntegrationCleanup();
        var runId = Guid.NewGuid().ToString("N");

        try
        {
            var sourceListId = await CreateDisposableTitleListAsync($"{Settings.PatronListName}-{runId}-source", cleanup);
            var destinationListId = await CreateDisposableTitleListAsync($"{Settings.PatronListName}-{runId}-destination", cleanup);

            if (Settings.BibId <= 0)
            {
                Assert.Inconclusive("Mutating title-list item workflow requires TestSettings:BibId for a real bibliographic record that can be added to disposable lists created by this test run.");
            }

            var addResponse = await Papi.PatronTitleListAddTitleAsync(Settings.PatronBarcode, sourceListId, Settings.BibId, Settings.PatronPin);
            var addData = PapiIntegrationAssert.Success(addResponse);
            Assert.IsTrue(addData.Position > 0, "Adding a title to a disposable title list should return the inserted position.");

            var sourceTitlesAfterAdd = PapiIntegrationAssert.Success(
                await Papi.PatronTitleListGetTitlesAsync(Settings.PatronBarcode, sourceListId, password: Settings.PatronPin));
            Assert.IsTrue(
                sourceTitlesAfterAdd.PatronTitleListTitleRows.Any(row => row.LocalControlNumber == Settings.BibId),
                "The disposable source list should contain the configured bibliographic record after add.");

            var copyResponse = await Papi.PatronTitleListCopyTitleAsync(Settings.PatronBarcode, sourceListId, addData.Position, destinationListId, Settings.PatronPin);
            PapiIntegrationAssert.Success(copyResponse);

            var destinationTitlesAfterCopy = PapiIntegrationAssert.Success(
                await Papi.PatronTitleListGetTitlesAsync(Settings.PatronBarcode, destinationListId, password: Settings.PatronPin));
            Assert.IsTrue(destinationTitlesAfterCopy.PatronTitleListTitleRows.Length > 0, "The disposable destination list should contain a title after copy.");

            var copiedPosition = destinationTitlesAfterCopy.PatronTitleListTitleRows[0].Position;
            var moveResponse = await Papi.PatronTitleListMoveTitleAsync(Settings.PatronBarcode, destinationListId, copiedPosition, sourceListId, Settings.PatronPin);
            PapiIntegrationAssert.Success(moveResponse);

            var sourceTitlesAfterMove = PapiIntegrationAssert.Success(
                await Papi.PatronTitleListGetTitlesAsync(Settings.PatronBarcode, sourceListId, password: Settings.PatronPin));
            Assert.IsTrue(sourceTitlesAfterMove.PatronTitleListTitleRows.Length >= 2, "The disposable source list should contain both the original and moved title before delete.");

            var deleteTitleResponse = await Papi.PatronTitleListDeleteTitleAsync(Settings.PatronBarcode, sourceListId, sourceTitlesAfterMove.PatronTitleListTitleRows[0].Position, Settings.PatronPin);
            PapiIntegrationAssert.Success(deleteTitleResponse);

            var deleteAllSourceResponse = await Papi.PatronTitleListDeleteAllTitlesAsync(Settings.PatronBarcode, sourceListId, Settings.PatronPin);
            PapiIntegrationAssert.Success(deleteAllSourceResponse);

            var deleteAllDestinationResponse = await Papi.PatronTitleListDeleteAllTitlesAsync(Settings.PatronBarcode, destinationListId, Settings.PatronPin);
            PapiIntegrationAssert.Success(deleteAllDestinationResponse);
        }
        finally
        {
            await cleanup.RunAsync();
        }
    }

    private async Task<int> CreateDisposableTitleListAsync(string uniqueListName, IntegrationCleanup cleanup)
    {
        var createResponse = await Papi.PatronAccountCreateTitleListAsync(Settings.PatronBarcode, uniqueListName, Settings.PatronPin);
        PapiIntegrationAssert.ExactZeroSuccess(createResponse);

        cleanup.Add($"delete generated title list '{uniqueListName}'", async () =>
        {
            var listId = await FindExactlyOneGeneratedTitleListIdAsync(uniqueListName);
            var deleteResponse = await Papi.PatronAccountDeleteTitleListAsync(Settings.PatronBarcode, listId, Settings.PatronPin);
            PapiIntegrationAssert.Success(deleteResponse);
        });

        return await FindExactlyOneGeneratedTitleListIdAsync(uniqueListName);
    }

    private async Task<int> FindExactlyOneGeneratedTitleListIdAsync(string uniqueListName)
    {
        var getResponse = await Papi.PatronAccountGetTitleListsAsync(Settings.PatronBarcode, Settings.PatronPin);
        var getData = PapiIntegrationAssert.Success(getResponse);
        var matchingLists = getData.PatronAccountTitleListsRows
            .Where(l => string.Equals(l.RecordStoreName, uniqueListName, StringComparison.Ordinal))
            .ToList();

        Assert.AreEqual(
            1,
            matchingLists.Count,
            $"Expected exactly one generated title list named '{uniqueListName}'. Cleanup will not delete pre-existing or ambiguous title lists; manual cleanup may be required.");
        return matchingLists[0].RecordStoreId;
    }

    [TestMethod]
    public void PatronAccountCreditSurface_WhenMutatingTestsEnabled_RequiresReversibleGeneratedCreditArtifact()
    {
        DocumentScenarioDependentPlaceholder(
            "PatronAccountCreateCreditAsync and PatronAccountDepositCreditAsync",
            "create-credit and deposit-credit mutate durable patron account state. The current client responses only expose common PAPI status, and the account rows must expose enough unique generated transaction/credit information for a later refund, void, or equivalent reversal before this can safely be executable",
            "IntegrationTestOptions:EnableMutatingIntegrationTests=true, IntegrationTestOptions:EnableStaffProtectedTests=true, disposable patron credentials, staff override credentials, and a verified model/API cleanup path that can uniquely identify the generated account row by a test-created note or transaction identifier and reverse it",
            "create or deposit only a tiny generated credit, read PatronAccountGetAsync rows, uniquely identify exactly the generated row, register refund/void cleanup immediately, run cleanup in finally, and fail rather than mutating if the row cannot be identified uniquely");
    }

}
