namespace Clc.Polaris.Api.UnitTests.EndpointRequests.SystemAdministration
{
    [TestClass]
    [UnitTest]
    public sealed class SortOptionsGetTests : PapiClientUnitTestBase
    {
        [TestMethod]
        public async Task SortOptionsGetAsync_SendsExpectedRequest()
        {
            var handler = new CapturingHttpMessageHandler(CreatePapiResponseJson());
            var client = CreateClient(handler);

            await client.SortOptionsGetAsync(branchId: 88, cancellationToken: TestContext.CancellationToken);

            Assert.AreEqual(HttpMethod.Get, GetLastRequest(handler).Method);
            Assert.AreEqual("/PAPIService/REST/public/v1/1033/100/88/sortoptions", GetLastRequestUri(handler).AbsolutePath);
        }
    }
}
