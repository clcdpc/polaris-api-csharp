namespace Clc.Polaris.Api.LiveIntegrationTests.ReadOnly.Authentication
{
    [TestClass]
    public sealed class ApiVersionGetTests : IntegrationTestBase
    {
        [TestMethod]
        [ReadOnlyLiveTest]
        public async Task ApiVersionGetTest()
        {
            var response = await Papi.ApiVersionGetAsync(TestContext.CancellationToken);
            Assert.AreEqual(0, response.Data.PAPIErrorCode);
            Assert.IsFalse(string.IsNullOrWhiteSpace(response.Data.ToString()));
        }
    }
}
