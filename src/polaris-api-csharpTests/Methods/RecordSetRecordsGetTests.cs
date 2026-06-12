using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Clc.Polaris.Api.Tests
{
    [TestClass]
    public sealed class RecordSetRecordsGetTests : IntegrationTestBase
    {
        [TestMethod]
        [ProtectedReadOnlyIntegrationTest]
        public async Task RecordSetRecordsGetTest()
        {
            IntegrationTestRequirements.RequireStaffOverrideAccount(PapiSettings);

            var response = await Papi.RecordSetRecordsGetAsync(1234, cancellationToken: TestContext.CancellationToken);
            Assert.AreEqual(-11001, response.Data.PAPIErrorCode);
        }
    }
}
