using System;
using Clc.Polaris.Api.Tests;
using Clc.Polaris.Api.Validation;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Clc.Polaris.Api.Validation.Tests
{
    [TestClass]
    [UnitTest]
    public class RequireTests
    {
        [TestMethod]
        public void Argument_NullObject_ThrowsArgumentNullException()
        {
            object? nullValue = null;

            var exception = Assert.ThrowsExactly<ArgumentNullException>(() => Require.Argument(nullValue));

            Assert.AreEqual(nameof(nullValue), exception.ParamName);
        }

        [TestMethod]
        public void Argument_NonNullObject_DoesNotThrow()
        {
            object notNullValue = new();

            Require.Argument(notNullValue);
        }

        [TestMethod]
        public void Argument_NullString_ThrowsArgumentNullException()
        {
            string? nullString = null;

            var exception = Assert.ThrowsExactly<ArgumentNullException>(() => Require.Argument(nullString));

            Assert.AreEqual(nameof(nullString), exception.ParamName);
        }

        [TestMethod]
        public void Argument_EmptyString_ThrowsArgumentException()
        {
            string emptyString = string.Empty;

            var exception = Assert.ThrowsExactly<ArgumentException>(() => Require.Argument(emptyString));

            Assert.AreEqual(nameof(emptyString), exception.ParamName);
        }

        [TestMethod]
        public void Argument_WhitespaceString_ThrowsArgumentException()
        {
            string whitespaceString = "   ";

            var exception = Assert.ThrowsExactly<ArgumentException>(() => Require.Argument(whitespaceString));

            Assert.AreEqual(nameof(whitespaceString), exception.ParamName);
        }

        [TestMethod]
        public void Argument_NonEmptyString_DoesNotThrow()
        {
            string nonEmptyString = "value";

            Require.Argument(nonEmptyString);
        }

        [TestMethod]
        public void Argument_ZeroInteger_DoesNotThrow()
        {
            int zeroValue = 0;

            Require.Argument(zeroValue);
        }

        [TestMethod]
        public void Positive_ZeroInteger_ThrowsArgumentOutOfRangeException()
        {
            int zeroValue = 0;

            var exception = Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => Require.Positive(zeroValue));

            Assert.AreEqual(nameof(zeroValue), exception.ParamName);
        }

        [TestMethod]
        public void Positive_NegativeInteger_ThrowsArgumentOutOfRangeException()
        {
            int negativeValue = -1;

            var exception = Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => Require.Positive(negativeValue));

            Assert.AreEqual(nameof(negativeValue), exception.ParamName);
        }

        [TestMethod]
        public void Positive_PositiveInteger_DoesNotThrow()
        {
            int positiveValue = 1;

            Require.Positive(positiveValue);
        }

        [TestMethod]
        public void Positive_NullNullableInteger_ThrowsArgumentNullException()
        {
            int? nullValue = null;

            var exception = Assert.ThrowsExactly<ArgumentNullException>(() => Require.Positive(nullValue));

            Assert.AreEqual(nameof(nullValue), exception.ParamName);
        }

        [TestMethod]
        public void Positive_ZeroNullableInteger_ThrowsArgumentOutOfRangeException()
        {
            int? zeroValue = 0;

            var exception = Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => Require.Positive(zeroValue));

            Assert.AreEqual(nameof(zeroValue), exception.ParamName);
        }

        [TestMethod]
        public void Positive_NegativeNullableInteger_ThrowsArgumentOutOfRangeException()
        {
            int? negativeValue = -1;

            var exception = Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => Require.Positive(negativeValue));

            Assert.AreEqual(nameof(negativeValue), exception.ParamName);
        }

        [TestMethod]
        public void Positive_PositiveNullableInteger_DoesNotThrow()
        {
            int? positiveValue = 1;

            Require.Positive(positiveValue);
        }

        [TestMethod]
        public void PositiveIfProvided_NullNullableInteger_DoesNotThrow()
        {
            int? nullValue = null;

            Require.PositiveIfProvided(nullValue);
        }

        [TestMethod]
        public void PositiveIfProvided_ZeroNullableInteger_ThrowsArgumentOutOfRangeException()
        {
            int? zeroValue = 0;

            var exception = Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => Require.PositiveIfProvided(zeroValue));

            Assert.AreEqual(nameof(zeroValue), exception.ParamName);
        }

        [TestMethod]
        public void PositiveIfProvided_NegativeNullableInteger_ThrowsArgumentOutOfRangeException()
        {
            int? negativeValue = -1;

            var exception = Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => Require.PositiveIfProvided(negativeValue));

            Assert.AreEqual(nameof(negativeValue), exception.ParamName);
        }

        [TestMethod]
        public void PositiveIfProvided_PositiveNullableInteger_DoesNotThrow()
        {
            int? positiveValue = 1;

            Require.PositiveIfProvided(positiveValue);
        }

        [TestMethod]
        public void NonNegative_NegativeInteger_ThrowsArgumentOutOfRangeException()
        {
            int negativeValue = -1;

            var exception = Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => Require.NonNegative(negativeValue));

            Assert.AreEqual(nameof(negativeValue), exception.ParamName);
        }

        [TestMethod]
        public void NonNegative_ZeroInteger_DoesNotThrow()
        {
            int zeroValue = 0;

            Require.NonNegative(zeroValue);
        }

        [TestMethod]
        public void NonNegative_PositiveInteger_DoesNotThrow()
        {
            int positiveValue = 1;

            Require.NonNegative(positiveValue);
        }

        [TestMethod]
        public void NonNegative_NullNullableInteger_ThrowsArgumentNullException()
        {
            int? nullValue = null;

            var exception = Assert.ThrowsExactly<ArgumentNullException>(() => Require.NonNegative(nullValue));

            Assert.AreEqual(nameof(nullValue), exception.ParamName);
        }

        [TestMethod]
        public void NonNegative_NegativeNullableInteger_ThrowsArgumentOutOfRangeException()
        {
            int? negativeValue = -1;

            var exception = Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => Require.NonNegative(negativeValue));

            Assert.AreEqual(nameof(negativeValue), exception.ParamName);
        }

        [TestMethod]
        public void NonNegative_ZeroNullableInteger_DoesNotThrow()
        {
            int? zeroValue = 0;

            Require.NonNegative(zeroValue);
        }

        [TestMethod]
        public void NonNegative_PositiveNullableInteger_DoesNotThrow()
        {
            int? positiveValue = 1;

            Require.NonNegative(positiveValue);
        }

        [TestMethod]
        public void NonNegativeIfProvided_NullNullableInteger_DoesNotThrow()
        {
            int? nullValue = null;

            Require.NonNegativeIfProvided(nullValue);
        }

        [TestMethod]
        public void NonNegativeIfProvided_NegativeNullableInteger_ThrowsArgumentOutOfRangeException()
        {
            int? negativeValue = -1;

            var exception = Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => Require.NonNegativeIfProvided(negativeValue));

            Assert.AreEqual(nameof(negativeValue), exception.ParamName);
        }

        [TestMethod]
        public void NonNegativeIfProvided_ZeroNullableInteger_DoesNotThrow()
        {
            int? zeroValue = 0;

            Require.NonNegativeIfProvided(zeroValue);
        }

        [TestMethod]
        public void NonNegativeIfProvided_PositiveNullableInteger_DoesNotThrow()
        {
            int? positiveValue = 1;

            Require.NonNegativeIfProvided(positiveValue);
        }

        [TestMethod]
        public void PositiveIfProvidedOrDefault_NullNullableInteger_ReturnsDefaultValue()
        {
            int? nullValue = null;
            var defaultValue = 42;

            var result = Require.PositiveIfProvidedOrDefault(nullValue, defaultValue);

            Assert.AreEqual(defaultValue, result);
        }

        [TestMethod]
        public void PositiveIfProvidedOrDefault_PositiveNullableInteger_ReturnsProvidedValue()
        {
            int? positiveValue = 7;
            var defaultValue = 42;

            var result = Require.PositiveIfProvidedOrDefault(positiveValue, defaultValue);

            Assert.AreEqual(positiveValue.Value, result);
        }

        [TestMethod]
        public void PositiveIfProvidedOrDefault_ZeroNullableInteger_ThrowsArgumentOutOfRangeException()
        {
            int? zeroValue = 0;

            var exception = Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => Require.PositiveIfProvidedOrDefault(zeroValue, 42));

            Assert.AreEqual(nameof(zeroValue), exception.ParamName);
        }

        [TestMethod]
        public void PositiveIfProvidedOrDefault_NegativeNullableInteger_ThrowsArgumentOutOfRangeException()
        {
            int? negativeValue = -1;

            var exception = Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => Require.PositiveIfProvidedOrDefault(negativeValue, 42));

            Assert.AreEqual(nameof(negativeValue), exception.ParamName);
        }
    }
}
