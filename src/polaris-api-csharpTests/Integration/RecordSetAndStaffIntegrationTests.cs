using Clc.Polaris.Api.Models;
using Clc.Polaris.Api.Tests.Integration.Infrastructure;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Clc.Polaris.Api.Tests.Integration;

[TestClass]
[TestCategory("Integration")]
public sealed class RecordSetAndStaffIntegrationTests : PapiIntegrationTestBase
{
    [TestMethod]
    public async Task RecordSetContentAdd_WithInvalidRecordSet_ReturnsKnownPapiError()
    {
        var response = await Papi.RecordSetContentAddAsync(InvalidRecordSetId, InvalidRecordId, EffectiveUserId, EffectiveWorkstationId);
        PapiAssert.PapiError(response, -11001);
    }

    [TestMethod]
    public async Task RecordSetContentAddList_WithInvalidRecordSet_ReturnsKnownPapiError()
    {
        var response = await Papi.RecordSetContentAddAsync(InvalidRecordSetId, new[] { InvalidRecordId }, EffectiveUserId, EffectiveWorkstationId);
        PapiAssert.PapiError(response, -11001);
    }

    [TestMethod]
    public async Task RecordSetContentRemove_WithInvalidRecordSet_ReturnsKnownPapiError()
    {
        var response = await Papi.RecordSetContentRemoveAsync(InvalidRecordSetId, InvalidRecordId, EffectiveUserId, EffectiveWorkstationId);
        PapiAssert.PapiError(response, -11001);
    }

    [TestMethod]
    public async Task RecordSetContentRemoveList_WithInvalidRecordSet_ReturnsKnownPapiError()
    {
        var response = await Papi.RecordSetContentRemoveAsync(InvalidRecordSetId, new[] { InvalidRecordId }, EffectiveUserId, EffectiveWorkstationId);
        PapiAssert.PapiError(response, -11001);
    }

    [TestMethod]
    public async Task RecordSetContentPut_WithInvalidRecordSet_ReturnsKnownPapiError()
    {
        var response = await Papi.RecordSetContentPutAsync(InvalidRecordSetId, new[] { InvalidRecordId }, RecordSetContentPutActions.Add, EffectiveUserId, EffectiveWorkstationId);
        PapiAssert.PapiError(response, -11001);
    }

    [TestMethod]
    public async Task RecordSetRecordsGet_WithInvalidRecordSet_ReturnsKnownPapiError()
    {
        var response = await Papi.RecordSetRecordsGetAsync(InvalidRecordSetId, EffectiveUserId, EffectiveWorkstationId);
        PapiAssert.PapiError(response, -11001);
    }

    [TestMethod]
    public async Task SaGetValueByOrg_WithOrgEmailAttribute_ReturnsConfiguredValueWhenProvided()
    {
        RequireNonEmpty(Settings.OrgEmail, "TestSettings:OrgEmail");

        var response = await Papi.SA_GetValueByOrgAsync("ORGEMAIL", EffectiveOrganizationId);
        var data = PapiAssert.HasData(response);
        Assert.AreEqual(Settings.OrgEmail, data.Value);
    }

    [TestMethod]
    public async Task NotificationQueueGet_WhenStaffProtectedEnabled_ReturnsDeserializedResponse()
    {
        RequireStaffProtectedTestsEnabled();
        RequireOrganizationId();

        var response = await Papi.NotificationQueueGetAsync(EffectiveOrganizationId);
        PapiAssert.HasData(response);
    }

    [TestMethod]
    public async Task RemoteStorageItemsGet_WhenScenarioEnabled_ReturnsDeserializedResponse()
    {
        RequireStaffProtectedTestsEnabled();
        RequireScenarioDependentTestsEnabled("RemoteStorageItemsGetAsync requires branch/date/list-type fixture values known to be valid for the target Polaris site.");
        RequireBranchId();
        RequireNonEmpty(Settings.RemoteStorageStartDate, "TestSettings:RemoteStorageStartDate");
        RequireNonEmpty(Settings.RemoteStorageEndDate, "TestSettings:RemoteStorageEndDate");

        var response = await Papi.RemoteStorageItemsGetAsync(EffectiveBranchId, Settings.RemoteStorageStartDate, Settings.RemoteStorageEndDate, maxItems: 1, Settings.RemoteStorageListType);
        PapiAssert.HasData(response);
    }
}
