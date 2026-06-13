using Clc.Polaris.Api.Configuration;

namespace Clc.Polaris.Api.Tests.Validation
{
    [TestClass]
    [UnitTest]
    public class IntegrationTestRequirementsTests
    {
        [TestMethod]
        public void HasRequiredStaffOverrideAccount_ReturnsFalseWhenSettingsAreNull()
        {
            Assert.IsFalse(IntegrationTestRequirements.HasRequiredStaffOverrideAccount(null));
        }

        [TestMethod]
        public void HasRequiredStaffOverrideAccount_ReturnsFalseWhenOverrideAccountIsMissing()
        {
            var settings = new PapiSettings();

            Assert.IsFalse(IntegrationTestRequirements.HasRequiredStaffOverrideAccount(settings));
        }

        [TestMethod]
        [DataRow("", "user", "password")]
        [DataRow("domain", "", "password")]
        [DataRow("domain", "user", "")]
        [DataRow("   ", "user", "password")]
        [DataRow("domain", "   ", "password")]
        [DataRow("domain", "user", "   ")]
        public void HasRequiredStaffOverrideAccount_ReturnsFalseWhenAnyCredentialFieldIsBlank(string domain, string username, string password)
        {
            var settings = CreateSettings(domain, username, password);

            Assert.IsFalse(IntegrationTestRequirements.HasRequiredStaffOverrideAccount(settings));
        }

        [TestMethod]
        public void HasRequiredStaffOverrideAccount_ReturnsTrueWhenAllCredentialFieldsArePresent()
        {
            var settings = CreateSettings("domain", "user", "password");

            Assert.IsTrue(IntegrationTestRequirements.HasRequiredStaffOverrideAccount(settings));
        }

        private static PapiSettings CreateSettings(string domain, string username, string password)
        {
            return new PapiSettings
            {
                PolarisOverrideAccount = new PolarisUser
                {
                    Domain = domain,
                    Username = username,
                    Password = password
                }
            };
        }
    }
}
