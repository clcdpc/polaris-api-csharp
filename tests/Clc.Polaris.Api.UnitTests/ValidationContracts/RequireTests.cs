namespace Clc.Polaris.Api.UnitTests.ValidationContracts
{
    [TestClass]
    [UnitTest]
    public class RequireTests
    {
        [TestMethod]
        public void Argument_NullObject_ThrowsArgumentNullException()
        {
            object? value = null;

            AssertThrowsArgumentException<ArgumentNullException>(() => Require.Argument(value), nameof(value));
        }

        [TestMethod]
        public void Argument_NullString_ThrowsArgumentNullException()
        {
            string? value = null;

            AssertThrowsArgumentException<ArgumentNullException>(() => Require.Argument(value), nameof(value));
        }

        [TestMethod]
        [DataRow("")]
        [DataRow("   ")]
        public void Argument_EmptyOrWhitespaceString_ThrowsArgumentException(string value)
        {
            AssertThrowsArgumentException<ArgumentException>(() => Require.Argument(value), nameof(value));
        }

        [TestMethod]
        [DataRow("value")]
        [DataRow(0)]
        public void Argument_ValidValue_DoesNotThrow(object value)
        {
            Require.Argument(value);
        }

        [TestMethod]
        [DataRow(0)]
        [DataRow(-1)]
        public void Positive_InvalidInteger_ThrowsArgumentOutOfRangeException(int value)
        {
            AssertThrowsArgumentException<ArgumentOutOfRangeException>(() => Require.Positive(value), nameof(value));
        }

        [TestMethod]
        public void Positive_PositiveInteger_DoesNotThrow()
        {
            var value = 1;

            Require.Positive(value);
        }

        [TestMethod]
        public void Positive_NullNullableInteger_ThrowsArgumentNullException()
        {
            int? value = null;

            AssertThrowsArgumentException<ArgumentNullException>(() => Require.Positive(value), nameof(value));
        }

        [TestMethod]
        [DataRow(0)]
        [DataRow(-1)]
        public void Positive_InvalidNullableInteger_ThrowsArgumentOutOfRangeException(int providedValue)
        {
            int? value = providedValue;

            AssertThrowsArgumentException<ArgumentOutOfRangeException>(() => Require.Positive(value), nameof(value));
        }

        [TestMethod]
        public void Positive_PositiveNullableInteger_DoesNotThrow()
        {
            int? value = 1;

            Require.Positive(value);
        }

        [TestMethod]
        public void PositiveIfProvided_NullNullableInteger_DoesNotThrow()
        {
            int? value = null;

            Require.PositiveIfProvided(value);
        }

        [TestMethod]
        [DataRow(0)]
        [DataRow(-1)]
        public void PositiveIfProvided_InvalidNullableInteger_ThrowsArgumentOutOfRangeException(int providedValue)
        {
            int? value = providedValue;

            AssertThrowsArgumentException<ArgumentOutOfRangeException>(() => Require.PositiveIfProvided(value), nameof(value));
        }

        [TestMethod]
        public void PositiveIfProvided_PositiveNullableInteger_DoesNotThrow()
        {
            int? value = 1;

            Require.PositiveIfProvided(value);
        }

        [TestMethod]
        public void NonNegative_NegativeInteger_ThrowsArgumentOutOfRangeException()
        {
            var value = -1;

            AssertThrowsArgumentException<ArgumentOutOfRangeException>(() => Require.NonNegative(value), nameof(value));
        }

        [TestMethod]
        [DataRow(0)]
        [DataRow(1)]
        public void NonNegative_ValidInteger_DoesNotThrow(int value)
        {
            Require.NonNegative(value);
        }

        [TestMethod]
        public void NonNegative_NullNullableInteger_ThrowsArgumentNullException()
        {
            int? value = null;

            AssertThrowsArgumentException<ArgumentNullException>(() => Require.NonNegative(value), nameof(value));
        }

        [TestMethod]
        public void NonNegative_NegativeNullableInteger_ThrowsArgumentOutOfRangeException()
        {
            int? value = -1;

            AssertThrowsArgumentException<ArgumentOutOfRangeException>(() => Require.NonNegative(value), nameof(value));
        }

        [TestMethod]
        [DataRow(0)]
        [DataRow(1)]
        public void NonNegative_ValidNullableInteger_DoesNotThrow(int providedValue)
        {
            int? value = providedValue;

            Require.NonNegative(value);
        }

        [TestMethod]
        public void NonNegativeIfProvided_NullNullableInteger_DoesNotThrow()
        {
            int? value = null;

            Require.NonNegativeIfProvided(value);
        }

        [TestMethod]
        public void NonNegativeIfProvided_NegativeNullableInteger_ThrowsArgumentOutOfRangeException()
        {
            int? value = -1;

            AssertThrowsArgumentException<ArgumentOutOfRangeException>(() => Require.NonNegativeIfProvided(value), nameof(value));
        }

        [TestMethod]
        [DataRow(0)]
        [DataRow(1)]
        public void NonNegativeIfProvided_ValidNullableInteger_DoesNotThrow(int providedValue)
        {
            int? value = providedValue;

            Require.NonNegativeIfProvided(value);
        }

        [TestMethod]
        public void PositiveIfProvidedOrDefault_NullNullableInteger_ReturnsDefaultValue()
        {
            int? value = null;
            var defaultValue = 42;

            var result = Require.PositiveIfProvidedOrDefault(value, defaultValue);

            Assert.AreEqual(defaultValue, result);
        }

        [TestMethod]
        public void PositiveIfProvidedOrDefault_PositiveNullableInteger_ReturnsProvidedValue()
        {
            int? value = 7;
            var defaultValue = 42;

            var result = Require.PositiveIfProvidedOrDefault(value, defaultValue);

            Assert.AreEqual(value.Value, result);
        }

        [TestMethod]
        [DataRow(0)]
        [DataRow(-1)]
        public void PositiveIfProvidedOrDefault_InvalidNullableInteger_ThrowsArgumentOutOfRangeException(int providedValue)
        {
            int? value = providedValue;

            AssertThrowsArgumentException<ArgumentOutOfRangeException>(() => Require.PositiveIfProvidedOrDefault(value, 42), nameof(value));
        }

        private static TException AssertThrowsArgumentException<TException>(Action action, string expectedParamName)
            where TException : ArgumentException
        {
            var exception = Assert.ThrowsExactly<TException>(action);

            Assert.AreEqual(expectedParamName, exception.ParamName);

            return exception;
        }
    }
}
