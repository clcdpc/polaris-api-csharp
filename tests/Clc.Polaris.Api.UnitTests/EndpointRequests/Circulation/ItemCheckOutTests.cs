namespace Clc.Polaris.Api.UnitTests.EndpointRequests.Circulation
{
    [TestClass]
    [UnitTest]
    public class ItemCheckOutTests : PapiClientUnitTestBase
    {
        [TestMethod]
        public async Task ItemCheckOutPostAsync_DefaultLogonIds_SendsPublicPostRequest()
        {
            var handler = new CapturingHttpMessageHandler(CreatePapiResponseJson());
            var client = CreateConfiguredClient(handler);

            await client.ItemCheckOutPostAsync(
                patronBarcode: "21756003332022",
                itemBarcode: "0000410443451",
                password: "1234",
                cancellationToken: TestContext.CancellationToken);

            Assert.IsNotNull(handler.LastRequest);
            Assert.AreEqual(HttpMethod.Post, handler.LastRequest!.Method);
            Assert.AreEqual("/PAPIService/REST/public/v1/1033/100/73/patron/21756003332022/itemsout", handler.LastRequest.RequestUri!.AbsolutePath);
            AssertLastRequestBodyJsonPropertyValue(handler, "ItemBarcode", "0000410443451");
            AssertLastRequestBodyJsonPropertyValue(handler, "LogonBranchID", 73);
            AssertLastRequestBodyJsonPropertyValue(handler, "LogonUserID", 11);
            AssertLastRequestBodyJsonPropertyValue(handler, "LogonWorkstationID", 22);
        }

        [TestMethod]
        public async Task ItemCheckOutPostAsync_ExplicitLogonIds_SendsOverridesInBody()
        {
            var handler = new CapturingHttpMessageHandler(CreatePapiResponseJson());
            var client = CreateConfiguredClient(handler);

            await client.ItemCheckOutPostAsync(
                patronBarcode: "21756003332022",
                itemBarcode: "0000410443451",
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
        public async Task ItemCheckOutPostAsync_InvalidPatronBarcode_ThrowsArgumentException(string? patronBarcode)
        {
            var client = CreateClient();

            await Assert.ThrowsAsync<ArgumentException>(async () =>
                await client.ItemCheckOutPostAsync(patronBarcode!, "0000410443451", cancellationToken: TestContext.CancellationToken));
        }

        [TestMethod]
        [DataRow(null)]
        [DataRow("")]
        [DataRow("   ")]
        public async Task ItemCheckOutPostAsync_InvalidItemBarcode_ThrowsArgumentException(string? itemBarcode)
        {
            var client = CreateClient();

            await Assert.ThrowsAsync<ArgumentException>(async () =>
                await client.ItemCheckOutPostAsync("21756003332022", itemBarcode!, cancellationToken: TestContext.CancellationToken));
        }

        [TestMethod]
        [DataRow("logonBranchId", 0)]
        [DataRow("logonBranchId", -1)]
        [DataRow("logonUserId", 0)]
        [DataRow("logonUserId", -1)]
        [DataRow("logonWorkstationId", 0)]
        [DataRow("logonWorkstationId", -1)]
        public async Task ItemCheckOutPostAsync_InvalidLogonId_ThrowsArgumentOutOfRangeException(string parameterName, int value)
        {
            var client = CreateClient();

            var exception = await Assert.ThrowsExactlyAsync<ArgumentOutOfRangeException>(async () =>
                await client.ItemCheckOutPostAsync(
                    "21756003332022",
                    "0000410443451",
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

            return client;
        }
    }
}
