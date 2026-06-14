using Clc.Polaris.Api.Configuration;

namespace Clc.Polaris.Api.Tests
{
    [TestClass]
    public sealed class ProtectedTokenStaffOverrideIntegrationTests : IntegrationTestBase
    {
        [TestMethod]
        [ProtectedReadOnlyIntegrationTest]
        public async Task AuthenticateStaffUser_ConfiguredOverrideAccountSucceeds()
        {
            IntegrationTestRequirements.RequireStaffOverrideAccount(PapiSettings);

            var response = await Papi.AuthenticateStaffUserAsync(PapiSettings.PolarisOverrideAccount!, TestContext.CancellationToken);

            Assert.AreEqual(0, response.Data.PAPIErrorCode);
            Assert.IsFalse(string.IsNullOrWhiteSpace(response.Data.AccessToken));
            Assert.IsFalse(string.IsNullOrWhiteSpace(response.Data.AccessSecret));
        }

        [TestMethod]
        [ProtectedReadOnlyIntegrationTest]
        public async Task PatronSearch_WithProtectedTokenFindsConfiguredPatron()
        {
            IntegrationTestRequirements.RequireStaffOverrideAccount(PapiSettings);

            var response = await Papi.PatronSearchAsync($"PRID={Settings.PatronId}", cancellationToken: TestContext.CancellationToken);

            Assert.AreEqual(response.Data.PatronSearchRows.Count, response.Data.PAPIErrorCode);
            Assert.IsNotNull(response.Data.PatronSearchRows.SingleOrDefault(row => row.PatronID == Settings.PatronId));
        }

        [TestMethod]
        [ProtectedReadOnlyIntegrationTest]
        public async Task PatronSearch_SameClientCanMakeBackToBackProtectedCalls()
        {
            IntegrationTestRequirements.RequireStaffOverrideAccount(PapiSettings);

            var firstResponse = await Papi.PatronSearchAsync($"PRID={Settings.PatronId}", cancellationToken: TestContext.CancellationToken);
            var secondResponse = await Papi.PatronSearchAsync($"PRID={Settings.PatronId}", cancellationToken: TestContext.CancellationToken);

            Assert.AreEqual(firstResponse.Data.PatronSearchRows.Count, firstResponse.Data.PAPIErrorCode);
            Assert.IsNotNull(firstResponse.Data.PatronSearchRows.SingleOrDefault(row => row.PatronID == Settings.PatronId));

            Assert.AreEqual(secondResponse.Data.PatronSearchRows.Count, secondResponse.Data.PAPIErrorCode);
            Assert.IsNotNull(secondResponse.Data.PatronSearchRows.SingleOrDefault(row => row.PatronID == Settings.PatronId));
        }

        [TestMethod]
        [ProtectedReadOnlyIntegrationTest]
        public async Task PatronAccountGet_StaffOverrideSucceedsWithoutPatronPassword()
        {
            IntegrationTestRequirements.RequireStaffOverrideAccount(PapiSettings);

            var client = CreateStaffOverrideClient();
            var response = await client.PatronAccountGetAsync(Settings.PatronBarcode, password: string.Empty, cancellationToken: TestContext.CancellationToken);

            Assert.AreEqual(0, response.Data.PAPIErrorCode);
        }

        [TestMethod]
        [ProtectedReadOnlyIntegrationTest]
        public async Task PatronAccountGet_StaffOverrideSameClientCanMakeBackToBackPublicOverrideCalls()
        {
            IntegrationTestRequirements.RequireStaffOverrideAccount(PapiSettings);

            var client = CreateStaffOverrideClient();

            var firstResponse = await client.PatronAccountGetAsync(Settings.PatronBarcode, password: string.Empty, cancellationToken: TestContext.CancellationToken);
            var secondResponse = await client.PatronAccountGetAsync(Settings.PatronBarcode, password: string.Empty, cancellationToken: TestContext.CancellationToken);

            Assert.AreEqual(0, firstResponse.Data.PAPIErrorCode);
            Assert.AreEqual(0, secondResponse.Data.PAPIErrorCode);
        }

        private PapiClient CreateStaffOverrideClient()
        {
            return new PapiClient(new PapiSettings
            {
                AccessId = PapiSettings.AccessId,
                AccessKey = PapiSettings.AccessKey,
                Hostname = PapiSettings.Hostname,
                OrganizationId = PapiSettings.OrganizationId,
                UserId = PapiSettings.UserId,
                WorkstationId = PapiSettings.WorkstationId,
                PolarisOverrideAccount = PapiSettings.PolarisOverrideAccount,
            })
            {
                AllowStaffOverrideRequests = true,
            };
        }
    }
}