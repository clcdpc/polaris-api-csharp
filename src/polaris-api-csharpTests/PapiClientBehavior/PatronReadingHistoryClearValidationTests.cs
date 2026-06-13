namespace Clc.Polaris.Api.Tests.PapiClientBehavior
{
    [TestClass]
    [UnitTest]
    public sealed class PatronReadingHistoryClearValidationTests : PapiClientTestBase
    {
        private const int TestOrganizationId = 73;
        private const string TestBarcode = "PAT123456";

        [TestMethod]
        public async Task PatronReadingHistoryClearAsync_NullIds_UsesConfiguredOrganizationAndDoesNotSendIdsQuery()
        {
            var handler = new CapturingHttpMessageHandler(CreatePapiResponseJson());
            var client = CreateConfiguredClient(handler);

            await client.PatronReadingHistoryClearAsync(TestBarcode, cancellationToken: TestContext.CancellationToken);

            AssertLastRequestPathContains(handler, $"/public/v1/1033/100/73/patron/{TestBarcode}/readinghistory");
            Assert.IsFalse(ParseQuery(GetLastRequestUri(handler).Query).ContainsKey("ids"));
        }

        [TestMethod]
        public async Task PatronReadingHistoryClearAsync_EmptyIds_DoesNotSendIdsQuery()
        {
            var handler = new CapturingHttpMessageHandler(CreatePapiResponseJson());
            var client = CreateConfiguredClient(handler);

            await client.PatronReadingHistoryClearAsync(TestBarcode, ids: Array.Empty<int>(), cancellationToken: TestContext.CancellationToken);

            Assert.IsFalse(ParseQuery(GetLastRequestUri(handler).Query).ContainsKey("ids"));
        }

        [TestMethod]
        public async Task PatronReadingHistoryClearAsync_ValidIds_SendsIdsQuery()
        {
            var handler = new CapturingHttpMessageHandler(CreatePapiResponseJson());
            var client = CreateConfiguredClient(handler);

            await client.PatronReadingHistoryClearAsync(TestBarcode, ids: new[] { 1, 2, 3 }, cancellationToken: TestContext.CancellationToken);

            AssertLastRequestQueryParameter(handler, "ids", "1,2,3");
        }

        [TestMethod]
        [DataRow(0)]
        [DataRow(-1)]
        public async Task PatronReadingHistoryClearAsync_InvalidId_ThrowsArgumentOutOfRangeException(int id)
        {
            var client = CreateClient();

            await Assert.ThrowsExactlyAsync<ArgumentOutOfRangeException>(async () =>
                await client.PatronReadingHistoryClearAsync(TestBarcode, ids: new[] { 1, id }, cancellationToken: TestContext.CancellationToken));
        }

        private static PapiClient CreateConfiguredClient(CapturingHttpMessageHandler handler)
        {
            var client = CreateClient(handler);
            client.OrganizationId = TestOrganizationId;

            return client;
        }
    }
}
