namespace Clc.Polaris.Api.UnitTests.Features.Patrons
{
    [TestClass]
    [UnitTest]
    public sealed class ILLRequestPostRequestShape : PapiClientUnitTestBase
    {
        [TestMethod]
        public async Task ILLRequestPostAsync_SendsPublicPostWithUtf8CompatibleXmlBody()
        {
            var handler = new CapturingHttpMessageHandler(CreatePapiResponseJson());
            var client = CreateClient(handler);
            client.OrganizationId = 101;
            var data = new ILLRequestCreateData { PatronID = 12, Title = "Title", PickupOrgID = 0 };

            await client.ILLRequestPostAsync(data, cancellationToken: TestContext.CancellationToken);

            Assert.AreEqual(HttpMethod.Post, GetLastRequest(handler).Method);
            Assert.AreEqual("/PAPIService/REST/public/v1/1033/100/101/illrequest", GetLastRequestUri(handler).AbsolutePath);
            var body = GetLastRequestBody(handler);
            Assert.DoesNotContain("encoding=\"utf-16\"", body, StringComparison.OrdinalIgnoreCase);
            Assert.IsTrue(body.StartsWith("<ILLRequestCreateData", StringComparison.Ordinal), "Expected XML body to omit the declaration and start with the root element.");
            Assert.Contains("<PatronID>12</PatronID>", body);
            Assert.Contains("<Title>Title</Title>", body);
            Assert.Contains("<PickupOrgID>0</PickupOrgID>", body);
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
