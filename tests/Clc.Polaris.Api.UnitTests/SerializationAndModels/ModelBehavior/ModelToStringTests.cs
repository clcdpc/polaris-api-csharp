namespace Clc.Polaris.Api.UnitTests.SerializationAndModels.ModelBehavior
{
    [TestClass]
    [UnitTest]
    public sealed class ModelToStringTests
    {
        [TestMethod]
        public void BibsPostResult_ToString_ReturnsImportJobId()
        {
            var result = new BibsPostResult { ImportJobID = 1234 };

            Assert.AreEqual("1234", result.ToString());
        }

        [TestMethod]
        public void BibsPostResult_ToString_ReturnsEmptyStringForNullImportJobId()
        {
            var result = new BibsPostResult { ImportJobID = null };

            Assert.AreEqual(string.Empty, result.ToString());
        }

        [TestMethod]
        public void HoldRequestCreateParams_ToString_UsesStableRequestShapeFields()
        {
            var parameters = new HoldRequestCreateParams(patronId: 10, bibId: 20, pickupBranchId: 30, requestingOrgId: 40)
            {
                ItemBarcode = "item-barcode",
                PatronNotes = "patron note"
            };

            Assert.AreEqual("PatronID=10, BibID=20, PickupOrgID=30, RequestingOrgID=40", parameters.ToString());
        }

        [TestMethod]
        public void BibGetByTypeResult_ToString_JoinsRowsWithCarriageReturnLineFeeds()
        {
            var result = new BibGetByTypeResult
            {
                BibGetByTypeRows =
                [
                    new() { ElementID = 1, Label = "Title", Value = "First" },
                    new() { ElementID = 2, Label = "Author", Value = "Second" }
                ]
            };

            Assert.AreEqual("1 - Title - First\r\n2 - Author - Second", result.ToString());
        }

        [TestMethod]
        public void BibGetByTypeRow_ToString_HandlesNullStrings()
        {
            var row = new BibGetByTypeRow { ElementID = 9 };

            Assert.AreEqual("9 -  - ", row.ToString());
        }

        [TestMethod]
        public void BibGetByTypeResult_ToString_HandlesNullRowsCollection()
        {
            var result = new BibGetByTypeResult { BibGetByTypeRows = null! };

            Assert.AreEqual(string.Empty, result.ToString());
        }

        [TestMethod]
        public void PapiResult_ToString_UsesInheritedCommonResponseBehavior()
        {
            var result = new PAPIResult { PAPIErrorCode = 7, ErrorMessage = "failed" };

            Assert.AreEqual("7 - failed", result.ToString());
        }

        [TestMethod]
        public void PatronAuthenticationResult_ToString_DoesNotExposeSecrets()
        {
            var result = new PatronAuthenticationResult
            {
                PatronID = 123,
                AccessToken = "secret-token",
                AccessSecret = "secret-access"
            };

            var text = result.ToString();

            Assert.AreEqual("123", text);
            Assert.DoesNotContain("secret-token", text);
            Assert.DoesNotContain("secret-access", text);
        }

        [TestMethod]
        public void CollectionsGetResult_ToString_JoinsRowsWhoseRowsAlreadyFormatSafely()
        {
            var result = new CollectionsGetResult
            {
                CollectionsRows =
                [
                    new() { ID = 1, Abbreviation = "FIC", Name = "Fiction" },
                    new() { ID = 2, Abbreviation = "NF", Name = "Nonfiction" }
                ]
            };

            Assert.AreEqual("1 - FIC - Fiction\r\n2 - NF - Nonfiction", result.ToString());
        }

        [TestMethod]
        public void CollectionsGetResult_ToString_HandlesNullRowsCollection()
        {
            var result = new CollectionsGetResult { CollectionsRows = null! };

            Assert.AreEqual(string.Empty, result.ToString());
        }

        [TestMethod]
        public void MarcTypeOfMaterialsGetResult_ToString_JoinsExistingRowOutput()
        {
            var result = new MARCTypeOfMaterialsGetResult
            {
                MARCTypeOfMaterialsRows =
                [
                    new() { MARCTypeOfMaterialId = 7, SearchCode = "BK", Description = "Books" },
                    new() { MARCTypeOfMaterialId = 8, SearchCode = "VM", Description = "Visual materials" }
                ]
            };

            Assert.AreEqual("7 - BK - Books\r\n8 - VM - Visual materials", result.ToString());
        }

        [TestMethod]
        public void ItemStatusesGetResult_ToString_JoinsRows()
        {
            var result = new ItemStatusesGetResult
            {
                ItemStatusesRows =
                [
                    new() { ItemStatusId = 1, Name = "In", Description = "In Library" },
                    new() { ItemStatusId = 2, Name = "Out", Description = "Checked Out" }
                ]
            };

            Assert.AreEqual("1 - In - In Library\r\n2 - Out - Checked Out", result.ToString());
        }

        [TestMethod]
        public void ItemStatusRow_ToString_HandlesNullStrings()
        {
            var row = new ItemStatusRow { ItemStatusId = 5 };

            Assert.AreEqual("5 -  - ", row.ToString());
        }

        [TestMethod]
        public void RemoteStorageItemsGetRow_ToString_IncludesItemOnlyBarcode()
        {
            var row = new RemoteStorageItemsGetRow
            {
                ItemRecordID = 11,
                Barcode = "item-barcode",
                BibliographicRecordID = 22,
                BrowseTitle = "Item title",
                MaterialType = "Book",
                Collection = "Stacks",
                ShelfLocation = "Main",
                CallNumber = "QA 1"
            };

            Assert.AreEqual("11 - item-barcode - 22 - Item title - Book - Stacks - Main - QA 1", row.ToString());
        }

        [TestMethod]
        public void RemoteStorageItemsGetResult_ToString_JoinsRowsAndHandlesNullCollection()
        {
            var withRows = new RemoteStorageItemsGetResult
            {
                RemoteStorageItemsGetRows =
                [
                    new() { ItemRecordID = 11, Barcode = "item-1", BibliographicRecordID = 22, BrowseTitle = "Title 1" },
                    new() { ItemRecordID = 12, Barcode = "item-2", BibliographicRecordID = 23, BrowseTitle = "Title 2" }
                ]
            };
            var withoutRows = new RemoteStorageItemsGetResult { RemoteStorageItemsGetRows = null! };

            Assert.AreEqual("11 - item-1 - 22 - Title 1 -  -  -  - \r\n12 - item-2 - 23 - Title 2 -  -  -  - ", withRows.ToString());
            Assert.AreEqual(string.Empty, withoutRows.ToString());
        }

        [TestMethod]
        public void SAMobilePhoneCarriersGetRow_ToString_ExcludesEmailToSmsAddress()
        {
            var row = new SAMobilePhoneCarriersGetRow
            {
                CarrierID = 3,
                CarrierName = "Carrier",
                Email2SMSEmailAddress = "private-sms-domain.example"
            };

            var text = row.ToString();

            Assert.AreEqual("3 - Carrier", text);
            Assert.DoesNotContain("private-sms-domain.example", text);
        }

        [TestMethod]
        public void PatronItemsOutGetResult_ToString_UsesCommonResponseWithoutPatronScopedItemRows()
        {
            var result = new PatronItemsOutGetResult
            {
                PAPIErrorCode = 0,
                ErrorMessage = "OK",
                PatronItemsOutGetRows =
                [
                    new() { ItemID = 99, Barcode = "patron-scoped-item-barcode", Title = "Patron scoped title" }
                ]
            };

            var text = result.ToString();

            Assert.AreEqual("0 - OK", text);
            Assert.DoesNotContain("patron-scoped-item-barcode", text);
            Assert.DoesNotContain("Patron scoped title", text);
        }

    }
}
