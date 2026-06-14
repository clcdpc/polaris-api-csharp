namespace Clc.Polaris.Api.LiveIntegrationTests
{
    [TestClass]
    public sealed class RecordSetRecordsGetTests : IntegrationTestBase
    {
        [TestMethod]
        [ProtectedReadOnlyLiveTest]
        public async Task RecordSetRecordsGetTest()
        {
            IntegrationTestRequirements.RequireStaffOverrideAccount(PapiSettings);

            var response = await Papi.RecordSetRecordsGetAsync(1234, cancellationToken: TestContext.CancellationToken);
            Assert.AreEqual(-11001, response.Data.PAPIErrorCode);
        }
    }
}
