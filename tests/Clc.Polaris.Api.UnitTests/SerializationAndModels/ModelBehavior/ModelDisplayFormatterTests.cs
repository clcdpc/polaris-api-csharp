using Clc.Polaris.Api.Internal;

namespace Clc.Polaris.Api.UnitTests.SerializationAndModels.ModelBehavior
{
    [TestClass]
    [UnitTest]
    public sealed class ModelDisplayFormatterTests
    {
        [TestMethod]
        public void MaskLeadingDigits_MasksAtLeastOneDigitAndPreservesDigitCount()
        {
            Assert.AreEqual("*", ModelDisplayFormatter.MaskLeadingDigits(1));
            Assert.AreEqual("*2", ModelDisplayFormatter.MaskLeadingDigits(12));
            Assert.AreEqual("*23", ModelDisplayFormatter.MaskLeadingDigits(123));
            Assert.AreEqual("*234", ModelDisplayFormatter.MaskLeadingDigits(1234));
            Assert.AreEqual("*2345", ModelDisplayFormatter.MaskLeadingDigits(12345));
            Assert.AreEqual("***4567", ModelDisplayFormatter.MaskLeadingDigits(1234567));
            Assert.AreEqual("*****6789", ModelDisplayFormatter.MaskLeadingDigits(123456789));
        }

        [TestMethod]
        public void MaskLeadingDigits_HandlesZero()
        {
            Assert.AreEqual("*", ModelDisplayFormatter.MaskLeadingDigits(0));
        }

        [TestMethod]
        public void MaskLeadingDigits_PreservesNegativeSignAndMasksDigits()
        {
            Assert.AreEqual("-*", ModelDisplayFormatter.MaskLeadingDigits(-1));
            Assert.AreEqual("-*2345", ModelDisplayFormatter.MaskLeadingDigits(-12345));
            Assert.AreEqual("-******3648", ModelDisplayFormatter.MaskLeadingDigits(int.MinValue));
        }

        [TestMethod]
        public void MaskLeadingDigits_UsesRequestedVisibleTrailingDigitCount()
        {
            Assert.AreEqual("****56", ModelDisplayFormatter.MaskLeadingDigits(123456, visibleTrailingDigits: 2));
            Assert.AreEqual("******", ModelDisplayFormatter.MaskLeadingDigits(123456, visibleTrailingDigits: 0));
            Assert.AreEqual("*23456", ModelDisplayFormatter.MaskLeadingDigits(123456, visibleTrailingDigits: 10));
        }

        [TestMethod]
        public void MaskLeadingDigits_ThrowsForNegativeVisibleTrailingDigits()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => ModelDisplayFormatter.MaskLeadingDigits(1234, visibleTrailingDigits: -1));
        }
    }
}