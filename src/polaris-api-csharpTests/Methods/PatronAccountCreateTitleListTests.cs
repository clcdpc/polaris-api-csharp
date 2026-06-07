using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

#pragma warning disable CS8602 // Dereference of a possibly null reference.

namespace Clc.Polaris.Api.Tests
{
    [TestClass]
    public sealed class PatronAccountCreateTitleListTests : IntegrationTestBase
    {
        [TestMethod]
        [MutatingIntegrationCategory]
        [DoNotParallelize]
        public async Task PatronAccountCreateTitleList_CreatedListCanBeFoundAndDeleted()
        {
            var listName = CreateUniqueTestArtifactText(Settings.PatronListName);

            var createResponse = await Papi.PatronAccountCreateTitleListAsync(Settings.PatronBarcode, listName, Settings.PatronPin);
            Assert.AreEqual(0, createResponse.Data.PAPIErrorCode);

            var getResponse = await Papi.PatronAccountGetTitleListsAsync(Settings.PatronBarcode, Settings.PatronPin);
            var list = getResponse.Data.PatronAccountTitleListsRows.FirstOrDefault(l => l.RecordStoreName == listName);

            Assert.IsNotNull(list, $"Expected to find uniquely named title list '{listName}'.");

            var deleteResponse = await Papi.PatronAccountDeleteTitleListAsync(Settings.PatronBarcode, list.RecordStoreId, Settings.PatronPin);
            Assert.IsTrue(deleteResponse.Data.PAPIErrorCode == 0);
        }
    }
}
