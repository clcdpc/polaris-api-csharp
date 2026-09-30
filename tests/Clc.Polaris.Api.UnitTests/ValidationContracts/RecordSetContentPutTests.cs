namespace Clc.Polaris.Api.UnitTests.ValidationContracts
{
    [TestClass]
    [UnitTest]
    public sealed class RecordSetContentPutTests : PapiClientUnitTestBase
    {
        private const int TestOrganizationId = 73;
        private const int TestUserId = 11;
        private const int TestWorkstationId = 22;
        private const string TestProtectedAccessToken = "protected-token";
        private const string TestProtectedAccessSecret = "protected-secret";

        [TestMethod]
        public async Task RecordSetContentPutAsync_ValidRecords_UsesConfiguredContextAndSendsCsvBody()
        {
            var handler = new CapturingHttpMessageHandler(CreatePapiResponseJson());
            var client = CreateConfiguredProtectedClient(handler);

            await client.RecordSetContentPutAsync(
                recordSetId: 12345,
                records: new[] { 111, 222 },
                action: RecordSetContentPutActions.Add,
                cancellationToken: TestContext.CancellationToken);

            AssertLastRequestPathContains(handler, $"/protected/v1/1033/100/73/{TestProtectedAccessToken}/recordsets/12345");
            AssertLastRequestQueryParameter(handler, "userid", "11");
            AssertLastRequestQueryParameter(handler, "wsid", "22");
            AssertLastRequestBodyJsonPropertyValue(handler, "records", "111,222");
        }

        [TestMethod]
        [DataRow(0)]
        [DataRow(-1)]
        public async Task RecordSetContentPutAsync_InvalidRecordSetId_ThrowsArgumentOutOfRangeException(int recordSetId)
        {
            var client = CreateClient();

            await Assert.ThrowsExactlyAsync<ArgumentOutOfRangeException>(async () =>
                await client.RecordSetContentPutAsync(
                    recordSetId: recordSetId,
                    records: new[] { 111 },
                    action: RecordSetContentPutActions.Add,
                    cancellationToken: TestContext.CancellationToken));
        }

        [TestMethod]
        public async Task RecordSetContentPutAsync_NullRecords_ThrowsArgumentNullException()
        {
            var client = CreateClient();

            await Assert.ThrowsExactlyAsync<ArgumentNullException>(async () =>
                await client.RecordSetContentPutAsync(
                    recordSetId: 12345,
                    records: null!,
                    action: RecordSetContentPutActions.Add,
                    cancellationToken: TestContext.CancellationToken));
        }

        [TestMethod]
        public async Task RecordSetContentPutAsync_EmptyRecords_ThrowsArgumentException()
        {
            var client = CreateClient();

            await Assert.ThrowsExactlyAsync<ArgumentException>(async () =>
                await client.RecordSetContentPutAsync(
                    recordSetId: 12345,
                    records: Array.Empty<int>(),
                    action: RecordSetContentPutActions.Add,
                    cancellationToken: TestContext.CancellationToken));
        }

        [TestMethod]
        [DataRow(0)]
        [DataRow(-1)]
        public async Task RecordSetContentPutAsync_InvalidRecordId_ThrowsArgumentOutOfRangeException(int recordId)
        {
            var client = CreateClient();

            await Assert.ThrowsExactlyAsync<ArgumentOutOfRangeException>(async () =>
                await client.RecordSetContentPutAsync(
                    recordSetId: 12345,
                    records: new[] { 111, recordId },
                    action: RecordSetContentPutActions.Add,
                    cancellationToken: TestContext.CancellationToken));
        }

        [TestMethod]
        [DataRow(nameof(PapiClient.UserId), 0)]
        [DataRow(nameof(PapiClient.UserId), -1)]
        [DataRow(nameof(PapiClient.WorkstationId), 0)]
        [DataRow(nameof(PapiClient.WorkstationId), -1)]
        public async Task RecordSetContentPutAsync_InvalidOptionalContextId_ThrowsArgumentOutOfRangeException(string propertyName, int value)
        {
            var client = CreateClient();

            await Assert.ThrowsExactlyAsync<ArgumentOutOfRangeException>(async () =>
                await client.RecordSetContentPutAsync(
                    recordSetId: 12345,
                    records: new[] { 111 },
                    action: RecordSetContentPutActions.Add,
                    userId: propertyName == nameof(PapiClient.UserId) ? value : null,
                    workstationId: propertyName == nameof(PapiClient.WorkstationId) ? value : null,
                    cancellationToken: TestContext.CancellationToken));
        }

        private static PapiClient CreateConfiguredProtectedClient(CapturingHttpMessageHandler handler)
        {
            var client = CreateClient(handler);
            client.OrganizationId = TestOrganizationId;
            client.UserId = TestUserId;
            client.WorkstationId = TestWorkstationId;
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
