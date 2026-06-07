using Microsoft.VisualStudio.TestTools.UnitTesting;

#pragma warning disable CS8602 // Dereference of a possibly null reference.

namespace Clc.Polaris.Api.Tests
{
    [TestClass]
    public sealed class RecordSetContentAddTests : IntegrationTestBase
    {
        [TestMethod]
        [ProtectedMutatingIntegrationCategory]
        [DoNotParallelize]
        public async Task RecordSetContentAddTest()
        {
            IntegrationTestRequirements.RequireStaffOverrideAccount(PapiSettings);

            var response = await Papi.RecordSetContentAddAsync(1234, 1234);
            Assert.IsTrue(response.Data.PAPIErrorCode == -11001);
        }

        [TestMethod]
        [ProtectedMutatingIntegrationCategory]
        [DoNotParallelize]
        public async Task RecordSetContentAddTest_List()
        {
            IntegrationTestRequirements.RequireStaffOverrideAccount(PapiSettings);

            var response = await Papi.RecordSetContentAddAsync(1234, new[] { 1234 });
            Assert.IsTrue(response.Data.PAPIErrorCode == -11001);
        }
    }
}
