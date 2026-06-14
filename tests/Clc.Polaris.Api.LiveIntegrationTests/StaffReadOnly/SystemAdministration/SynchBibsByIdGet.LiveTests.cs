namespace Clc.Polaris.Api.LiveIntegrationTests
{
    [TestClass]
    public sealed class Synch_BibsByIdGetTests : IntegrationTestBase
    {
        [TestMethod]
        [StaffReadOnlyLiveTest]
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
