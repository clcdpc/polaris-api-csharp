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

            var response = await Papi.RecordSetContentAddAsync(1234, 1234, cancellationToken: TestContext.CancellationToken);
            Assert.AreEqual(-11001, response.Data.PAPIErrorCode);
        }

        [TestMethod]
        [ProtectedMutatingIntegrationTest]
        [DoNotParallelize]
        public async Task RecordSetContentAddTest_List()
        {
            IntegrationTestRequirements.RequireStaffOverrideAccount(PapiSettings);

            var response = await Papi.RecordSetContentAddAsync(1234, [1234], cancellationToken: TestContext.CancellationToken);
            Assert.AreEqual(-11001, response.Data.PAPIErrorCode);
        }
    }
}
