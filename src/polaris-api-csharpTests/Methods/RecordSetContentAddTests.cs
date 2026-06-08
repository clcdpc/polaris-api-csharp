using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Clc.Polaris.Api.Tests
{
    [TestClass]
    public sealed class RecordSetContentAddTests : IntegrationTestBase
    {
        [TestMethod]
        [ProtectedMutatingIntegrationTest]
        [DoNotParallelize]
        public async Task RecordSetContentAddTest()
        {
            IntegrationTestRequirements.RequireStaffOverrideAccount(PapiSettings);

            var response = await Papi.RecordSetContentAddAsync(1234, 1234);
            Assert.IsTrue(response.Data.PAPIErrorCode == -11001);
        }

        [TestMethod]
        [ProtectedMutatingIntegrationTest]
        [DoNotParallelize]
        public async Task RecordSetContentAddTest_List()
        {
            IntegrationTestRequirements.RequireStaffOverrideAccount(PapiSettings);

            var response = await Papi.RecordSetContentAddAsync(1234, new[] { 1234 });
            Assert.IsTrue(response.Data.PAPIErrorCode == -11001);
        }
    }
}
