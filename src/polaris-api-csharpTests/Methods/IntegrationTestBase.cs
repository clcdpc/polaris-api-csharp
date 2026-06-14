using Clc.Polaris.Api.Configuration;
using Microsoft.Extensions.Configuration;

namespace Clc.Polaris.Api.Tests
{
    public abstract class IntegrationTestBase
    {
        protected const string TestArtifactPrefix = "PAPI_TEST_";

        protected TestSettings Settings = null!;
        protected PapiSettings PapiSettings = null!;
        protected IPapiClient Papi = null!;

        public TestContext TestContext { get; set; } = null!;

        protected int RequireConfiguredStaffUser() => RequirePositiveSetting(Settings.StaffUserId, nameof(Settings.StaffUserId));

        protected int RequireConfiguredStaffWorkstation() => RequirePositiveSetting(Settings.StaffWorkstationId, nameof(Settings.StaffWorkstationId));

        protected static IConfiguration InitConfiguration()
        {
            var config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.Test.json", optional: true)
                .AddEnvironmentVariables()
                .Build();

            return config;
        }

        [TestInitialize]
        public void TestInitialize()
        {
            var config = InitConfiguration();

            var papiSettings = config.GetSection(PapiSettings.SECTION_NAME).Get<PapiSettings>();
            var testSettings = config.Get<TestSettings>();

            if (!IntegrationTestRequirements.HasRequiredPapiSettings(papiSettings) || !IntegrationTestRequirements.HasRequiredTestSettings(testSettings))
            {
                Assert.Inconclusive(IntegrationTestRequirements.MissingIntegrationConfigurationMessage);
            }

            Papi = new PapiClient(papiSettings!);
            PapiSettings = papiSettings!;
            Settings = testSettings!;
        }

        protected static int RequirePositiveSetting(int? value, string settingName)
        {
            if (value is not > 0)
            {
                Assert.Inconclusive($"Integration test setting '{settingName}' must be configured with a positive integer to run this live scenario.");
            }

            return value.Value;
        }

        protected int RequireConfiguredBib() => RequirePositiveSetting(Settings.BibId, nameof(Settings.BibId));

        protected int RequireConfiguredBranch() => RequirePositiveSetting(Settings.BranchId, nameof(Settings.BranchId));

        protected static string CreateUniqueTestArtifactText(string? baseName = null, int maxLength = 80)
        {
            var sanitizedBaseName = SanitizeArtifactBaseName(baseName);
            var uniqueSuffix = $"{DateTime.UtcNow:yyyyMMddHHmmss}_{Guid.NewGuid():N}";
            var suffixWithPrefix = $"{TestArtifactPrefix}{uniqueSuffix}";

            if (string.IsNullOrWhiteSpace(sanitizedBaseName))
            {
                return suffixWithPrefix;
            }

            var baseNameBudget = maxLength - suffixWithPrefix.Length - 1;
            if (baseNameBudget <= 0)
            {
                return suffixWithPrefix;
            }

            var trimmedBaseName = sanitizedBaseName.Length <= baseNameBudget
                ? sanitizedBaseName
                : sanitizedBaseName[..baseNameBudget];

            return $"{TestArtifactPrefix}{trimmedBaseName}_{uniqueSuffix}";
        }

        protected static string SanitizeArtifactBaseName(string? baseName)
        {
            if (string.IsNullOrWhiteSpace(baseName))
            {
                return string.Empty;
            }

            var sanitized = new string(baseName
                .Where(c => char.IsLetterOrDigit(c) || c == '-' || c == '_')
                .ToArray());

            return sanitized.Trim('_', '-');
        }
    }
}
