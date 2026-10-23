namespace Clc.Polaris.Api.UnitTests.EndpointRequests.Acquisitions
{
    [TestClass]
    [UnitTest]
    public sealed class JobsPurchaseOrdersResultGetTests : PapiClientUnitTestBase
    {
        [TestMethod]
        public async Task JobsPurchaseOrdersResultGetAsync_SendsProtectedGetRequest()
        {
            var handler = new CapturingHttpMessageHandler(CreatePapiResponseJson()); var client = CreateConfiguredClient(handler); client.Token = CreateToken(); var jobGuid = Guid.Parse("11111111-1111-1111-1111-111111111111");
            await client.JobsPurchaseOrdersResultGetAsync(jobGuid, cancellationToken: TestContext.CancellationToken);
            Assert.AreEqual(HttpMethod.Get, GetLastRequest(handler).Method);
            Assert.AreEqual($"/PAPIService/REST/protected/v1/1033/100/101/protected-token/jobs/purchaseorders/{jobGuid}/result", GetLastRequestUri(handler).AbsolutePath);
        }
        [TestMethod] public async Task JobsPurchaseOrdersResultGetAsync_WithEmptyGuid_Throws() => await Assert.ThrowsExactlyAsync<ArgumentOutOfRangeException>(async () => await CreateClient().JobsPurchaseOrdersResultGetAsync(Guid.Empty, cancellationToken: TestContext.CancellationToken));
        private static PapiClient CreateConfiguredClient(CapturingHttpMessageHandler handler) { var client = CreateClient(handler); client.OrganizationId = 101; return client; }
        private static ProtectedToken CreateToken() => new() { AccessToken = "protected-token", AccessSecret = "protected-secret", ExpirationDate = ValidProtectedTokenExpirationDate };
    }
}
