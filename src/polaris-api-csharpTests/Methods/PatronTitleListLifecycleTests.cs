namespace Clc.Polaris.Api.Tests
{
    [TestClass]
    public sealed class PatronTitleListLifecycleTests : IntegrationTestBase
    {
        [TestMethod]
        [MutatingIntegrationTest]
        [DoNotParallelize]
        public async Task PatronTitleListLifecycle_CanCreatePopulateCopyMoveClearAndDeleteLists()
        {
            var sourceListName = CreateUniqueTestArtifactText(Settings.PatronListName, maxLength: 80);
            var destinationListName = CreateUniqueTestArtifactText(Settings.PatronListName, maxLength: 80);
            PatronAccountTitleListsRow? sourceList = null;
            PatronAccountTitleListsRow? destinationList = null;

            try
            {
                sourceList = await CreateTitleListAsync(sourceListName);
                destinationList = await CreateTitleListAsync(destinationListName);

                var titleLists = await Papi.PatronAccountGetTitleListsAsync(Settings.PatronBarcode, Settings.PatronPin, TestContext.CancellationToken);
                Assert.AreEqual(0, titleLists.Data.PAPIErrorCode);
                Assert.IsNotNull(titleLists.Data.PatronAccountTitleListsRows.SingleOrDefault(list => list.RecordStoreId == sourceList.RecordStoreId));
                Assert.IsNotNull(titleLists.Data.PatronAccountTitleListsRows.SingleOrDefault(list => list.RecordStoreId == destinationList.RecordStoreId));

                if (Settings.LocalControlNumber is not > 0)
                {
                    return;
                }

                var addTitle = await Papi.PatronTitleListAddTitleAsync(Settings.PatronBarcode, sourceList.RecordStoreId, Settings.LocalControlNumber.Value, Settings.PatronPin, TestContext.CancellationToken);
                Assert.AreEqual(0, addTitle.Data.PAPIErrorCode);

                var sourceTitles = await Papi.PatronTitleListGetTitlesAsync(Settings.PatronBarcode, sourceList.RecordStoreId, password: Settings.PatronPin, cancellationToken: TestContext.CancellationToken);
                Assert.AreEqual(0, sourceTitles.Data.PAPIErrorCode);
                Assert.IsNotEmpty(sourceTitles.Data.PatronTitleListTitleRows);
                Assert.IsNotNull(sourceTitles.Data.PatronTitleListTitleRows.SingleOrDefault(row => row.LocalControlNumber == Settings.LocalControlNumber.Value));

                var sourcePosition = sourceTitles.Data.PatronTitleListTitleRows.First(row => row.LocalControlNumber == Settings.LocalControlNumber.Value).Position;
                var copyTitle = await Papi.PatronTitleListCopyTitleAsync(Settings.PatronBarcode, sourceList.RecordStoreId, sourcePosition, destinationList.RecordStoreId, Settings.PatronPin, TestContext.CancellationToken);
                Assert.AreEqual(0, copyTitle.Data.PAPIErrorCode);

                var copyAllTitles = await Papi.PatronTitleListCopyAllTitlesAsync(Settings.PatronBarcode, sourceList.RecordStoreId, destinationList.RecordStoreId, Settings.PatronPin, TestContext.CancellationToken);
                Assert.AreEqual(0, copyAllTitles.Data.PAPIErrorCode);

                var destinationTitles = await Papi.PatronTitleListGetTitlesAsync(Settings.PatronBarcode, destinationList.RecordStoreId, password: Settings.PatronPin, cancellationToken: TestContext.CancellationToken);
                Assert.AreEqual(0, destinationTitles.Data.PAPIErrorCode);
                Assert.IsNotEmpty(destinationTitles.Data.PatronTitleListTitleRows);

                var destinationPosition = destinationTitles.Data.PatronTitleListTitleRows.First().Position;
                var moveTitle = await Papi.PatronTitleListMoveTitleAsync(Settings.PatronBarcode, destinationList.RecordStoreId, destinationPosition, sourceList.RecordStoreId, Settings.PatronPin, TestContext.CancellationToken);
                Assert.AreEqual(0, moveTitle.Data.PAPIErrorCode);

                var refreshedSourceTitles = await Papi.PatronTitleListGetTitlesAsync(Settings.PatronBarcode, sourceList.RecordStoreId, password: Settings.PatronPin, cancellationToken: TestContext.CancellationToken);
                Assert.AreEqual(0, refreshedSourceTitles.Data.PAPIErrorCode);
                Assert.IsNotEmpty(refreshedSourceTitles.Data.PatronTitleListTitleRows);

                var deleteTitle = await Papi.PatronTitleListDeleteTitleAsync(Settings.PatronBarcode, sourceList.RecordStoreId, refreshedSourceTitles.Data.PatronTitleListTitleRows.First().Position, Settings.PatronPin, TestContext.CancellationToken);
                Assert.AreEqual(0, deleteTitle.Data.PAPIErrorCode);

                var deleteAllTitles = await Papi.PatronTitleListDeleteAllTitlesAsync(Settings.PatronBarcode, destinationList.RecordStoreId, Settings.PatronPin, TestContext.CancellationToken);
                Assert.AreEqual(0, deleteAllTitles.Data.PAPIErrorCode);
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
    }
}
