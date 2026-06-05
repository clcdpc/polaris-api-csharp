using Clc.Polaris.Api.Configuration;

namespace Clc.Polaris.Api.Tests
{
    internal static class IntegrationTestConfiguration
    {
        public static bool HasRequiredPapiSettings(PapiSettings? settings)
        {
            return settings != null
                && !string.IsNullOrWhiteSpace(settings.AccessId)
                && !string.IsNullOrWhiteSpace(settings.AccessKey)
                && !string.IsNullOrWhiteSpace(settings.Hostname);
        }

        public static bool HasRequiredStaffOverrideAccount(PapiSettings? settings)
        {
            return settings?.PolarisOverrideAccount != null
                && !string.IsNullOrWhiteSpace(settings.PolarisOverrideAccount.Domain)
                && !string.IsNullOrWhiteSpace(settings.PolarisOverrideAccount.Username)
                && !string.IsNullOrWhiteSpace(settings.PolarisOverrideAccount.Password);
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
    }
}
