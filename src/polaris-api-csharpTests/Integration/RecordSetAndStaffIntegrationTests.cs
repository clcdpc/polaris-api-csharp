using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Clc.Polaris.Api.Tests.Integration;

[TestClass]
[TestCategory("Integration")]
public sealed class RecordSetAndStaffIntegrationTests : PapiIntegrationTestBase
{
    [TestMethod]
    public async Task RecordSetRecordsGet_WithInvalidRecordSet_ReturnsKnownError()
    {
        RequireStaffProtectedTestsEnabled();

        var response = await Client.RecordSetRecordsGetAsync(NonexistentId, Settings.EffectiveUserId, Settings.EffectiveWorkstationId);
        PapiIntegrationAssert.ErrorCode(response, -11001);
    }

    [TestMethod]
    public async Task RecordSetContentAdd_WithInvalidRecordSet_ReturnsKnownError()
    {
        RequireStaffProtectedTestsEnabled();

        var response = await Client.RecordSetContentAddAsync(NonexistentId, NonexistentId, Settings.EffectiveUserId, Settings.EffectiveWorkstationId);
        PapiIntegrationAssert.ErrorCode(response, -11001);
    }

    [TestMethod]
    public async Task RecordSetContentAddList_WithInvalidRecordSet_ReturnsKnownError()
    {
        RequireStaffProtectedTestsEnabled();

        var response = await Client.RecordSetContentAddAsync(NonexistentId, new[] { NonexistentId }, Settings.EffectiveUserId, Settings.EffectiveWorkstationId);
        PapiIntegrationAssert.ErrorCode(response, -11001);
    }

    [TestMethod]
    public async Task RecordSetContentRemove_WithInvalidRecordSet_ReturnsKnownError()
    {
        RequireStaffProtectedTestsEnabled();

        var response = await Client.RecordSetContentRemoveAsync(NonexistentId, NonexistentId, Settings.EffectiveUserId, Settings.EffectiveWorkstationId);
        PapiIntegrationAssert.ErrorCode(response, -11001);
    }

    [TestMethod]
    public async Task RecordSetContentRemoveList_WithInvalidRecordSet_ReturnsKnownError()
    {
        RequireStaffProtectedTestsEnabled();

        var response = await Client.RecordSetContentRemoveAsync(NonexistentId, new[] { NonexistentId }, Settings.EffectiveUserId, Settings.EffectiveWorkstationId);
        PapiIntegrationAssert.ErrorCode(response, -11001);
    }

    [TestMethod]
    public void RecordSetContentPut_WithRealRecordSet_RequiresScenarioData()
    {
        RequireScenarioDependentTestsEnabled(
            nameof(Client.RecordSetContentPutAsync),
            "a disposable record set id and record ids safe to add/remove",
            "add records, verify RecordSetRecordsGet contains them, remove them in finally, and assert PAPIErrorCode=0");
        Assert.Inconclusive("Record set mutation requires a disposable configured record set.");
    }

    [TestMethod]
    public async Task SaGetValueByOrg_WithOrgEmailAttribute_ReturnsConfiguredValueWhenProvided()
    {
        RequireStaffProtectedTestsEnabled();
        RequireOrgEmail();

        var response = await Client.SA_GetValueByOrgAsync("ORGEMAIL", Settings.EffectiveOrganizationId);
        var data = PapiIntegrationAssert.HasData(response);
        Assert.AreEqual(Settings.OrgEmail, data.Value);
    }

    [TestMethod]
    public async Task RemoteStorageItemsGet_WhenStaffEnabled_ReturnsDeserializedResponse()
    {
        RequireStaffProtectedTestsEnabled();

        var response = await Client.RemoteStorageItemsGetAsync(Settings.EffectiveBranchId, "1900-01-01", "1900-01-02", 1, 1);
        PapiIntegrationAssert.HasData(response);
    }
}
