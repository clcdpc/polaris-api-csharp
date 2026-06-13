namespace Clc.Polaris.Api.Tests
{
    [TestClass]
    public sealed class Synch_BibsByIdGetTests : IntegrationTestBase
    {
        [TestMethod]
        [ProtectedReadOnlyIntegrationTest]
        public async Task Synch_BibsByIdGetTest()
        {
            IntegrationTestRequirements.RequireStaffOverrideAccount(PapiSettings);

            var response = await Papi.Synch_BibsByIdGetAsync(478907, cancellationToken: TestContext.CancellationToken);
            Assert.IsTrue(response.Response.IsSuccessStatusCode);
        }
    }
}
