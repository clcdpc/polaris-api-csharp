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
    }
}
