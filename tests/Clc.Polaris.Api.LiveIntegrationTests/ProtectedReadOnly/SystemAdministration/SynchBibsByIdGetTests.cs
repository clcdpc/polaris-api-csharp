namespace Clc.Polaris.Api.LiveIntegrationTests.ProtectedReadOnly.SystemAdministration
{
    [TestClass]
    public sealed class SynchBibsByIdGetTests : IntegrationTestBase
    {
        [TestMethod]
        [ProtectedReadOnlyLiveTest]
        public async Task Synch_BibsByIdGetAsync_ReturnsConfiguredBib()
        {
            IntegrationTestRequirements.RequireStaffOverrideAccount(PapiSettings);
            var bibId = RequireConfiguredBib();

            var response = await Papi.Synch_BibsByIdGetAsync([bibId], includeItems: true, cancellationToken: TestContext.CancellationToken);

            Assert.IsTrue(response.Response.IsSuccessStatusCode);
            Assert.AreEqual(0, response.Data.PAPIErrorCode);
            Assert.IsNotNull(response.Data.GetBibsByIDRows.SingleOrDefault(row => row.BibliographicRecordID == bibId));
        }
    }
}
