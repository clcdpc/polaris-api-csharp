using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Clc.Polaris.Api.Tests
{
    [TestClass]
    public sealed class RecordSetRecordsGetTests : IntegrationTestBase
    {
        [TestMethod]
        [ProtectedReadOnlyIntegrationCategory]
        public async Task RecordSetRecordsGetTest()
        {
            IntegrationTestRequirements.RequireStaffOverrideAccount(PapiSettings);

            var response = await Papi.RecordSetRecordsGetAsync(1234);
            Assert.IsTrue(response.Data.PAPIErrorCode == -11001);
        }
    }
}
