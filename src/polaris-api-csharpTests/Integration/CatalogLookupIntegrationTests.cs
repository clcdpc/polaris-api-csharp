using Clc.Polaris.Api.Models;
using Clc.Polaris.Api.Tests.Integration.Infrastructure;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Clc.Polaris.Api.Tests.Integration;

[TestClass]
[TestCategory("Integration")]
public sealed class CatalogLookupIntegrationTests : PapiIntegrationTestBase
{
    [TestMethod]
    public async Task BibGet_WithConfiguredBib_ReturnsRecordAndUsesDefaultBranchRoute()
    {
        RequireBibId();

        var response = await Papi.BibGetAsync(Settings.BibId);
        var data = PapiAssert.Success(response);
        Assert.IsTrue(data.BibGetRows.Count > 0, "Expected bibliographic rows.");
        PapiAssert.RequestUriContains(response, $"/100/{PapiSettings.OrganizationId}/bib/{Settings.BibId}");
    }

    [TestMethod]
    public async Task BibGet_WithConfiguredBibAndBranch_ReturnsRecordAndUsesBranchRoute()
    {
        RequireBibId();
        RequireBranchId();

        var response = await Papi.BibGetAsync(Settings.BibId, EffectiveBranchId);
        var data = PapiAssert.Success(response);
        Assert.IsTrue(data.BibGetRows.Count > 0, "Expected bibliographic rows.");
        PapiAssert.RequestUriContains(response, $"/100/{EffectiveBranchId}/bib/{Settings.BibId}");
    }

    [TestMethod]
    public async Task BibGet_WithInvalidBib_ReturnsDocumentedReachabilityError()
    {
        var response = await Papi.BibGetAsync(InvalidBibId);
        PapiAssert.PapiError(response, -3000);
    }

    [TestMethod]
    public async Task BibSearch_WithConfiguredTerm_ReturnsDeserializedRows()
    {
        var response = await Papi.BibSearchAsync(new BibSearchOptions { Term = Settings.BibSearchTerm, PageSize = 10 });
        var data = PapiAssert.HasData(response);
        Assert.AreEqual(data.BibSearchRows.Count, data.PAPIErrorCode, "Expected success PAPIErrorCode to equal the page row count.");
        Assert.IsTrue(data.PAPIErrorCode >= 0, $"Expected non-negative search row count, received {data.PAPIErrorCode}: {data.ErrorMessage}");
    }

    [TestMethod]
    public async Task BibKeywordSearch_WithConfiguredTerm_ReturnsDeserializedRows()
    {
        var response = await Papi.BibKeywordSearchAsync(Settings.BibSearchTerm, pageSize: 5);
        var data = PapiAssert.HasData(response);
        Assert.IsTrue(data.PAPIErrorCode >= 0, $"Expected non-negative search row count, received {data.PAPIErrorCode}: {data.ErrorMessage}");
        Assert.AreEqual(data.BibSearchRows.Count, data.PAPIErrorCode);
    }

    [TestMethod]
    public async Task BibBooleanSearch_WithConfiguredCcl_ReturnsDeserializedRows()
    {
        var response = await Papi.BibBooleanSearchAsync(Settings.BibBooleanSearch, pageSize: 5);
        var data = PapiAssert.HasData(response);
        Assert.IsTrue(data.PAPIErrorCode >= 0, $"Expected non-negative search row count, received {data.PAPIErrorCode}: {data.ErrorMessage}");
        Assert.AreEqual(data.BibSearchRows.Count, data.PAPIErrorCode);
    }

    [TestMethod]
    public async Task HoldingsGet_WithConfiguredBib_ReturnsHoldingsPayload()
    {
        RequireBibId();

        var response = await Papi.HoldingsGetAsync(Settings.BibId);
        var data = PapiAssert.HasData(response);
        Assert.IsTrue(data.PAPIErrorCode >= 0, $"Expected holdings to return a non-negative row count, received {data.PAPIErrorCode}: {data.ErrorMessage}");
    }

    [TestMethod]
    public void HeadingsSearch_IsNotImplementedInClient()
    {
        Assert.ThrowsException<NotImplementedException>(() => Papi.HeadingsSearchAsync(Settings.BibId));
    }

    [TestMethod]
    public async Task SynchBibsById_WithConfiguredBib_ReturnsSuccessHttpPayload()
    {
        RequireBibId();

        var response = await Papi.Synch_BibsByIdGetAsync(Settings.BibId);
        PapiAssert.HasData(response);
        Assert.IsTrue(response.Response!.IsSuccessStatusCode, "Expected synch bib request to reach PAPI successfully.");
    }
}
