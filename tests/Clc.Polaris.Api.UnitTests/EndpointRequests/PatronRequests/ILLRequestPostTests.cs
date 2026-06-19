namespace Clc.Polaris.Api.UnitTests.EndpointRequests.PatronRequests
{
    [TestClass]
    [UnitTest]
    public sealed class ILLRequestPostTests : PapiClientUnitTestBase
    {
        [TestMethod]
        public async Task ILLRequestPostAsync_SendsPublicPostWithBody()
        {
            var handler = new CapturingHttpMessageHandler(CreatePapiResponseJson());
            var client = CreateClient(handler);
            client.OrganizationId = 101;
            var data = new ILLRequestCreateData { PatronID = 12, Title = "Title", PickupOrgID = 0 };

            await client.ILLRequestPostAsync(data, cancellationToken: TestContext.CancellationToken);

            Assert.AreEqual(HttpMethod.Post, GetLastRequest(handler).Method);
            Assert.AreEqual("/PAPIService/REST/public/v1/1033/100/101/illrequest", GetLastRequestUri(handler).AbsolutePath);
            Assert.AreEqual("application/json", GetLastRequest(handler).Content!.Headers.ContentType!.MediaType);
            AssertLastRequestBodyJsonPropertyValue(handler, "PatronID", 12);
            AssertLastRequestBodyJsonPropertyValue(handler, "Title", "Title");
            AssertLastRequestBodyJsonPropertyValue(handler, "PickupOrgID", 0);
            var body = GetLastRequestBody(handler);
            Assert.DoesNotContain("<ILLRequestCreateData", body);
            Assert.DoesNotContain("<PatronID>", body);
            Assert.DoesNotContain("<?xml", body);
        }

        [TestMethod]
        public async Task ILLRequestPostAsync_WithNullRequest_Throws() =>
            await Assert.ThrowsExactlyAsync<ArgumentNullException>(async () => await CreateClient().ILLRequestPostAsync(null!, cancellationToken: TestContext.CancellationToken));

        [TestMethod]
        public async Task ILLRequestPostAsync_WithInvalidPatronId_Throws() =>
            await Assert.ThrowsExactlyAsync<ArgumentOutOfRangeException>(async () => await CreateClient().ILLRequestPostAsync(new ILLRequestCreateData { PatronID = 0, Title = "t", PickupOrgID = 1 }, cancellationToken: TestContext.CancellationToken));

        [TestMethod]
        public async Task ILLRequestPostAsync_WithEmptyTitle_Throws() =>
            await Assert.ThrowsAsync<ArgumentException>(async () => await CreateClient().ILLRequestPostAsync(new ILLRequestCreateData { PatronID = 1, Title = " ", PickupOrgID = 1 }, cancellationToken: TestContext.CancellationToken));

        [TestMethod]
        public async Task ILLRequestPostAsync_WithInvalidPickupOrgId_Throws() =>
            await Assert.ThrowsExactlyAsync<ArgumentOutOfRangeException>(async () => await CreateClient().ILLRequestPostAsync(new ILLRequestCreateData { PatronID = 1, Title = "t", PickupOrgID = -1 }, cancellationToken: TestContext.CancellationToken));
    }
}
