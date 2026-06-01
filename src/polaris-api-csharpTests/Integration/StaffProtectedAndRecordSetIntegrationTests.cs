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
    public void RecordSetContentAddAsync_WithSingleRecordAndNonexistentRecordSet_ReturnsDocumentedError()
    {
        DocumentScenarioDependentPlaceholder(
            nameof(Papi.RecordSetContentAddAsync),
            "adding record-set content mutates staff/protected record-set state and hard-coded record-set or bib IDs might exist in a live Polaris database",
            "IntegrationTestOptions:EnableStaffProtectedTests=true, IntegrationTestOptions:EnableMutatingIntegrationTests=true, staff override credentials, and configured disposable record-set plus bib fixtures",
            "call RecordSetContentAddAsync only against a disposable record set and assert the documented response before cleanup");
    }

    [TestMethod]
    public void RecordSetContentAddAsync_WithRecordListAndNonexistentRecordSet_ReturnsDocumentedError()
    {
        DocumentScenarioDependentPlaceholder(
            nameof(Papi.RecordSetContentAddAsync),
            "adding record-set content mutates staff/protected record-set state and hard-coded record-set or bib IDs might exist in a live Polaris database",
            "IntegrationTestOptions:EnableStaffProtectedTests=true, IntegrationTestOptions:EnableMutatingIntegrationTests=true, staff override credentials, and configured disposable record-set plus bib fixtures",
            "call the record-list overload only against a disposable record set and assert the documented response before cleanup");
    }

    [TestMethod]
    public void RecordSetContentRemoveAsync_WithSingleRecordAndNonexistentRecordSet_ReturnsDocumentedError()
    {
        DocumentScenarioDependentPlaceholder(
            nameof(Papi.RecordSetContentRemoveAsync),
            "removing record-set content mutates staff/protected record-set state and hard-coded record-set or bib IDs might exist in a live Polaris database",
            "IntegrationTestOptions:EnableStaffProtectedTests=true, IntegrationTestOptions:EnableMutatingIntegrationTests=true, staff override credentials, and configured disposable record-set plus bib fixtures",
            "call RecordSetContentRemoveAsync only against a disposable record set and assert the documented response before cleanup");
    }

    [TestMethod]
    public void RecordSetContentRemoveAsync_WithRecordListAndNonexistentRecordSet_ReturnsDocumentedError()
    {
        DocumentScenarioDependentPlaceholder(
            nameof(Papi.RecordSetContentRemoveAsync),
            "removing record-set content mutates staff/protected record-set state and hard-coded record-set or bib IDs might exist in a live Polaris database",
            "IntegrationTestOptions:EnableStaffProtectedTests=true, IntegrationTestOptions:EnableMutatingIntegrationTests=true, staff override credentials, and configured disposable record-set plus bib fixtures",
            "call the record-list overload only against a disposable record set and assert the documented response before cleanup");
    }

    [TestMethod]
    public void RecordSetContentPutAsync_WithNonexistentRecordSet_ReturnsDocumentedError()
    {
        DocumentScenarioDependentPlaceholder(
            nameof(Papi.RecordSetContentPutAsync),
            "putting record-set content mutates staff/protected record-set state and hard-coded record-set or bib IDs might exist in a live Polaris database",
            "IntegrationTestOptions:EnableStaffProtectedTests=true, IntegrationTestOptions:EnableMutatingIntegrationTests=true, staff override credentials, and configured disposable record-set plus bib fixtures",
            "call RecordSetContentPutAsync only against a disposable record set and assert the documented response before cleanup");
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

        RequireRemoteStorageScenario();

        var response = await Papi.RemoteStorageItemsGetAsync(Settings.RemoteStorageBranchId, Settings.RemoteStorageStartDate, Settings.RemoteStorageEndDate, 10, 1);
        var data = PapiIntegrationAssert.HasPapiData(response);

        Assert.IsTrue(data.PAPIErrorCode >= 0, data.ErrorMessage);
        Assert.IsNotNull(data.RemoteStorageItemsGetRows);
    }
}
