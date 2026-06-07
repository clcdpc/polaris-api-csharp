using System;
using Clc.Polaris.Api.Tests;
using Clc.Polaris.Api.Validation;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Clc.Polaris.Api.Validation.Tests
{
    [TestClass]
    [UnitCategory]
    public class RequireTests
    {
        [TestMethod]
        public void Argument_NullObject_ThrowsArgumentNullException()
        {
            object? nullValue = null;

            var exception = Assert.ThrowsException<ArgumentNullException>(() => Require.Argument(nullValue));

            Assert.AreEqual(nameof(nullValue), exception.ParamName);
        }

        [TestMethod]
        public void Argument_NonNullObject_DoesNotThrow()
        {
            object notNullValue = new object();

            Require.Argument(notNullValue);
        }

        [TestMethod]
        public void Argument_NullString_ThrowsArgumentNullException()
        {
            string? nullString = null;

            var exception = Assert.ThrowsException<ArgumentNullException>(() => Require.Argument(nullString));

            Assert.AreEqual(nameof(nullString), exception.ParamName);
        }

        [TestMethod]
        public void Argument_EmptyString_ThrowsArgumentException()
        {
            string emptyString = string.Empty;

            var exception = Assert.ThrowsException<ArgumentException>(() => Require.Argument(emptyString));

            Assert.AreEqual(nameof(emptyString), exception.ParamName);
        }

        [TestMethod]
        public void Argument_WhitespaceString_ThrowsArgumentException()
        {
            string whitespaceString = "   ";

            var exception = Assert.ThrowsException<ArgumentException>(() => Require.Argument(whitespaceString));

            Assert.AreEqual(nameof(whitespaceString), exception.ParamName);
        }

        [TestMethod]
        public void Argument_NonEmptyString_DoesNotThrow()
        {
            string nonEmptyString = "value";

            Require.Argument(nonEmptyString);
        }
    }
}
