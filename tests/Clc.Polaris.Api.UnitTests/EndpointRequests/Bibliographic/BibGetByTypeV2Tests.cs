namespace Clc.Polaris.Api.UnitTests.EndpointRequests.Bibliographic
{
    [TestClass]
    [UnitTest]
    public sealed class BibGetByTypeV2Tests : PapiClientUnitTestBase
    {
        [TestMethod]
        public async Task BibGetByTypeV2Async_SendsExpectedRequest()
        {
            var handler = new CapturingHttpMessageHandler(CreatePapiResponseJson());
            var client = CreateConfiguredClient(handler);

            await client.BibGetByTypeV2Async("abc 123", branchId: 88, cancellationToken: TestContext.CancellationToken);

            Assert.AreEqual(HttpMethod.Get, GetLastRequest(handler).Method);
            Assert.AreEqual("/PAPIService/REST/public/v2/1033/100/88/bib/abc+123", GetLastRequestUri(handler).AbsolutePath);
            AssertLastRequestQueryParameter(handler, "type", "barcode");
        }

        [TestMethod]
        public async Task BibGetByTypeV2Async_WithEmptyKey_Throws()
        {
            var client = CreateClient();

            await Assert.ThrowsAsync<ArgumentException>(async () =>
                await client.BibGetByTypeV2Async("", cancellationToken: TestContext.CancellationToken));
        }

        private static PapiClient CreateConfiguredClient(CapturingHttpMessageHandler handler)
        {
            var client = CreateClient(handler);
            client.OrganizationId = 101;
            return client;
        }
    }
}
