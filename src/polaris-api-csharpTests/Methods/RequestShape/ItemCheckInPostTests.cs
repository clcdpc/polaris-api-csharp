namespace Clc.Polaris.Api.Tests.Methods.RequestShape
{
    [TestClass]
    [UnitTest]
    public class ItemCheckInPostTests : PapiClientTestBase
    {
        [TestMethod]
        public async Task ItemCheckInPostAsync_DefaultLogonIds_SendsProtectedPostRequest()
        {
            var handler = new CapturingHttpMessageHandler(CreatePapiResponseJson());
            var client = CreateConfiguredClient(handler);

            await client.ItemCheckInPostAsync("0000100004126", cancellationToken: TestContext.CancellationToken);

            Assert.IsNotNull(handler.LastRequest);
            Assert.AreEqual(HttpMethod.Post, handler.LastRequest!.Method);
            AssertLastRequestPathContains(handler, "/PAPIService/REST/protected/v1/1033/100/73/");
            Assert.IsTrue(handler.LastRequest.RequestUri!.AbsolutePath.EndsWith("/item/0000100004126/checkin", StringComparison.Ordinal));
            AssertLastRequestBodyJsonPropertyValue(handler, "LogonBranchID", 73);
            AssertLastRequestBodyJsonPropertyValue(handler, "LogonUserID", 11);
            AssertLastRequestBodyJsonPropertyValue(handler, "LogonWorkstationID", 22);
        }

        [TestMethod]
        public async Task ItemCheckInPostAsync_ExplicitLogonIds_SendsOverridesInBody()
        {
            var handler = new CapturingHttpMessageHandler(CreatePapiResponseJson());
            var client = CreateConfiguredClient(handler);

            await client.ItemCheckInPostAsync(
                "0000100004126",
                logonBranchId: 99,
                logonUserId: 1,
                logonWorkstationId: 1243,
                cancellationToken: TestContext.CancellationToken);

            AssertLastRequestBodyJsonPropertyValue(handler, "LogonBranchID", 99);
            AssertLastRequestBodyJsonPropertyValue(handler, "LogonUserID", 1);
            AssertLastRequestBodyJsonPropertyValue(handler, "LogonWorkstationID", 1243);
        }

        [TestMethod]
        [DataRow(null)]
        [DataRow("")]
        [DataRow("   ")]
        public async Task ItemCheckInPostAsync_InvalidItemBarcode_ThrowsArgumentException(string? itemBarcode)
        {
            var client = CreateClient();

            await Assert.ThrowsAsync<ArgumentException>(async () =>
                await client.ItemCheckInPostAsync(itemBarcode!, cancellationToken: TestContext.CancellationToken));
        }

        [TestMethod]
        [DataRow("logonBranchId", 0)]
        [DataRow("logonBranchId", -1)]
        [DataRow("logonUserId", 0)]
        [DataRow("logonUserId", -1)]
        [DataRow("logonWorkstationId", 0)]
        [DataRow("logonWorkstationId", -1)]
        public async Task ItemCheckInPostAsync_InvalidLogonId_ThrowsArgumentOutOfRangeException(string parameterName, int value)
        {
            var client = CreateClient();

            var exception = await Assert.ThrowsExactlyAsync<ArgumentOutOfRangeException>(async () =>
                await client.ItemCheckInPostAsync(
                    "0000100004126",
                    logonBranchId: parameterName == "logonBranchId" ? value : null,
                    logonUserId: parameterName == "logonUserId" ? value : null,
                    logonWorkstationId: parameterName == "logonWorkstationId" ? value : null,
                    cancellationToken: TestContext.CancellationToken));
            Assert.AreEqual(parameterName, exception.ParamName);
        }

        private static PapiClient CreateConfiguredClient(CapturingHttpMessageHandler handler)
        {
            var client = CreateClient(handler);
            client.OrganizationId = 73;
            client.UserId = 11;
            client.WorkstationId = 22;
            client.Token = new ProtectedToken
            {
                AccessToken = "protected-token",
                AccessSecret = "protected-secret",
                ExpirationDate = ValidProtectedTokenExpirationDate
            };

            return client;
        }
    }
}
