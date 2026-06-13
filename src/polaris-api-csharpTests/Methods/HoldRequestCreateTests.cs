namespace Clc.Polaris.Api.Tests
{
    [TestClass]
    public sealed class HoldRequestCreateTests : IntegrationTestBase
    {
        [TestMethod]
        [MutatingIntegrationTest]
        [DoNotParallelize]
        public async Task HoldRequestCreateTest()
        {
            var response = await Papi.HoldRequestCreateAsync(new HoldRequestCreateParams(Settings.PatronId, 1234, 7, 7), TestContext.CancellationToken);
            Assert.AreEqual(-4006, response.Data.PAPIErrorCode);
        }

        [TestMethod]
        [MutatingIntegrationTest]
        [DoNotParallelize]
        public async Task HoldRequestCreateTest2()
        {
            var response = await ((PapiClient)Papi).HoldRequestCreateAsync(Settings.PatronId, 1234, 7, cancellationToken: TestContext.CancellationToken);
            Assert.AreEqual(-4006, response.Data.PAPIErrorCode);
        }
    }
}
