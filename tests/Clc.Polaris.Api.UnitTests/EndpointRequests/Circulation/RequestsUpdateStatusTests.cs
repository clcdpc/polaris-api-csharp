namespace Clc.Polaris.Api.UnitTests.EndpointRequests.Circulation
{
    [TestClass]
    [UnitTest]
    public sealed class RequestsUpdateStatusTests : PapiClientUnitTestBase
    {
        [TestMethod]
        public async Task RequestsUpdateStatusAsync_SendsExpectedProtectedPutRequest()
        {
            var handler = new CapturingHttpMessageHandler(CreatePapiResponseJson());
            var client = CreateConfiguredClient(handler);
            client.Token = CreateToken();

            await client.RequestsUpdateStatusAsync(123, RequestStatusAction.AskMeLater, itemId: 456, cancellationToken: TestContext.CancellationToken);

            Assert.AreEqual(HttpMethod.Put, GetLastRequest(handler).Method);
            Assert.AreEqual("/PAPIService/REST/protected/v1/1033/100/101/protected-token/circulation/requests/123/status", GetLastRequestUri(handler).AbsolutePath);
            AssertLastRequestQueryParameter(handler, "action", "askmelater");
            AssertLastRequestQueryParameter(handler, "itemid", "456");
        }

        [TestMethod]
        public async Task RequestsUpdateStatusAsync_WithDenyAndNoReason_Throws()
        {
            var client = CreateClient();

            await Assert.ThrowsAsync<ArgumentException>(async () =>
                await client.RequestsUpdateStatusAsync(1, RequestStatusAction.Deny, cancellationToken: TestContext.CancellationToken));
        }

        [TestMethod]
        public async Task RequestsUpdateStatusAsync_WithReturnAndNoItemId_Throws()
        {
            var client = CreateClient();

            await Assert.ThrowsAsync<ArgumentException>(async () =>
                await client.RequestsUpdateStatusAsync(1, RequestStatusAction.Return, cancellationToken: TestContext.CancellationToken));
        }

        private static PapiClient CreateConfiguredClient(CapturingHttpMessageHandler handler)
        {
            var client = CreateClient(handler);
            client.OrganizationId = 101;
            return client;
        }

        private static ProtectedToken CreateToken()
        {
            return new ProtectedToken
            {
                AccessToken = "protected-token",
                AccessSecret = "protected-secret",
                ExpirationDate = ValidProtectedTokenExpirationDate
            };
        }
    }
}
