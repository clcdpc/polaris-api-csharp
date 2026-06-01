using Clc.Polaris.Api.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Clc.Polaris.Api.Tests.Integration;

[TestClass]
[TestCategory("Integration")]
public sealed class CatalogLookupIntegrationTests : PapiIntegrationTestBase
{
    [TestMethod]
    public async Task BibGet_WithConfiguredBib_ReturnsTitleAndUsesExpectedRoute()
    {
        RequireBibId();

        var response = await Client.BibGetAsync(Settings.BibId);
        var data = PapiIntegrationAssert.Success(response);
        Assert.IsFalse(string.IsNullOrWhiteSpace(data.Title));
        PapiIntegrationAssert.RequestUriContains(response, "/bib/");
    }

    [TestMethod]
    public async Task BibGet_WithConfiguredBibAndBranch_UsesBranchRoute()
    {
        RequireBibId();
        RequireBranchId();

        var response = await Client.BibGetAsync(Settings.BibId, Settings.BranchId);
        var data = PapiIntegrationAssert.Success(response);
        Assert.IsFalse(string.IsNullOrWhiteSpace(data.Title));
        PapiIntegrationAssert.RequestUriContains(response, $"/{Settings.BranchId}/bib/");
    }

    [TestMethod]
    public async Task BibGet_WithNonexistentBib_ReturnsDeserializedKnownError()
    {
        var response = await Client.BibGetAsync(NonexistentLargeId);
        var data = PapiIntegrationAssert.HasData(response);
        Assert.IsTrue(data.PAPIErrorCode <= 0, $"Expected nonexistent bib lookup to return success with empty/error shape or a negative PAPI code, but received {data.PAPIErrorCode}.");
    }

    [TestMethod]
    public async Task BibSearch_WithStableKeyword_ReturnsRowsAndCount()
    {
        var response = await Client.BibSearchAsync(new BibSearchOptions { Term = "dogs", PageSize = 10 });
        var data = PapiIntegrationAssert.HasData(response);
        Assert.IsTrue(data.PAPIErrorCode >= 0, $"Expected non-negative row count, but received {data.PAPIErrorCode}: {data.ErrorMessage}");
        Assert.IsNotNull(data.BibSearchRows);
    }

    [TestMethod]
    public async Task BibKeywordSearch_WithStableKeyword_ReturnsDeserializedRows()
    {
        var response = await Client.BibKeywordSearchAsync("dogs", pageSize: 10);
        var data = PapiIntegrationAssert.HasData(response);
        Assert.IsTrue(data.PAPIErrorCode >= 0, $"Expected non-negative row count, but received {data.PAPIErrorCode}: {data.ErrorMessage}");
        Assert.IsNotNull(data.BibSearchRows);
    }

    [TestMethod]
    public async Task BibBooleanSearch_WithSimpleCcl_ReturnsDeserializedRows()
    {
        var response = await Client.BibBooleanSearchAsync("KW=dogs", pageSize: 10);
        var data = PapiIntegrationAssert.HasData(response);
        Assert.IsTrue(data.PAPIErrorCode >= 0, $"Expected non-negative row count, but received {data.PAPIErrorCode}: {data.ErrorMessage}");
        Assert.IsNotNull(data.BibSearchRows);
    }

    [TestMethod]
    public async Task HoldingsGet_WithConfiguredBib_ReturnsRows()
    {
        RequireBibId();

        var response = await Client.HoldingsGetAsync(Settings.BibId);
        var data = PapiIntegrationAssert.HasData(response);
        Assert.IsTrue(data.PAPIErrorCode >= 0, $"Expected non-negative row count, but received {data.PAPIErrorCode}: {data.ErrorMessage}");
        Assert.IsNotNull(data.BibHoldingsGetRows);
    }

    [TestMethod]
    public void HeadingsSearch_RemainsNotImplementedInClient()
    {
        Assert.ThrowsException<NotImplementedException>(() => new PapiClient().HeadingsSearchAsync(NonexistentId));
    }
}
