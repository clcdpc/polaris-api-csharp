namespace Clc.Polaris.Api.UnitTests.EndpointRequests.Bibliographic
{
    [TestClass]
    [UnitTest]
    public sealed class MultipartGetTests : PapiClientUnitTestBase
    {
        [TestMethod]
        public async Task MultipartGetAsync_SendsExpectedRequestAndOptionalPickupLocation()
        {
            var handler = new CapturingHttpMessageHandler(CreatePapiResponseJson());
            var client = CreateConfiguredClient(handler);

            await client.MultipartGetAsync(123, 456, pickupLocationId: 789, cancellationToken: TestContext.CancellationToken);

            Assert.AreEqual(HttpMethod.Get, GetLastRequest(handler).Method);
            Assert.AreEqual("/PAPIService/REST/public/v1/1033/100/101/bib/123/multiparts", GetLastRequestUri(handler).AbsolutePath);
            AssertLastRequestQueryParameter(handler, "PatronID", "456");
            AssertLastRequestQueryParameter(handler, "PickupLocID", "789");
        }

        [TestMethod]
        public async Task MultipartGetAsync_WithInvalidBibId_Throws()
        {
            var client = CreateClient();

            await Assert.ThrowsExactlyAsync<ArgumentOutOfRangeException>(async () =>
                await client.MultipartGetAsync(0, 1, cancellationToken: TestContext.CancellationToken));
        }

        private static PapiClient CreateConfiguredClient(CapturingHttpMessageHandler handler)
        {
            var client = CreateClient(handler);
            client.OrganizationId = 101;
            return client;
        }
    }
}
