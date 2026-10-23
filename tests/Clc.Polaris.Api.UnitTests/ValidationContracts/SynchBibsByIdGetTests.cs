namespace Clc.Polaris.Api.UnitTests.ValidationContracts
{
    [TestClass]
    [UnitTest]
    public sealed class SynchBibsByIdGetTests : PapiClientUnitTestBase
    {
        private const int TestOrganizationId = 73;
        private const string TestProtectedAccessToken = "protected-token";
        private const string TestProtectedAccessSecret = "protected-secret";

        [TestMethod]
        public async Task Synch_BibsByIdGetAsync_ValidBibIds_UsesConfiguredOrganizationAndSendsBibIdsQuery()
        {
            var handler = new CapturingHttpMessageHandler(CreatePapiResponseJson());
            var client = CreateConfiguredProtectedClient(handler);

            await client.Synch_BibsByIdGetAsync(new[] { 111, 222 }, includeItems: true, cancellationToken: TestContext.CancellationToken);

            AssertLastRequestPathContains(handler, $"/protected/v1/1033/100/73/{TestProtectedAccessToken}/synch/bibs/MARCxml");
            AssertLastRequestQueryParameter(handler, "bibids", "111,222");
            AssertLastRequestQueryParameter(handler, "includeItems", "1");
        }

        [TestMethod]
        public async Task Synch_BibsByIdGetAsync_NullBibIds_ThrowsArgumentNullException()
        {
            var client = CreateClient();

            await Assert.ThrowsExactlyAsync<ArgumentNullException>(async () =>
                await client.Synch_BibsByIdGetAsync(null!, cancellationToken: TestContext.CancellationToken));
        }

        [TestMethod]
        public async Task Synch_BibsByIdGetAsync_EmptyBibIds_ThrowsArgumentException()
        {
            var client = CreateClient();

            await Assert.ThrowsExactlyAsync<ArgumentException>(async () =>
                await client.Synch_BibsByIdGetAsync(Array.Empty<int>(), cancellationToken: TestContext.CancellationToken));
        }

        [TestMethod]
        [DataRow(0)]
        [DataRow(-1)]
        public async Task Synch_BibsByIdGetAsync_InvalidBibId_ThrowsArgumentOutOfRangeException(int bibId)
        {
            var client = CreateClient();

            await Assert.ThrowsExactlyAsync<ArgumentOutOfRangeException>(async () =>
                await client.Synch_BibsByIdGetAsync(new[] { 111, bibId }, cancellationToken: TestContext.CancellationToken));
        }

        [TestMethod]
        [DataRow(0)]
        [DataRow(-1)]
        public async Task Synch_BibsByIdGetAsync_ExtensionInvalidBibId_ThrowsArgumentOutOfRangeException(int bibId)
        {
            var client = CreateClient();

            await Assert.ThrowsExactlyAsync<ArgumentOutOfRangeException>(async () =>
                await client.Synch_BibsByIdGetAsync(bibId, cancellationToken: TestContext.CancellationToken));
        }

        private static PapiClient CreateConfiguredProtectedClient(CapturingHttpMessageHandler handler)
        {
            var client = CreateClient(handler);
            client.OrganizationId = TestOrganizationId;
            client.Token = new ProtectedToken
            {
                AccessToken = TestProtectedAccessToken,
                AccessSecret = TestProtectedAccessSecret,
                ExpirationDate = DateTime.UtcNow.AddHours(1)
            };

            return client;
        }
    }
}
