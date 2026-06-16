namespace Clc.Polaris.Api.UnitTests.Features.Circulation
{
    [TestClass]
    [UnitTest]
    public sealed class SysHoldStatusesGetRequestShape : PapiClientUnitTestBase
    {
        [TestMethod]
        public async Task SysHoldStatusesGetAsync_SendsExpectedRequest()
        {
            var handler = new CapturingHttpMessageHandler(CreatePapiResponseJson());
            var client = CreateClient(handler);

            await client.SysHoldStatusesGetAsync(branchId: 88, cancellationToken: TestContext.CancellationToken);

            Assert.AreEqual(HttpMethod.Get, GetLastRequest(handler).Method);
            Assert.AreEqual("/PAPIService/REST/public/v1/1033/100/88/sysholdstatuses", GetLastRequestUri(handler).AbsolutePath);
        }
    }
}
