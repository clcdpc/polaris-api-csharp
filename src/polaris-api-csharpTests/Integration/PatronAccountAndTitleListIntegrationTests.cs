using Clc.Polaris.Api.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Runtime.ExceptionServices;

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
            Assert.Inconclusive("Mutating title-list flow requires TestSettings:PatronListName.");
        }

        if (Settings.BibId <= 0)
        {
            Assert.Inconclusive("Mutating title-list item coverage requires TestSettings:BibId because PatronTitleListAddTitleAsync needs a real bibliographic record. No live API call was made.");
        }

        var cleanup = new IntegrationCleanup();
        ExceptionDispatchInfo? testFailure = null;
        var runId = Guid.NewGuid().ToString("N");
        var sourceListName = $"{Settings.PatronListName}-{runId}-source";
        var destinationListName = $"{Settings.PatronListName}-{runId}-destination";

        try
        {
            var sourceListId = await CreateDisposableTitleListAsync(sourceListName, cleanup);
            var destinationListId = await CreateDisposableTitleListAsync(destinationListName, cleanup);

            var addResponse = await Papi.PatronTitleListAddTitleAsync(Settings.PatronBarcode, sourceListId, Settings.BibId, Settings.PatronPin);
            var addedTitle = PapiIntegrationAssert.Success(addResponse);
            Assert.IsTrue(addedTitle.Position > 0, "Adding a title should return a positive source-list position.");

            var sourceTitlesResponse = await Papi.PatronTitleListGetTitlesAsync(Settings.PatronBarcode, sourceListId, password: Settings.PatronPin);
            var sourceTitles = PapiIntegrationAssert.Success(sourceTitlesResponse);
            Assert.IsTrue(
                sourceTitles.PatronTitleListTitleRows.Any(t => t.Position == addedTitle.Position && t.LocalControlNumber == Settings.BibId),
                "The title added by this workflow should be visible in the disposable source list.");

            var copyAllResponse = await Papi.PatronTitleListCopyAllTitlesAsync(Settings.PatronBarcode, sourceListId, destinationListId, Settings.PatronPin);
            PapiIntegrationAssert.Success(copyAllResponse);

            var destinationTitlesAfterCopyAll = await GetTitleListTitlesAsync(destinationListId);
            Assert.AreEqual(1, destinationTitlesAfterCopyAll.Count(t => t.LocalControlNumber == Settings.BibId), "Copy-all should place the generated title in the disposable destination list.");

            var deleteAllCopiedTitlesResponse = await Papi.PatronTitleListDeleteAllTitlesAsync(Settings.PatronBarcode, destinationListId, Settings.PatronPin);
            PapiIntegrationAssert.Success(deleteAllCopiedTitlesResponse);

            var copyResponse = await Papi.PatronTitleListCopyTitleAsync(Settings.PatronBarcode, sourceListId, addedTitle.Position, destinationListId, Settings.PatronPin);
            PapiIntegrationAssert.Success(copyResponse);

            var destinationTitlesAfterCopy = await GetTitleListTitlesAsync(destinationListId);
            Assert.AreEqual(1, destinationTitlesAfterCopy.Count(t => t.LocalControlNumber == Settings.BibId), "Copying the generated title should place it in the disposable destination list.");

            var moveResponse = await Papi.PatronTitleListMoveTitleAsync(Settings.PatronBarcode, sourceListId, addedTitle.Position, destinationListId, Settings.PatronPin);
            PapiIntegrationAssert.Success(moveResponse);

            var sourceTitlesAfterMove = await GetTitleListTitlesAsync(sourceListId);
            Assert.IsFalse(sourceTitlesAfterMove.Any(t => t.LocalControlNumber == Settings.BibId), "Moving the generated title should remove it from the disposable source list.");

            var destinationTitlesAfterMove = await GetTitleListTitlesAsync(destinationListId);
            var copiedOrMovedTitle = destinationTitlesAfterMove.FirstOrDefault(t => t.LocalControlNumber == Settings.BibId);
            Assert.IsNotNull(copiedOrMovedTitle, "The disposable destination list should contain generated title-list content before delete coverage runs.");

            var deleteTitleResponse = await Papi.PatronTitleListDeleteTitleAsync(Settings.PatronBarcode, destinationListId, copiedOrMovedTitle!.Position, Settings.PatronPin);
            PapiIntegrationAssert.Success(deleteTitleResponse);

            var deleteAllTitlesResponse = await Papi.PatronTitleListDeleteAllTitlesAsync(Settings.PatronBarcode, destinationListId, Settings.PatronPin);
            PapiIntegrationAssert.Success(deleteAllTitlesResponse);
        }
        catch (Exception ex)
        {
            testFailure = ExceptionDispatchInfo.Capture(ex);
        }
        finally
        {
            await cleanup.RunAsync(testFailure?.SourceException);
        }

        testFailure?.Throw();
    }

    private async Task<int> CreateDisposableTitleListAsync(string uniqueListName, IntegrationCleanup cleanup)
    {
        var createResponse = await Papi.PatronAccountCreateTitleListAsync(Settings.PatronBarcode, uniqueListName, Settings.PatronPin);
        PapiIntegrationAssert.ExactZeroSuccess(createResponse);

        cleanup.Add($"delete generated title list '{uniqueListName}'", async () =>
        {
            var listId = await FindUniqueGeneratedTitleListIdAsync(uniqueListName);
            if (!listId.HasValue)
            {
                Assert.Fail($"Cleanup could not find the generated title list '{uniqueListName}' by exact name. Manual cleanup may be required.");
            }

            var deleteResponse = await Papi.PatronAccountDeleteTitleListAsync(Settings.PatronBarcode, listId.Value, Settings.PatronPin);
            PapiIntegrationAssert.Success(deleteResponse);
        });

        var createdListId = await FindUniqueGeneratedTitleListIdAsync(uniqueListName);
        Assert.IsTrue(createdListId.HasValue, $"The generated title list '{uniqueListName}' should be visible before cleanup.");
        return createdListId.Value;
    }

    private async Task<IReadOnlyCollection<PatronTitleListTitleRow>> GetTitleListTitlesAsync(int listId)
    {
        var response = await Papi.PatronTitleListGetTitlesAsync(Settings.PatronBarcode, listId, password: Settings.PatronPin);
        var data = PapiIntegrationAssert.Success(response);
        return data.PatronTitleListTitleRows;
    }

    private async Task<int?> FindUniqueGeneratedTitleListIdAsync(string uniqueListName)
    {
        var getResponse = await Papi.PatronAccountGetTitleListsAsync(Settings.PatronBarcode, Settings.PatronPin);
        var getData = PapiIntegrationAssert.Success(getResponse);
        var matchingLists = getData.PatronAccountTitleListsRows
            .Where(l => string.Equals(l.RecordStoreName, uniqueListName, StringComparison.Ordinal))
            .ToList();

        return matchingLists.Count == 1 ? matchingLists[0].RecordStoreId : null;
    }

    [TestMethod]
    public void PatronAccountCreditSurface_RequiresReliableCleanupPathAndIsDisabledByDefault()
    {
        DocumentScenarioDependentPlaceholder(
            "PatronAccountCreateCreditAsync and PatronAccountDepositCreditAsync",
            "these endpoints mutate durable patron account state. An executable account-credit workflow must first create a uniquely noted tiny credit or deposit, read the generated patron account row, identify that exact generated transaction or credit row, and reverse it with a reliable cleanup API such as refund or void. The current client surface does not prove a safe end-to-end cleanup path for generated credit/deposit artifacts, so no live API call is made",
            "IntegrationTestOptions:EnableMutatingIntegrationTests=true, IntegrationTestOptions:EnableStaffProtectedTests=true, disposable patron credentials, staff override credentials, and response models/API methods that can uniquely identify and reverse the generated account artifact",
            "remain inconclusive until generated account-credit artifacts can be uniquely identified and reversed without leaving durable patron account rows");
    }
}
