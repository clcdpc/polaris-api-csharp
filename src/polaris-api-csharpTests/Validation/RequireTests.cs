using Microsoft.VisualStudio.TestTools.UnitTesting;
using Clc.Polaris.Api.Validation;
using System;

namespace Clc.Polaris.Api.Validation.Tests
{
    [TestClass]
    public class RequireTests
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Argument_NullValue_ThrowsArgumentNullException()
        {
            // Arrange
            string argumentName = "testArgument";
            object? nullValue = null;

            // Act
            Require.Argument(argumentName, nullValue);

            // Assert is handled by ExpectedException
        }

        [TestMethod]
        public void Argument_NotNullValue_DoesNotThrow()
        {
            // Arrange
            string argumentName = "testArgument";
            object notNullValue = new object();

            // Act
            Require.Argument(argumentName, notNullValue);

            // Assert
            // No exception should be thrown
        }
    }
}
