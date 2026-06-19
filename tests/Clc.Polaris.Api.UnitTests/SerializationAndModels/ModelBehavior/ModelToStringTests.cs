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


        [TestMethod]
        public void ItemCheckInResult_ToString_ExcludesPatronBarcodeAndTrappingPatronFields()
        {
            var result = new ItemCheckInResult
            {
                ItemRecordID = 100,
                ItemBarcode = "item-barcode",
                Title = "Item title",
                ItemStatusID = 3,
                PreviousItemStatusID = 2,
                ShelfLocation = "Stacks",
                CallNumber = "QA 1",
                PatronBarcode = "patron-barcode",
                Comment = "private comment",
                HoldData = new ItemCheckInHoldData
                {
                    TrappingPatronBarcode = "trapping-barcode",
                    TrappingPatronName = "Patron Name",
                    PickupBranchName = "Main",
                    PickupArea = "Desk"
                }
            };

            var text = result.ToString();

            Assert.AreEqual("100 - item-barcode - Item title - 3 - 2 - Stacks - QA 1", text);
            Assert.DoesNotContain("patron-barcode", text);
            Assert.DoesNotContain("private comment", text);
            Assert.DoesNotContain("trapping-barcode", text);
            Assert.DoesNotContain("Patron Name", text);
        }

        [TestMethod]
        public void ItemCheckInHoldData_ToString_ExcludesTrappingPatronFields()
        {
            var holdData = new ItemCheckInHoldData
            {
                TrappingPatronBarcode = "trapping-barcode",
                TrappingPatronName = "Patron Name",
                PickupBranchName = "Main",
                PickupArea = "Desk"
            };

            var text = holdData.ToString();

            Assert.AreEqual("Main - Desk", text);
            Assert.DoesNotContain("trapping-barcode", text);
            Assert.DoesNotContain("Patron Name", text);
        }

        [TestMethod]
        public void ItemCheckOutResult_ToString_ExcludesPatronBlockFieldsAndDescriptions()
        {
            var dueDate = new DateTime(2026, 6, 18, 12, 30, 0, DateTimeKind.Utc);
            var result = new ItemCheckOutResult
            {
                ItemRecordID = 200,
                Title = "Checkout title",
                DueDate = dueDate,
                MaterialTypeID = 4,
                ItemBlockFlags = 8,
                RenewalBlockFlags = 16,
                PatronBlockFlags = 32
            };

            var text = result.ToString();

            Assert.AreEqual("200 - Checkout title - 2026-06-18T12:30:00.0000000Z - 4 - 8", text);
            Assert.DoesNotContain("16", text);
            Assert.DoesNotContain("32", text);
            Assert.DoesNotContain("Patron", text);
        }

        [TestMethod]
        public void BibHoldingsGetResult_ToString_JoinsRows()
        {
            var result = new BibHoldingsGetResult
            {
                BibHoldingsGetRows =
                [
                    new() { Barcode = "item-1", LocationName = "Main", CollectionName = "Stacks", CallNumber = "QA 1", ShelfLocation = "A", CircStatus = "In", MaterialType = "Book", DueDate = "2026-06-18" },
                    new() { Barcode = "item-2", LocationName = "Branch", CollectionName = "Media", CallNumber = "DVD 2", ShelfLocation = "B", CircStatus = "Out", MaterialType = "DVD", DueDate = "2026-06-19" }
                ]
            };

            Assert.AreEqual("item-1 - Main - Stacks - QA 1 - A - In - Book - 2026-06-18\r\nitem-2 - Branch - Media - DVD 2 - B - Out - DVD - 2026-06-19", result.ToString());
        }

        [TestMethod]
        public void ItemRenewResultWrapper_ToString_DelegatesToBodyRows()
        {
            var dueDate = new DateTime(2026, 6, 18, 12, 30, 0, DateTimeKind.Utc);
            var wrapper = new ItemRenewResultWrapper
            {
                PAPIErrorCode = 0,
                ErrorMessage = "OK",
                ItemRenewResult = new ItemRenewResultBody
                {
                    BlockRows =
                    [
                        new() { ItemRecordID = 300, PAPIErrorType = 2, PolarisErrorCode = 42, ErrorAllowOverride = true, ErrorDesc = "Renewal blocked" }
                    ],
                    DueDateRows =
                    [
                        new() { ItemRecordID = 301, DueDate = dueDate }
                    ]
                }
            };

            Assert.AreEqual("300 - 2 - 42 - True - Renewal blocked\r\n301 - 2026-06-18T12:30:00.0000000Z", wrapper.ToString());
        }

        [TestMethod]
        public void ItemsOutActionResult_ToString_DelegatesToRenewResultOrCommonResponse()
        {
            var withRenewResult = new ItemsOutActionResult
            {
                ItemRenewResult = new ItemRenewResultWrapper
                {
                    ItemRenewResult = new ItemRenewResultBody
                    {
                        BlockRows =
                        [
                            new() { ItemRecordID = 400, PAPIErrorType = 2, PolarisErrorCode = 99, ErrorAllowOverride = false, ErrorDesc = "No renewals" }
                        ]
                    }
                }
            };
            var withoutRenewResult = new ItemsOutActionResult { PAPIErrorCode = 5, ErrorMessage = "Failed" };

            Assert.AreEqual("400 - 2 - 99 - False - No renewals", withRenewResult.ToString());
            Assert.AreEqual("5 - Failed", withoutRenewResult.ToString());
        }

        [TestMethod]
        public void Requestpicklistrow_ToString_ExcludesPatronFieldsAndKeepsItemFields()
        {
            var row = new Requestpicklistrow
            {
                ItemRecordID = 500,
                ItemBarcode = "item-barcode",
                BrowseTitle = "Hold title",
                PatronFullName = "Patron Name",
                PatronBarcode = "patron-barcode",
                PatronID = 123,
                EmailAddress = "patron@example.org",
                AltEmailAddress = "alt@example.org",
                PhoneVoice1 = "555-0100",
                SMSAddress = "sms@example.org"
            };

            var text = row.ToString();

            Assert.AreEqual("500 - item-barcode - Hold title", text);
            Assert.DoesNotContain("Patron Name", text);
            Assert.DoesNotContain("patron-barcode", text);
            Assert.DoesNotContain("patron@example.org", text);
            Assert.DoesNotContain("alt@example.org", text);
            Assert.DoesNotContain("555-0100", text);
            Assert.DoesNotContain("sms@example.org", text);
        }


        [TestMethod]
        public void PatronTitleListAddTitleResult_ToString_ReturnsPositionAndRecordId()
        {
            var result = new PatronTitleListAddTitleResult { Position = 3, RecordID = 456 };

            Assert.AreEqual("3 - 456", result.ToString());
        }

        [TestMethod]
        public void PatronTitleListAddTitleData_ToString_ReturnsRecordStoreAndLocalControlNumber()
        {
            var data = new PatronTitleListAddTitleData { RecordStoreId = 12, LocalControlNumber = 345 };

            Assert.AreEqual("12 - 345", data.ToString());
        }

        [TestMethod]
        public void PatronTitleListCopyTitleData_ToString_ReturnsSourcePositionAndDestination()
        {
            var data = new PatronTitleListCopyTitleData { FromRecordStoreId = 12, FromPosition = 5, ToRecordStoreId = 34 };

            Assert.AreEqual("12 - 5 - 34", data.ToString());
        }

        [TestMethod]
        public void PatronTitleListCopyAllTitlesData_ToString_ReturnsSourceAndDestination()
        {
            var data = new PatronTitleListCopyAllTitlesData { FromRecordStoreId = 12, ToRecordStoreId = 34 };

            Assert.AreEqual("12 - 34", data.ToString());
        }

        [TestMethod]
        public void PatronTitleListMoveTitleData_ToString_ReturnsSourcePositionAndDestination()
        {
            var data = new PatronTitleListMoveTitleData { FromRecordStoreId = 12, FromPosition = 5, ToRecordStoreId = 34 };

            Assert.AreEqual("12 - 5 - 34", data.ToString());
        }

    }
}
