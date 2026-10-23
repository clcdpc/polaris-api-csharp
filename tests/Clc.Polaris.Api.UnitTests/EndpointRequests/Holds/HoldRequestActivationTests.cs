using System.Net;
using System.Text.Json;

namespace Clc.Polaris.Api.UnitTests.EndpointRequests.Holds
{
    [TestClass]
    [UnitTest]
    public sealed class HoldRequestActivationTests : PapiClientUnitTestBase
    {
        [TestMethod]
        public async Task HoldRequestSuspendAsync_SendsFlatActivationBodyWithExplicitUserId()
        {
            var handler = new CapturingHttpMessageHandler(CreatePapiResponseJson());
            var client = CreateClient(handler);
            var activationDate = DateTime.Now.AddDays(7);

            var response = await client.HoldRequestSuspendAsync("AB C/+#?=", 1234, activationDate, password: "1234", userId: 9876, cancellationToken: TestContext.CancellationToken);

            Assert.IsNotNull(response);
            Assert.AreEqual(HttpMethod.Put, GetLastRequest(handler).Method);

            var encodedBarcode = WebUtility.UrlEncode("AB C/+#?=");
            AssertLastRequestPathContains(handler, $"/public/v1/1033/100/1/patron/{encodedBarcode}/holdrequests/1234/inactive");
            AssertFlatActivationBody(handler, expectedUserId: 9876, expectedActivationDate: activationDate);
        }

        [TestMethod]
        public async Task HoldRequestReactivateAsync_SendsFlatActivationBodyWithExplicitUserId()
        {
            var handler = new CapturingHttpMessageHandler(CreatePapiResponseJson());
            var client = CreateClient(handler);
            var activationDate = DateTime.Now.AddDays(7);

            var response = await client.HoldRequestReactivateAsync("AB C/+#?=", 1234, activationDate, "1234", userId: 9876, cancellationToken: TestContext.CancellationToken);

            Assert.IsNotNull(response);
            Assert.AreEqual(HttpMethod.Put, GetLastRequest(handler).Method);

            var encodedBarcode = WebUtility.UrlEncode("AB C/+#?=");
            AssertLastRequestPathContains(handler, $"/public/v1/1033/100/1/patron/{encodedBarcode}/holdrequests/1234/active");
            AssertFlatActivationBody(handler, expectedUserId: 9876, expectedActivationDate: activationDate);
        }

        [TestMethod]
        public async Task HoldRequestSuspendAsync_SendsClientUserIdWhenUserIdIsNotProvided()
        {
            var handler = new CapturingHttpMessageHandler(CreatePapiResponseJson());
            var client = CreateClient(handler);
            client.UserId = 2468;
            var activationDate = DateTime.Now.AddDays(7);

            var response = await client.HoldRequestSuspendAsync("AB C/+#?=", 1234, activationDate, password: "1234", cancellationToken: TestContext.CancellationToken);

            Assert.IsNotNull(response);
            AssertFlatActivationBody(handler, expectedUserId: 2468, expectedActivationDate: activationDate);
        }

        [TestMethod]
        public async Task HoldRequestReactivateAsync_SendsClientUserIdWhenUserIdIsNotProvided()
        {
            var handler = new CapturingHttpMessageHandler(CreatePapiResponseJson());
            var client = CreateClient(handler);
            client.UserId = 2468;
            var activationDate = DateTime.Now.AddDays(7);

            var response = await client.HoldRequestReactivateAsync("AB C/+#?=", 1234, activationDate, "1234", cancellationToken: TestContext.CancellationToken);

            Assert.IsNotNull(response);
            AssertFlatActivationBody(handler, expectedUserId: 2468, expectedActivationDate: activationDate);
        }

        [TestMethod]
        public async Task HoldRequestReactivateAsync_AllRequests_UsesZeroRequestId()
        {
            var handler = new CapturingHttpMessageHandler(CreatePapiResponseJson());
            var client = CreateClient(handler);
            var activationDate = DateTime.Today.AddDays(30);

            await client.HoldRequestReactivateAsync("21234000002105", 0, activationDate, "1234", cancellationToken: TestContext.CancellationToken);

            Assert.AreEqual(HttpMethod.Put, handler.LastRequest!.Method);
            Assert.Contains("/patron/21234000002105/holdrequests/0/active", handler.LastRequest.RequestUri!.AbsolutePath);
            Assert.IsNotNull(handler.LastRequest.Content);
            Assert.IsNotNull(handler.LastRequestContent);
            Assert.Contains(activationDate.ToString("yyyy-MM-dd"), handler.LastRequestContent);
        }

        [TestMethod]
        public async Task HoldRequestSuspendAsync_AllRequests_UsesZeroRequestId()
        {
            var handler = new CapturingHttpMessageHandler(CreatePapiResponseJson());
            var client = CreateClient(handler);
            var activationDate = DateTime.Today.AddDays(30);

            await client.HoldRequestSuspendAsync("21234000002105", 0, activationDate, "1234", cancellationToken: TestContext.CancellationToken);

            Assert.AreEqual(HttpMethod.Put, handler.LastRequest!.Method);
            Assert.Contains("/patron/21234000002105/holdrequests/0/inactive", handler.LastRequest.RequestUri!.AbsolutePath);
            Assert.IsNotNull(handler.LastRequest.Content);
            Assert.IsNotNull(handler.LastRequestContent);
            Assert.Contains(activationDate.ToString("yyyy-MM-dd"), handler.LastRequestContent);
        }

        private static void AssertFlatActivationBody(CapturingHttpMessageHandler handler, int expectedUserId, DateTime expectedActivationDate)
        {
            using var document = JsonDocument.Parse(GetLastRequestBody(handler));
            var body = document.RootElement;

            Assert.AreEqual(JsonValueKind.Object, body.ValueKind);
            Assert.IsFalse(body.TryGetProperty("HoldRequestActivationData", out _), "Expected activation request body to be flat. It must not be wrapped in a HoldRequestActivationData object.");

            Assert.IsTrue(body.TryGetProperty("UserID", out var userId), "Expected flat UserID property.");
            Assert.AreEqual(expectedUserId, userId.GetInt32());

            Assert.IsTrue(body.TryGetProperty("ActivationDate", out var activationDate), "Expected flat ActivationDate property.");
            Assert.AreEqual(expectedActivationDate, activationDate.GetDateTime());
        }
    }
}
