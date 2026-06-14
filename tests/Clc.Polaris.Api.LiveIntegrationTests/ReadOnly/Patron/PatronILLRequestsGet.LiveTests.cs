namespace Clc.Polaris.Api.LiveIntegrationTests
{
    [TestClass]
    public sealed class PatronILLRequestsGetTests : IntegrationTestBase
    {
        [TestMethod]
        [ReadOnlyLiveTest]
        public async Task PatronILLRequestsGetTest()
        {
            var response = await Papi.PatronILLRequestsGetAsync(Settings.PatronBarcode, password: Settings.PatronPin, cancellationToken: TestContext.CancellationToken);
            Assert.AreEqual(0, response.Data.PAPIErrorCode);
        }
    }
}
