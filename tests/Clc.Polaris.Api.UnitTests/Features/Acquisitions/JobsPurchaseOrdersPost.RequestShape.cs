namespace Clc.Polaris.Api.UnitTests.Features.Acquisitions
{
    [TestClass]
    [UnitTest]
    public sealed class JobsPurchaseOrdersPostRequestShape : PapiClientUnitTestBase
    {
        [TestMethod]
        public async Task JobsPurchaseOrdersPostAsync_SendsProtectedPostWithJsonBody()
        {
            var handler = new CapturingHttpMessageHandler(CreatePapiResponseJson()); var client = CreateConfiguredClient(handler); client.Token = CreateToken();
            await client.JobsPurchaseOrdersPostAsync(new JobsPurchaseOrdersCreateData { PONumber = "PO-1" }, cancellationToken: TestContext.CancellationToken);
            Assert.AreEqual(HttpMethod.Post, GetLastRequest(handler).Method);
            Assert.AreEqual("/PAPIService/REST/protected/v1/1033/100/101/protected-token/jobs/purchaseorders", GetLastRequestUri(handler).AbsolutePath);
            AssertLastRequestBodyContains(handler, "PO-1");
        }
        [TestMethod] public async Task JobsPurchaseOrdersPostAsync_WithNullRequest_Throws() => await Assert.ThrowsExactlyAsync<ArgumentNullException>(async () => await CreateClient().JobsPurchaseOrdersPostAsync(null!, cancellationToken: TestContext.CancellationToken));
        private static PapiClient CreateConfiguredClient(CapturingHttpMessageHandler handler) { var client = CreateClient(handler); client.OrganizationId = 101; return client; }
        private static ProtectedToken CreateToken() => new() { AccessToken = "protected-token", AccessSecret = "protected-secret", ExpirationDate = ValidProtectedTokenExpirationDate };
    }
}
