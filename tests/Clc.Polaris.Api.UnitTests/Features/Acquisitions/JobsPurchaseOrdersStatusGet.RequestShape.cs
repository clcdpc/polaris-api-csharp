namespace Clc.Polaris.Api.UnitTests.Features.Acquisitions
{
    [TestClass]
    [UnitTest]
    public sealed class JobsPurchaseOrdersStatusGetRequestShape : PapiClientUnitTestBase
    {
        [TestMethod]
        public async Task JobsPurchaseOrdersStatusGetAsync_SendsProtectedGetRequest()
        {
            var handler = new CapturingHttpMessageHandler(CreatePapiResponseJson()); var client = CreateConfiguredClient(handler); client.Token = CreateToken(); var jobGuid = Guid.Parse("22222222-2222-2222-2222-222222222222");
            await client.JobsPurchaseOrdersStatusGetAsync(jobGuid, cancellationToken: TestContext.CancellationToken);
            Assert.AreEqual(HttpMethod.Get, GetLastRequest(handler).Method);
            Assert.AreEqual($"/PAPIService/REST/protected/v1/1033/100/101/protected-token/jobs/purchaseorders/{jobGuid}/status", GetLastRequestUri(handler).AbsolutePath);
        }
        [TestMethod] public async Task JobsPurchaseOrdersStatusGetAsync_WithEmptyGuid_Throws() => await Assert.ThrowsExactlyAsync<ArgumentOutOfRangeException>(async () => await CreateClient().JobsPurchaseOrdersStatusGetAsync(Guid.Empty, cancellationToken: TestContext.CancellationToken));
        private static PapiClient CreateConfiguredClient(CapturingHttpMessageHandler handler) { var client = CreateClient(handler); client.OrganizationId = 101; return client; }
        private static ProtectedToken CreateToken() => new() { AccessToken = "protected-token", AccessSecret = "protected-secret", ExpirationDate = ValidProtectedTokenExpirationDate };
    }
}
