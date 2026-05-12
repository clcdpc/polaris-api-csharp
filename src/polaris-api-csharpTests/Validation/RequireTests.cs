using Microsoft.VisualStudio.TestTools.UnitTesting;
using Clc.Polaris.Api.Validation;
using System;

namespace Clc.Polaris.Api.Tests.Validation
{
    [TestClass()]
    [TestCategory("Unit")]
    public class RequireTests
    {
        [TestMethod()]
        public void Argument_NullValue_ThrowsArgumentNullException()
        {
            // Arrange
            string argumentName = "testArgument";
            object nullValue = null;

            // Act & Assert
            var ex = Assert.ThrowsException<ArgumentNullException>(() => Require.Argument(argumentName, nullValue));
            Assert.AreEqual(argumentName, ex.ParamName);
        }

        [TestMethod()]
        public void Argument_NotNullValue_DoesNotThrow()
        {
            // Arrange
            string argumentName = "testArgument";
            object notNullValue = new object();

            // Act
            Require.Argument(argumentName, notNullValue);

            // Assert
            // If no exception is thrown, the test passes
        }
    }
}
