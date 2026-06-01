using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Clc.Polaris.Api.Tests.Integration;

[TestClass]
[TestCategory("Integration")]
public sealed class SynchIntegrationTests : IntegrationTestBase
{
    [TestMethod]
    public async Task Synch_BibsByIdGetAsync_WithConfiguredBib_ReturnsSynchronisationPayload()
    {
        RequireBibScenario();

        var response = await Papi.Synch_BibsByIdGetAsync(Settings.BibId);
        var data = PapiIntegrationAssert.HasData(response);

        Assert.IsNotNull(response.Response);
        Assert.IsTrue(response.Response!.IsSuccessStatusCode);
        Assert.IsTrue(data.PAPIErrorCode >= 0, data.ErrorMessage?.ToString());
    }
}
