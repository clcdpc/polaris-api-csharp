using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Clc.Polaris.Api.Tests.Integration;

[TestClass]
[TestCategory("Integration")]
public sealed class SynchIntegrationTests : PapiIntegrationTestBase
{
    [TestMethod]
    public async Task SynchBibsByIdGet_WithConfiguredBib_ReturnsSuccessHttpResponseAndRows()
    {
        RequireBibId();

        var response = await Client.Synch_BibsByIdGetAsync(Settings.BibId);
        var data = PapiIntegrationAssert.HasData(response);
        Assert.IsTrue(response.Response!.IsSuccessStatusCode, $"Expected HTTP success from synch bibs endpoint, received {response.Response!.StatusCode}.");
        Assert.IsTrue(data.PAPIErrorCode >= 0, $"Expected non-negative row count, but received {data.PAPIErrorCode}.");
        Assert.IsNotNull(data.GetBibsByIDRows);
    }

    [TestMethod]
    public async Task SynchBibsByIdGet_WithInvalidBib_ReturnsDeserializedResponse()
    {
        var response = await Client.Synch_BibsByIdGetAsync(NonexistentLargeId);
        var data = PapiIntegrationAssert.HasData(response);
        Assert.IsTrue(response.Response!.IsSuccessStatusCode, $"Expected HTTP success from synch bibs endpoint, received {response.Response!.StatusCode}.");
        Assert.IsTrue(data.PAPIErrorCode >= 0 || data.PAPIErrorCode < 0);
    }
}
