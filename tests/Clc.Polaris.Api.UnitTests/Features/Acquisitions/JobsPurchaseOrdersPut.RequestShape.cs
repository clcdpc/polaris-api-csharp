namespace Clc.Polaris.Api.UnitTests.Features.Acquisitions
{
    [TestClass]
    [UnitTest]
    public sealed class JobsPurchaseOrdersPutRequestShape : PapiClientUnitTestBase
    {
        [TestMethod]
        public async Task JobsPurchaseOrdersPutAsync_SendsProtectedPutWithJsonBody()
        {
            var handler = new CapturingHttpMessageHandler(CreatePapiResponseJson()); var client = CreateConfiguredClient(handler); client.Token = CreateToken();
            await client.JobsPurchaseOrdersPutAsync(new JobsPurchaseOrdersPreorderValidationData { Vendor = "vendor" }, cancellationToken: TestContext.CancellationToken);
            Assert.AreEqual(HttpMethod.Put, GetLastRequest(handler).Method);
            Assert.AreEqual("/PAPIService/REST/protected/v1/1033/100/101/protected-token/jobs/purchaseorders", GetLastRequestUri(handler).AbsolutePath);
            AssertLastRequestQueryParameter(handler, "preordervalidation", "1"); AssertLastRequestBodyContains(handler, "vendor");
        }
        [TestMethod] public async Task JobsPurchaseOrdersPutAsync_WithNullRequest_Throws() => await Assert.ThrowsExactlyAsync<ArgumentNullException>(async () => await CreateClient().JobsPurchaseOrdersPutAsync(null!, cancellationToken: TestContext.CancellationToken));
        [TestMethod] public async Task JobsPurchaseOrdersPutAsync_WithInvalidPreorderValidation_Throws() => await Assert.ThrowsExactlyAsync<ArgumentOutOfRangeException>(async () => await CreateClient().JobsPurchaseOrdersPutAsync(new JobsPurchaseOrdersPreorderValidationData(), 2, cancellationToken: TestContext.CancellationToken));
        private static PapiClient CreateConfiguredClient(CapturingHttpMessageHandler handler) { var client = CreateClient(handler); client.OrganizationId = 101; return client; }
        private static ProtectedToken CreateToken() => new() { AccessToken = "protected-token", AccessSecret = "protected-secret", ExpirationDate = ValidProtectedTokenExpirationDate };
    }
}
