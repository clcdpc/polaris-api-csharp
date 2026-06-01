using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Clc.Polaris.Api;

[TestClass]
[TestCategory("Integration")]
public sealed class RecordSetAndRemoteStorageIntegrationTests : PapiIntegrationTestBase
{
    [TestMethod]
    public async Task RecordSet_content_methods_reach_api_with_nonexistent_record_set()
    {
        PapiIntegrationAssert.PapiError(await Papi.RecordSetContentAddAsync(NonexistentId, NonexistentId), -11001);
        PapiIntegrationAssert.PapiError(await Papi.RecordSetContentAddAsync(NonexistentId, new[] { NonexistentId }), -11001);
        PapiIntegrationAssert.PapiError(await Papi.RecordSetContentRemoveAsync(NonexistentId, NonexistentId), -11001);
        PapiIntegrationAssert.PapiError(await Papi.RecordSetContentRemoveAsync(NonexistentId, new[] { NonexistentId }), -11001);
        PapiIntegrationAssert.PapiError(await Papi.RecordSetRecordsGetAsync(NonexistentId), -11001);
    }

    [TestMethod]
    public async Task RecordSetRecordsGet_configured_record_set_requires_scenario_data()
    {
        RequireScenarioDependentTestsEnabled("TestSettings:RecordSetId containing known records");
        if (Settings.RecordSetId <= 0)
        {
            InconclusivePlaceholder(
                nameof(Papi.RecordSetRecordsGetAsync),
                "a success path requires a real record set whose contents may vary by library",
                "TestSettings:RecordSetId, TestSettings:UserId, TestSettings:WorkstationId",
                "assert PAPIErrorCode is non-negative and rows deserialize; for a stable fixture assert row count boundaries");
        }

        var data = PapiIntegrationAssert.HasData(await Papi.RecordSetRecordsGetAsync(Settings.RecordSetId, Settings.UserId, Settings.WorkstationId));
        Assert.IsTrue(data.PAPIErrorCode >= 0, data.ErrorMessage);
    }

    [TestMethod]
    public async Task RemoteStorageItemsGet_requires_scenario_data()
    {
        RequireScenarioDependentTestsEnabled("a branch with remote storage activity and a date range");
        if (Settings.BranchId <= 0)
        {
            InconclusivePlaceholder(
                nameof(Papi.RemoteStorageItemsGetAsync),
                "remote-storage data depends on local branch configuration and activity dates",
                "TestSettings:BranchId plus a known safe date range (currently use recent dates only after validation)",
                "assert response deserializes and PAPIErrorCode is non-negative or a documented no-data error");
        }

        var today = DateTime.UtcNow.ToString("yyyy-MM-dd");
        var data = PapiIntegrationAssert.HasData(await Papi.RemoteStorageItemsGetAsync(Settings.BranchId, today, today, 1, 1));
        Assert.IsTrue(data.PAPIErrorCode >= 0 || data.PAPIErrorCode == -1, data.ErrorMessage);
    }
}
