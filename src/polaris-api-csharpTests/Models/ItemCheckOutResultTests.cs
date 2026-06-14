namespace Clc.Polaris.Api.Tests.Models
{
    [TestClass]
    [UnitTest]
    public sealed class ItemCheckOutResultTests
    {
        [TestMethod]
        public void ItemCheckOutResult_BlockReasons_DecodesKnownFlags()
        {
            var result = new ItemCheckOutResult
            {
                PatronBlockFlags = (int)CheckoutPatronBlockReasons.LibraryAssignedBlock | (int)CheckoutPatronBlockReasons.FreeTextBlock,
                ItemBlockFlags = (int)CheckoutItemBlockReasons.ItemFreeTextBlock | (int)CheckoutItemBlockReasons.ItemMaterialTypeBlocked,
                RenewalBlockFlags = (int)CheckoutRenewalBlockReasons.ItemAlreadyOverdue | (int)CheckoutRenewalBlockReasons.ItemRenewalLimitReached,
            };

            var expectedPatronReasons = new[] { CheckoutPatronBlockReasons.LibraryAssignedBlock, CheckoutPatronBlockReasons.FreeTextBlock, };
            var expectedItemReasons = new[] { CheckoutItemBlockReasons.ItemFreeTextBlock, CheckoutItemBlockReasons.ItemMaterialTypeBlocked, };
            var expectedRenewalReasons = new[] { CheckoutRenewalBlockReasons.ItemAlreadyOverdue, CheckoutRenewalBlockReasons.ItemRenewalLimitReached, };

            AssertDecodedReasons(result.PatronBlockReasons, expectedPatronReasons, result.PatronBlockReasonDescriptions);
            AssertDecodedReasons(result.ItemBlockReasons, expectedItemReasons, result.ItemBlockReasonDescriptions);
            AssertDecodedReasons(result.RenewalBlockReasons, expectedRenewalReasons, result.RenewalBlockReasonDescriptions);
        }

        private static void AssertDecodedReasons<TReason>(TReason actualReasons, TReason[] expectedReasons, IReadOnlyList<string> actualDescriptions) where TReason : struct, Enum
        {
            var actualValue = Convert.ToInt64(actualReasons);

            foreach (var expectedReason in expectedReasons)
            {
                var expectedValue = Convert.ToInt64(expectedReason);

                Assert.AreEqual(expectedValue, actualValue & expectedValue, $"Expected decoded reasons to include {typeof(TReason).Name}.{expectedReason}.");
            }

            Assert.HasCount(expectedReasons.Length, actualDescriptions);
            Assert.IsTrue(actualDescriptions.All(description => !string.IsNullOrWhiteSpace(description)), "Expected all decoded reason descriptions to be populated.");
            Assert.HasCount(actualDescriptions.Count, actualDescriptions.Distinct().ToArray());
        }

        [TestMethod]
        public void ItemCheckOutResult_BlockReasons_ReturnEmptyDescriptionsForNoFlags()
        {
            var result = new ItemCheckOutResult();

            Assert.AreEqual(CheckoutPatronBlockReasons.None, result.PatronBlockReasons);
            Assert.AreEqual(CheckoutItemBlockReasons.None, result.ItemBlockReasons);
            Assert.AreEqual(CheckoutRenewalBlockReasons.None, result.RenewalBlockReasons);
            Assert.IsEmpty(result.PatronBlockReasonDescriptions);
            Assert.IsEmpty(result.ItemBlockReasonDescriptions);
            Assert.IsEmpty(result.RenewalBlockReasonDescriptions);
        }

        [TestMethod]
        public void ItemCheckOutResult_BlockReasons_PreserveUnknownFlags()
        {
            var result = new ItemCheckOutResult
            {
                PatronBlockFlags = 65536,
                ItemBlockFlags = 131072,
                RenewalBlockFlags = 2097152,
            };

            Assert.AreEqual(65536, result.UnknownPatronBlockFlags);
            Assert.AreEqual(131072, result.UnknownItemBlockFlags);
            Assert.AreEqual(2097152, result.UnknownRenewalBlockFlags);
            Assert.IsEmpty(result.PatronBlockReasonDescriptions);
            Assert.IsEmpty(result.ItemBlockReasonDescriptions);
            Assert.IsEmpty(result.RenewalBlockReasonDescriptions);
        }

        [TestMethod]
        public void ItemCheckOutResult_BlockReasons_DecodesDocumentedFailedItemCheckoutExample()
        {
            var result = new ItemCheckOutResult
            {
                PAPIErrorCode = -6112,
                ItemBlockFlags = 270336,
            };

            Assert.IsTrue(result.ItemBlockReasons.HasFlag(CheckoutItemBlockReasons.ItemFreeTextBlock));
            Assert.IsTrue(result.ItemBlockReasons.HasFlag(CheckoutItemBlockReasons.ItemMaterialTypeBlocked));
            Assert.AreEqual(0, result.UnknownItemBlockFlags);
            Assert.HasCount(2, result.ItemBlockReasonDescriptions);
            Assert.IsTrue(result.ItemBlockReasonDescriptions.All(description => !string.IsNullOrWhiteSpace(description)));
        }
    }
}