namespace Clc.Polaris.Api.Tests
{
    [TestClass]
    public sealed class Synch_BibsByIdGetTests : IntegrationTestBase
    {
        [TestMethod]
        [ProtectedReadOnlyIntegrationTest]
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
