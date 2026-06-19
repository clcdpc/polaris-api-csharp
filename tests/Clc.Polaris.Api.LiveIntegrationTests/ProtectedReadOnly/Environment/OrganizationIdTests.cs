using Clc.Polaris.Api.Configuration;
using Clc.Rest;
using System.Net;

namespace Clc.Polaris.Api.LiveIntegrationTests.ProtectedReadOnly.Environment
{
    [TestClass]
    public sealed class OrganizationIdTests : IntegrationTestBase
    {
        private const int OrganizationOneId = 1;
        private const int ComparisonOrganizationId = 73;

        [TestMethod]
        [ProtectedReadOnlyLiveTest]
        public async Task RecordSetRecordsGetAsync_ComparisonOrganizationIdMatchesOrganizationOne()
        {
            var recordSetId = RequireConfiguredRecordSetId();
            IntegrationTestRequirements.RequireStaffOverrideAccount(PapiSettings);

            var organizationOneClient = CreateClientWithOrganizationId(OrganizationOneId);
            var organizationOneResponse = await organizationOneClient.RecordSetRecordsGetAsync(recordSetId, cancellationToken: TestContext.CancellationToken);
            AssertSuccessfulResponse(organizationOneResponse, "organization 1");
            Assert.IsNotNull(organizationOneResponse.Data.RecordSetRecordsGetRows);

            var comparisonClient = CreateClientWithOrganizationId(ComparisonOrganizationId);
            var comparisonResponse = await comparisonClient.RecordSetRecordsGetAsync(recordSetId, cancellationToken: TestContext.CancellationToken);
            AssertSuccessfulResponse(comparisonResponse, $"organization {ComparisonOrganizationId}");
            Assert.IsNotNull(comparisonResponse.Data.RecordSetRecordsGetRows);

            Assert.AreEqual(organizationOneResponse.Data.PAPIErrorCode, comparisonResponse.Data.PAPIErrorCode);
            Assert.HasCount(organizationOneResponse.Data.RecordSetRecordsGetRows.Count, comparisonResponse.Data.RecordSetRecordsGetRows);
            Assert.AreEqual(organizationOneResponse.Data.ToString(), comparisonResponse.Data.ToString());
        }

        private PapiClient CreateClientWithOrganizationId(int organizationId)
        {
            var settings = new PapiSettings
            {
                AccessId = PapiSettings.AccessId,
                AccessKey = PapiSettings.AccessKey,
                Hostname = PapiSettings.Hostname,
                OrganizationId = organizationId,
                UserId = PapiSettings.UserId,
                WorkstationId = PapiSettings.WorkstationId,
                PolarisOverrideAccount = PapiSettings.PolarisOverrideAccount
            };

            return new PapiClient(settings)
            {
                UseProtectedTokenCache = false
            };
        }

        private int RequireConfiguredRecordSetId()
        {
            if (Settings.RecordSetId is not > 0)
            {
                Assert.Inconclusive("RecordSetRecordsGet OrganizationId integration coverage requires TestSettings:RecordSetId to be configured with an accessible record set ID.");
            }

            return Settings.RecordSetId.Value;
        }

        private static void AssertSuccessfulResponse<T>(IRestResponse<T> response, string label)
        {
            Assert.IsNotNull(response, $"Expected {label} response.");
            Assert.IsNotNull(response.Response, $"Expected {label} HTTP response.");
            Assert.AreEqual(HttpStatusCode.OK, response.Response.StatusCode, $"Expected {label} HTTP success response but got {(int)response.Response.StatusCode} {response.Response.ReasonPhrase}.");
            Assert.IsNotNull(response.Data, $"Expected {label} response data.");
        }
    }
}
