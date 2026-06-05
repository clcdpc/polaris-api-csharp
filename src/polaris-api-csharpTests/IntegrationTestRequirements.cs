using Clc.Polaris.Api.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Clc.Polaris.Api.Tests
{
    internal static class IntegrationTestRequirements
    {
        public const string MissingIntegrationConfigurationMessage = "Integration test configuration is missing. Provide appsettings.Test.json or environment variables to run integration tests.";
        public const string MissingStaffOverrideAccountMessage = "Integration test requires staff override credentials. Configure PapiSettings:PolarisOverrideAccount with non-empty Domain, Username, and Password to run protected or destructive live tests.";

        public static bool HasRequiredPapiSettings(PapiSettings? settings)
        {
            return settings != null
                && !string.IsNullOrWhiteSpace(settings.AccessId)
                && !string.IsNullOrWhiteSpace(settings.AccessKey)
                && !string.IsNullOrWhiteSpace(settings.Hostname);
        }

        public static bool HasRequiredTestSettings(TestSettings? settings)
        {
            return settings != null
                && settings.PatronId > 0
                && !string.IsNullOrWhiteSpace(settings.PatronBarcode)
                && !string.IsNullOrWhiteSpace(settings.PatronPin)
                && !string.IsNullOrWhiteSpace(settings.FreeTextBlock)
                && !string.IsNullOrWhiteSpace(settings.OrgEmail);
        }

        public static bool HasRequiredStaffOverrideAccount(PapiSettings? settings)
        {
            var staffOverrideAccount = settings?.PolarisOverrideAccount;

            return staffOverrideAccount != null
                && !string.IsNullOrWhiteSpace(staffOverrideAccount.Domain)
                && !string.IsNullOrWhiteSpace(staffOverrideAccount.Username)
                && !string.IsNullOrWhiteSpace(staffOverrideAccount.Password);
        }

        public static void RequireStaffOverrideAccount(PapiSettings? settings)
        {
            if (!HasRequiredStaffOverrideAccount(settings))
            {
                Assert.Inconclusive(MissingStaffOverrideAccountMessage);
            }
        }
    }
}
