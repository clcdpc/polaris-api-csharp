using Clc.Polaris.Api.Configuration;
using Clc.Polaris.Api.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Clc.Polaris.Api.Tests
{
    [TestClass]
    [TestCategory("Unit")]
    public class IntegrationTestConfigurationTests
    {
        [TestMethod]
        public void HasRequiredStaffOverrideAccount_ReturnsFalseWhenSettingsAreNull()
        {
            Assert.IsFalse(IntegrationTestConfiguration.HasRequiredStaffOverrideAccount(null));
        }

        [TestMethod]
        public void HasRequiredStaffOverrideAccount_ReturnsFalseWhenOverrideAccountIsMissing()
        {
            var settings = new PapiSettings();

            Assert.IsFalse(IntegrationTestConfiguration.HasRequiredStaffOverrideAccount(settings));
        }

        [DataTestMethod]
        [DataRow("", "username", "password")]
        [DataRow(" ", "username", "password")]
        [DataRow("domain", "", "password")]
        [DataRow("domain", " ", "password")]
        [DataRow("domain", "username", "")]
        [DataRow("domain", "username", " ")]
        public void HasRequiredStaffOverrideAccount_ReturnsFalseWhenAnyOverrideFieldIsBlank(string domain, string username, string password)
        {
            var settings = new PapiSettings
            {
                PolarisOverrideAccount = new PolarisUser(domain, username, password)
            };

            Assert.IsFalse(IntegrationTestConfiguration.HasRequiredStaffOverrideAccount(settings));
        }

        [TestMethod]
        public void HasRequiredStaffOverrideAccount_ReturnsTrueWhenOverrideAccountIsComplete()
        {
            var settings = new PapiSettings
            {
                PolarisOverrideAccount = new PolarisUser("domain", "username", "password")
            };

            Assert.IsTrue(IntegrationTestConfiguration.HasRequiredStaffOverrideAccount(settings));
        }
    }
}
