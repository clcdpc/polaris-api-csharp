using Clc.Polaris.Api.Tests.Integration.Infrastructure;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Clc.Polaris.Api.Tests.Integration;

[TestClass]
[TestCategory("Integration")]
public sealed class PatronTitleListIntegrationTests : PapiIntegrationTestBase
{
    [TestMethod]
    public async Task PatronTitleListAddTitle_WithInvalidList_ReturnsKnownPapiError()
    {
        RequirePatronCredentials();

        var response = await Papi.PatronTitleListAddTitleAsync(Settings.PatronBarcode, InvalidRecordSetId, InvalidBibId, Settings.PatronPin);
        var data = PapiAssert.HasData(response);
        Assert.AreEqual(-1, data.PAPIErrorCode);
    }

    [TestMethod]
    public async Task PatronTitleListCopyAllTitles_WithInvalidLists_ReturnsKnownPapiError()
    {
        RequirePatronCredentials();

        var response = await Papi.PatronTitleListCopyAllTitlesAsync(Settings.PatronBarcode, InvalidRecordSetId, InvalidRecordSetId + 1, Settings.PatronPin);
        PapiAssert.PapiError(response, -1);
    }

    [TestMethod]
    public async Task PatronTitleListCopyTitle_WithInvalidLists_ReturnsKnownPapiError()
    {
        RequirePatronCredentials();

        var response = await Papi.PatronTitleListCopyTitleAsync(Settings.PatronBarcode, InvalidRecordSetId, 1, InvalidRecordSetId + 1, Settings.PatronPin);
        PapiAssert.PapiError(response, -1);
    }

    [TestMethod]
    public async Task PatronTitleListDeleteAllTitles_WithInvalidList_ReturnsKnownPapiError()
    {
        RequirePatronCredentials();

        var response = await Papi.PatronTitleListDeleteAllTitlesAsync(Settings.PatronBarcode, InvalidRecordSetId, Settings.PatronPin);
        PapiAssert.PapiError(response, -1);
    }

    [TestMethod]
    public async Task PatronTitleListDeleteTitle_WithInvalidList_ReturnsKnownPapiError()
    {
        RequirePatronCredentials();

        var response = await Papi.PatronTitleListDeleteTitleAsync(Settings.PatronBarcode, InvalidRecordSetId, 1, Settings.PatronPin);
        PapiAssert.PapiError(response, -1);
    }

    [TestMethod]
    public async Task PatronTitleListGetTitles_WithInvalidList_ReturnsKnownPapiError()
    {
        RequirePatronCredentials();

        var response = await Papi.PatronTitleListGetTitlesAsync(Settings.PatronBarcode, InvalidRecordSetId, password: Settings.PatronPin);
        var data = PapiAssert.HasData(response);
        Assert.AreEqual(-1, data.PAPIErrorCode);
    }

    [TestMethod]
    public async Task PatronTitleListMoveTitle_WithInvalidLists_ReturnsKnownPapiError()
    {
        RequirePatronCredentials();

        var response = await Papi.PatronTitleListMoveTitleAsync(Settings.PatronBarcode, InvalidRecordSetId, 1, InvalidRecordSetId + 1, Settings.PatronPin);
        PapiAssert.PapiError(response, -1);
    }
}
