using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Clc.Polaris.Api.Tests
{
    [TestClass]
    public sealed class RecordSetContentRemoveTests : IntegrationTestBase
    {
        [TestMethod]
        [ProtectedMutatingIntegrationCategory]
        [DoNotParallelize]
        public async Task RecordSetContentRemoveTest()
        {
            IntegrationTestRequirements.RequireStaffOverrideAccount(PapiSettings);

            var response = await Papi.RecordSetContentRemoveAsync(1234, 1234);
            Assert.IsTrue(response.Data.PAPIErrorCode == -11001);
        }

        [TestMethod]
        [ProtectedMutatingIntegrationCategory]
        [DoNotParallelize]
        public async Task RecordSetContentRemoveTest_List()
        {
            IntegrationTestRequirements.RequireStaffOverrideAccount(PapiSettings);

            var response = await Papi.RecordSetContentRemoveAsync(1234, new[] { 1234 });
            Assert.IsTrue(response.Data.PAPIErrorCode == -11001);
        }
    }
}
