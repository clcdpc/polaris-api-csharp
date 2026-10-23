namespace Clc.Polaris.Api.LiveIntegrationTests.ReadOnly.Authentication
{
    [TestClass]
    public sealed class ApiKeyValidateTests : IntegrationTestBase
    {
        [TestMethod]
        [ReadOnlyLiveTest]
        public async Task ApiKeyValidateTest()
        {
            var response = await Papi.ApiKeyValidateAsync(TestContext.CancellationToken);
            Assert.AreEqual(0, response.Data.PAPIErrorCode);
        }
    }
}
