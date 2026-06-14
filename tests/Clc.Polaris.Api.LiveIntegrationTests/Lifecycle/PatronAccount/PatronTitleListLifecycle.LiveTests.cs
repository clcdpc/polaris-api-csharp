namespace Clc.Polaris.Api.LiveIntegrationTests
{
    [TestClass]
    public sealed class PatronTitleListLifecycleTests : IntegrationTestBase
    {
        [TestMethod]
        [LifecycleLiveTest]
        [DoNotParallelize]
        public async Task PatronTitleListLifecycle_CanCreateReadAndDeleteList()
        {
            var listName = CreateUniqueTestArtifactText(Settings.PatronListName, maxLength: 80);
            PatronAccountTitleListsRow? createdList = null;

            try
            {
                createdList = await CreateTitleListAsync(listName);

                var titleLists = await Papi.PatronAccountGetTitleListsAsync(Settings.PatronBarcode, Settings.PatronPin, TestContext.CancellationToken);
                Assert.AreEqual(0, titleLists.Data.PAPIErrorCode);
                Assert.IsNotNull(titleLists.Data.PatronAccountTitleListsRows.SingleOrDefault(list => list.RecordStoreId == createdList.RecordStoreId));

                var deleteResponse = await Papi.PatronAccountDeleteTitleListAsync(Settings.PatronBarcode, createdList.RecordStoreId, Settings.PatronPin, TestContext.CancellationToken);
                Assert.AreEqual(0, deleteResponse.Data.PAPIErrorCode);

                var afterDelete = await Papi.PatronAccountGetTitleListsAsync(Settings.PatronBarcode, Settings.PatronPin, TestContext.CancellationToken);
                Assert.AreEqual(0, afterDelete.Data.PAPIErrorCode);
                Assert.IsNull(afterDelete.Data.PatronAccountTitleListsRows.SingleOrDefault(list => list.RecordStoreId == createdList.RecordStoreId));

                createdList = null;
            }
            finally
            {
                if (createdList != null)
                {
                    _ = await Papi.PatronAccountDeleteTitleListAsync(Settings.PatronBarcode, createdList.RecordStoreId, Settings.PatronPin, TestContext.CancellationToken);
                }
            }
        }

        [TestMethod]
        [LifecycleLiveTest]
        [DoNotParallelize]
        public async Task PatronTitleListLifecycle_CanPopulateCopyMoveClearAndDeleteLists()
        {
            var localControlNumber = RequirePositiveSetting(Settings.LocalControlNumber, nameof(Settings.LocalControlNumber));
            var sourceListName = CreateUniqueTestArtifactText(Settings.PatronListName, maxLength: 80);
            var destinationListName = CreateUniqueTestArtifactText(Settings.PatronListName, maxLength: 80);
            PatronAccountTitleListsRow? sourceList = null;
            PatronAccountTitleListsRow? destinationList = null;

            try
            {
                sourceList = await CreateTitleListAsync(sourceListName);
                destinationList = await CreateTitleListAsync(destinationListName);

                var addTitle = await Papi.PatronTitleListAddTitleAsync(Settings.PatronBarcode, sourceList.RecordStoreId, localControlNumber, Settings.PatronPin, TestContext.CancellationToken);
                Assert.AreEqual(0, addTitle.Data.PAPIErrorCode);

                var sourceTitle = await GetSingleTitleListTitleAsync(sourceList.RecordStoreId, localControlNumber, "Expected source list to contain the configured local control number after add.");

                var copyTitle = await Papi.PatronTitleListCopyTitleAsync(Settings.PatronBarcode, sourceList.RecordStoreId, sourceTitle.Position, destinationList.RecordStoreId, Settings.PatronPin, TestContext.CancellationToken);
                Assert.AreEqual(0, copyTitle.Data.PAPIErrorCode);

                _ = await GetSingleTitleListTitleAsync(destinationList.RecordStoreId, localControlNumber, "Expected destination list to contain the configured local control number after copy.");

                var clearDestination = await Papi.PatronTitleListDeleteAllTitlesAsync(Settings.PatronBarcode, destinationList.RecordStoreId, Settings.PatronPin, TestContext.CancellationToken);
                Assert.AreEqual(0, clearDestination.Data.PAPIErrorCode);

                var copyAllTitles = await Papi.PatronTitleListCopyAllTitlesAsync(Settings.PatronBarcode, sourceList.RecordStoreId, destinationList.RecordStoreId, Settings.PatronPin, TestContext.CancellationToken);
                Assert.AreEqual(0, copyAllTitles.Data.PAPIErrorCode);

                var sourceTitleToDelete = await GetSingleTitleListTitleAsync(sourceList.RecordStoreId, localControlNumber, "Expected source list to still contain the configured local control number after copy-all.");

                var deleteTitle = await Papi.PatronTitleListDeleteTitleAsync(Settings.PatronBarcode, sourceList.RecordStoreId, sourceTitleToDelete.Position, Settings.PatronPin, TestContext.CancellationToken);
                Assert.AreEqual(0, deleteTitle.Data.PAPIErrorCode);

                var destinationTitleToMove = await GetSingleTitleListTitleAsync(destinationList.RecordStoreId, localControlNumber, "Expected destination list to contain the configured local control number before move.");

                var moveTitle = await Papi.PatronTitleListMoveTitleAsync(Settings.PatronBarcode, destinationList.RecordStoreId, destinationTitleToMove.Position, sourceList.RecordStoreId, Settings.PatronPin, TestContext.CancellationToken);
                Assert.AreEqual(0, moveTitle.Data.PAPIErrorCode);

                _ = await GetSingleTitleListTitleAsync(sourceList.RecordStoreId, localControlNumber, "Expected source list to contain the configured local control number after move.");

                var deleteAllSourceTitles = await Papi.PatronTitleListDeleteAllTitlesAsync(Settings.PatronBarcode, sourceList.RecordStoreId, Settings.PatronPin, TestContext.CancellationToken);
                Assert.AreEqual(0, deleteAllSourceTitles.Data.PAPIErrorCode);

                var deleteAllDestinationTitles = await Papi.PatronTitleListDeleteAllTitlesAsync(Settings.PatronBarcode, destinationList.RecordStoreId, Settings.PatronPin, TestContext.CancellationToken);
                Assert.AreEqual(0, deleteAllDestinationTitles.Data.PAPIErrorCode);

                var deleteSourceList = await Papi.PatronAccountDeleteTitleListAsync(Settings.PatronBarcode, sourceList.RecordStoreId, Settings.PatronPin, TestContext.CancellationToken);
                Assert.AreEqual(0, deleteSourceList.Data.PAPIErrorCode);
                sourceList = null;

                var deleteDestinationList = await Papi.PatronAccountDeleteTitleListAsync(Settings.PatronBarcode, destinationList.RecordStoreId, Settings.PatronPin, TestContext.CancellationToken);
                Assert.AreEqual(0, deleteDestinationList.Data.PAPIErrorCode);
                destinationList = null;
            }
            finally
            {
                if (sourceList != null)
                {
                    _ = await Papi.PatronAccountDeleteTitleListAsync(Settings.PatronBarcode, sourceList.RecordStoreId, Settings.PatronPin, TestContext.CancellationToken);
                }

                if (destinationList != null)
                {
                    _ = await Papi.PatronAccountDeleteTitleListAsync(Settings.PatronBarcode, destinationList.RecordStoreId, Settings.PatronPin, TestContext.CancellationToken);
                }
            }
        }

        private async Task<PatronAccountTitleListsRow> CreateTitleListAsync(string listName)
        {
            for (var attempt = 0; attempt < 2; attempt++)
            {
                var candidateName = attempt == 0 ? listName : CreateUniqueTestArtifactText(Settings.PatronListName, maxLength: 80);
                var createResponse = await Papi.PatronAccountCreateTitleListAsync(Settings.PatronBarcode, candidateName, Settings.PatronPin, TestContext.CancellationToken);

                if (createResponse.Data.PAPIErrorCode != 0 && attempt == 0)
                {
                    continue;
                }

                Assert.AreEqual(0, createResponse.Data.PAPIErrorCode);

                var getResponse = await Papi.PatronAccountGetTitleListsAsync(Settings.PatronBarcode, Settings.PatronPin, TestContext.CancellationToken);
                Assert.AreEqual(0, getResponse.Data.PAPIErrorCode);

                var list = getResponse.Data.PatronAccountTitleListsRows.SingleOrDefault(row => row.RecordStoreName == candidateName);
                Assert.IsNotNull(list, $"Expected created title list '{candidateName}' to be returned by PatronAccountGetTitleListsAsync.");

                return list;
            }

            Assert.Fail("Expected title-list creation retry loop to return a created title list.");
            return null!;
        }

        private async Task<PatronTitleListTitleRow> GetSingleTitleListTitleAsync(int recordStoreId, int localControlNumber, string failureMessage)
        {
            var response = await Papi.PatronTitleListGetTitlesAsync(Settings.PatronBarcode, recordStoreId, password: Settings.PatronPin, cancellationToken: TestContext.CancellationToken);
            Assert.AreEqual(0, response.Data.PAPIErrorCode);

            var matches = response.Data.PatronTitleListTitleRows.Where(row => row.LocalControlNumber == localControlNumber).ToArray();
            Assert.HasCount(1, matches, failureMessage);

            return matches[0];
        }
    }
}