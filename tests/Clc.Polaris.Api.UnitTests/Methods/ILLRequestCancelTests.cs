namespace Clc.Polaris.Api.UnitTests.Methods
{
    [TestClass]
    [UnitTest]
    public sealed class ILLRequestCancelTests : PapiClientUnitTestBase
    {
        [TestMethod]
        public async Task ILLRequestCancelAsync_SendsExpectedPutRequest()
        {
            var handler = new CapturingHttpMessageHandler(CreatePapiResponseJson());
            var client = CreateConfiguredClient(handler);

            await client.ILLRequestCancelAsync("abc 123", 0, cancellationToken: TestContext.CancellationToken);

            Assert.AreEqual(HttpMethod.Put, GetLastRequest(handler).Method);
            Assert.AreEqual("/PAPIService/REST/public/v1/1033/100/101/patron/abc+123/illrequests/0/cancelled", GetLastRequestUri(handler).AbsolutePath);
            AssertLastRequestQueryParameter(handler, "wsid", "22");
            AssertLastRequestQueryParameter(handler, "userid", "11");
        }

        [TestMethod]
        public async Task ILLRequestCancelAsync_WithNegativeIllRequestId_Throws()
        {
            var client = CreateClient();

            await Assert.ThrowsExactlyAsync<ArgumentOutOfRangeException>(async () =>
                await client.ILLRequestCancelAsync("b", -1, cancellationToken: TestContext.CancellationToken));
        }

        private static PapiClient CreateConfiguredClient(CapturingHttpMessageHandler handler)
        {
            var client = CreateClient(handler);
            client.OrganizationId = 101;
            client.UserId = 11;
            client.WorkstationId = 22;
            return client;
        }
    }
}
