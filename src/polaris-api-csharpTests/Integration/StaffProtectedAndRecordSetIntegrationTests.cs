using Clc.Polaris.Api.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Clc.Polaris.Api.Tests.Integration;

[TestClass]
[TestCategory("Integration")]
public sealed class StaffProtectedAndRecordSetIntegrationTests : IntegrationTestBase
{
    [TestMethod]
    public async Task RecordSetRecordsGetAsync_WithNonexistentRecordSet_ReturnsDocumentedError()
    {
        RequireStaffProtectedTestsEnabled();

        var response = await Papi.RecordSetRecordsGetAsync(NonexistentRecordSetId, UserIdOrConfigured, WorkstationIdOrConfigured);

        PapiIntegrationAssert.PapiError(response, -11001);
    }

    [TestMethod]
    public async Task RecordSetContentAddAsync_WithSingleRecordAndNonexistentRecordSet_ReturnsDocumentedError()
    {
        RequireStaffProtectedTestsEnabled();

        var response = await Papi.RecordSetContentAddAsync(NonexistentRecordSetId, NonexistentBibId, UserIdOrConfigured, WorkstationIdOrConfigured);

        PapiIntegrationAssert.PapiError(response, -11001);
    }

    [TestMethod]
    public async Task RecordSetContentAddAsync_WithRecordListAndNonexistentRecordSet_ReturnsDocumentedError()
    {
        RequireStaffProtectedTestsEnabled();

        var response = await Papi.RecordSetContentAddAsync(NonexistentRecordSetId, new[] { NonexistentBibId }, UserIdOrConfigured, WorkstationIdOrConfigured);

        PapiIntegrationAssert.PapiError(response, -11001);
    }

    [TestMethod]
    public async Task RecordSetContentRemoveAsync_WithSingleRecordAndNonexistentRecordSet_ReturnsDocumentedError()
    {
        RequireStaffProtectedTestsEnabled();

        var response = await Papi.RecordSetContentRemoveAsync(NonexistentRecordSetId, NonexistentBibId, UserIdOrConfigured, WorkstationIdOrConfigured);

        PapiIntegrationAssert.PapiError(response, -11001);
    }

    [TestMethod]
    public async Task RecordSetContentRemoveAsync_WithRecordListAndNonexistentRecordSet_ReturnsDocumentedError()
    {
        RequireStaffProtectedTestsEnabled();

        var response = await Papi.RecordSetContentRemoveAsync(NonexistentRecordSetId, new[] { NonexistentBibId }, UserIdOrConfigured, WorkstationIdOrConfigured);

        PapiIntegrationAssert.PapiError(response, -11001);
    }

    [TestMethod]
    public async Task RecordSetContentPutAsync_WithNonexistentRecordSet_ReturnsDocumentedError()
    {
        RequireStaffProtectedTestsEnabled();

        var response = await Papi.RecordSetContentPutAsync(NonexistentRecordSetId, new[] { NonexistentBibId }, RecordSetContentPutActions.Add, UserIdOrConfigured, WorkstationIdOrConfigured);

        PapiIntegrationAssert.PapiError(response, -11001);
    }

    [TestMethod]
    public async Task SA_GetValueByOrgAsync_WithOrgEmailScenario_ReturnsConfiguredValue()
    {
        RequireStaffProtectedTestsEnabled();
        RequireOrgEmailScenario();

        var response = await Papi.SA_GetValueByOrgAsync("ORGEMAIL", OrganizationIdOrConfigured);
        var data = PapiIntegrationAssert.HasData(response);

        Assert.AreEqual(Settings.OrgEmail, data.Value);
    }

    [TestMethod]
    public async Task NotificationQueueGetAsync_WhenStaffProtectedTestsEnabled_ReturnsDeserializedResponse()
    {
        RequireStaffProtectedTestsEnabled();

        var response = await Papi.NotificationQueueGetAsync(OrganizationIdOrConfigured);
        var data = PapiIntegrationAssert.HasPapiData(response);

        Assert.IsTrue(data.PAPIErrorCode >= 0, data.ErrorMessage);
    }

    [TestMethod]
    public async Task RemoteStorageItemsGetAsync_WhenScenarioEnabled_ReturnsDeserializedRows()
    {
        RequireStaffProtectedTestsEnabled();
        RequireScenarioDependentTestsEnabled(
            nameof(Papi.RemoteStorageItemsGetAsync),
            "TestSettings:RemoteStorageBranchId, TestSettings:RemoteStorageStartDate, and TestSettings:RemoteStorageEndDate for a branch/date range with remote-storage activity",
            "call RemoteStorageItemsGetAsync and assert PAPIErrorCode equals the deserialized row count");

        if (Settings.RemoteStorageBranchId <= 0 || string.IsNullOrWhiteSpace(Settings.RemoteStorageStartDate) || string.IsNullOrWhiteSpace(Settings.RemoteStorageEndDate))
        {
            Assert.Inconclusive("Remote storage scenario settings are incomplete.");
        }

        var response = await Papi.RemoteStorageItemsGetAsync(Settings.RemoteStorageBranchId, Settings.RemoteStorageStartDate, Settings.RemoteStorageEndDate, 10, 1);
        var data = PapiIntegrationAssert.HasPapiData(response);

        Assert.IsTrue(data.PAPIErrorCode >= 0, data.ErrorMessage);
        Assert.IsNotNull(data.RemoteStorageItemsGetRows);
    }
}
