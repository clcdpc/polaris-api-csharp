using Clc.Polaris.Api.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Clc.Polaris.Api.Tests.Integration;

[TestClass]
[TestCategory("Integration")]
public sealed class CatalogLookupIntegrationTests : IntegrationTestBase
{
    [TestMethod]
    public async Task BibGetAsync_WithConfiguredBib_ReturnsBibliographicRecord()
    {
        RequireBibScenario();

        var response = await Papi.BibGetAsync(Settings.BibId);
        var data = PapiIntegrationAssert.Success(response);

        Assert.IsFalse(string.IsNullOrWhiteSpace(data.Title));
        PapiIntegrationAssert.RequestUriContains(response, $"/bib/{Settings.BibId}");
    }

    [TestMethod]
    public async Task BibGetAsync_WithConfiguredBibAndBranch_UsesBranchRoute()
    {
        RequireBibScenario();
        RequireBranchScenario();

        var response = await Papi.BibGetAsync(Settings.BibId, Settings.BranchId);
        var data = PapiIntegrationAssert.Success(response);

        Assert.IsFalse(string.IsNullOrWhiteSpace(data.Title));
        PapiIntegrationAssert.RequestUriContains(response, $"/{Settings.BranchId}/bib/{Settings.BibId}");
    }

    [TestMethod]
    public async Task BibSearchAsync_WithStableKeyword_ReturnsDeserializedRowsOrEmptyPage()
    {
        RequirePapiConfiguration();

        var response = await Papi.BibSearchAsync(new BibSearchOptions
        {
            Branch = OrganizationIdOrConfigured,
            Term = "dogs",
            PageSize = 10
        });
        var data = PapiIntegrationAssert.HasPapiData(response);

        Assert.AreEqual(data.BibSearchRows.Count, data.PAPIErrorCode);
        Assert.IsTrue(data.PAPIErrorCode >= 0);
        PapiIntegrationAssert.RequestUriContains(response, "/search/bibs/keyword");
    }

    [TestMethod]
    public async Task BibKeywordSearchAsync_WithStableKeyword_ReturnsDeserializedResult()
    {
        RequirePapiConfiguration();

        var response = await Papi.BibKeywordSearchAsync("dogs", OrganizationIdOrConfigured, pageSize: 5);
        var data = PapiIntegrationAssert.HasPapiData(response);

        Assert.AreEqual(data.BibSearchRows.Count, data.PAPIErrorCode);
        Assert.IsTrue(data.PAPIErrorCode >= 0);
    }

    [TestMethod]
    public async Task BibBooleanSearchAsync_WithStableExpression_ReturnsDeserializedResult()
    {
        RequirePapiConfiguration();

        var response = await Papi.BibBooleanSearchAsync("KW=dogs", OrganizationIdOrConfigured, pageSize: 5);
        var data = PapiIntegrationAssert.HasPapiData(response);

        Assert.IsTrue(data.PAPIErrorCode >= 0, $"Boolean search should return a row count or success code. Error: {data.ErrorMessage}");
    }

    [TestMethod]
    public async Task HoldingsGetAsync_WithConfiguredBib_ReturnsHoldingsResult()
    {
        RequireBibScenario();

        var response = await Papi.HoldingsGetAsync(Settings.BibId);
        var data = PapiIntegrationAssert.HasPapiData(response);

        Assert.IsTrue(data.PAPIErrorCode >= 0, data.ErrorMessage);
        Assert.IsNotNull(data.BibHoldingsGetRows);
    }

    [TestMethod]
    public void HeadingsSearchAsync_IsDocumentedPlaceholderUntilClientImplementsEndpoint()
    {
        RequirePapiConfiguration();

        Assert.ThrowsException<NotImplementedException>(() => Papi.HeadingsSearchAsync(NonexistentBibId));
    }

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
