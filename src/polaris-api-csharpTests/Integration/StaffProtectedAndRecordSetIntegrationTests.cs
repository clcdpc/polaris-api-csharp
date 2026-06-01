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
    public void RecordSetContentAddAsync_WithSingleRecord_RequiresDisposableRecordSetFixtureAndIsDisabledByDefault()
    {
        DocumentScenarioDependentPlaceholder(
            nameof(Papi.RecordSetContentAddAsync),
            "adding a record to a record set mutates record-set content and must not target hard-coded record-set or bib ids that might exist",
            "IntegrationTestOptions:EnableStaffProtectedTests=true, IntegrationTestOptions:EnableMutatingIntegrationTests=true, staff override credentials, and disposable TestSettings:RecordSetId/BibId fixture data",
            "call RecordSetContentAddAsync only for the configured disposable record set and record, assert the documented PAPIErrorCode, and remove only content the test definitely added");
    }

    [TestMethod]
    public void RecordSetContentAddAsync_WithRecordList_RequiresDisposableRecordSetFixtureAndIsDisabledByDefault()
    {
        DocumentScenarioDependentPlaceholder(
            nameof(Papi.RecordSetContentAddAsync),
            "adding records to a record set mutates record-set content and must not target hard-coded record-set or bib ids that might exist",
            "IntegrationTestOptions:EnableStaffProtectedTests=true, IntegrationTestOptions:EnableMutatingIntegrationTests=true, staff override credentials, and disposable TestSettings:RecordSetId/BibId fixture data",
            "call the record-list overload only for the configured disposable record set and records, assert the documented PAPIErrorCode, and remove only content the test definitely added");
    }

    [TestMethod]
    public void RecordSetContentRemoveAsync_WithSingleRecord_RequiresDisposableRecordSetFixtureAndIsDisabledByDefault()
    {
        DocumentScenarioDependentPlaceholder(
            nameof(Papi.RecordSetContentRemoveAsync),
            "removing a record from a record set mutates record-set content and must not target hard-coded record-set or bib ids that might exist",
            "IntegrationTestOptions:EnableStaffProtectedTests=true, IntegrationTestOptions:EnableMutatingIntegrationTests=true, staff override credentials, and disposable TestSettings:RecordSetId/BibId fixture data preloaded by the test",
            "call RecordSetContentRemoveAsync only for content the test definitely added to the disposable record set and assert the documented PAPIErrorCode");
    }

    [TestMethod]
    public void RecordSetContentRemoveAsync_WithRecordList_RequiresDisposableRecordSetFixtureAndIsDisabledByDefault()
    {
        DocumentScenarioDependentPlaceholder(
            nameof(Papi.RecordSetContentRemoveAsync),
            "removing records from a record set mutates record-set content and must not target hard-coded record-set or bib ids that might exist",
            "IntegrationTestOptions:EnableStaffProtectedTests=true, IntegrationTestOptions:EnableMutatingIntegrationTests=true, staff override credentials, and disposable TestSettings:RecordSetId/BibId fixture data preloaded by the test",
            "call the record-list overload only for content the test definitely added to the disposable record set and assert the documented PAPIErrorCode");
    }

    [TestMethod]
    public void RecordSetContentPutAsync_RequiresDisposableRecordSetFixtureAndIsDisabledByDefault()
    {
        DocumentScenarioDependentPlaceholder(
            nameof(Papi.RecordSetContentPutAsync),
            "putting record-set content mutates record-set content and must not target hard-coded record-set or bib ids that might exist",
            "IntegrationTestOptions:EnableStaffProtectedTests=true, IntegrationTestOptions:EnableMutatingIntegrationTests=true, staff override credentials, and disposable TestSettings:RecordSetId/BibId fixture data",
            "call RecordSetContentPutAsync only for the configured disposable record set and records, assert the documented PAPIErrorCode, and restore/remove only content the test definitely changed");
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
