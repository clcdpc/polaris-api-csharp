using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Clc.Polaris.Api.Tests
{
    [TestClass]
    public sealed class RecordSetContentRemoveTests : IntegrationTestBase
    {
        [TestMethod]
        [ProtectedMutatingIntegrationTest]
        [DoNotParallelize]
        public async Task RecordSetContentRemoveTest()
        {
            IntegrationTestRequirements.RequireStaffOverrideAccount(PapiSettings);

            var response = await Papi.RecordSetContentRemoveAsync(1234, 1234, cancellationToken: TestContext.CancellationToken);
            Assert.AreEqual(-11001, response.Data.PAPIErrorCode);
        }

        [TestMethod]
        [ProtectedMutatingIntegrationTest]
        [DoNotParallelize]
        public async Task RecordSetContentRemoveTest_List()
        {
            IntegrationTestRequirements.RequireStaffOverrideAccount(PapiSettings);

            var response = await Papi.RecordSetContentRemoveAsync(1234, [1234], cancellationToken: TestContext.CancellationToken);
            Assert.AreEqual(-11001, response.Data.PAPIErrorCode);
        }
    }
}
