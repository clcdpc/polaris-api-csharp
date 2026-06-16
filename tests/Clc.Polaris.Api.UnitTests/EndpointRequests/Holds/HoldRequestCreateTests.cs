using System.Text.Json;

namespace Clc.Polaris.Api.UnitTests.EndpointRequests.Holds
{
    [TestClass]
    [UnitTest]
    public sealed class HoldRequestCreateTests : PapiClientUnitTestBase
    {
        [TestMethod]
        public async Task HoldRequestCreateAsync_ConvenienceOverloadSendsExpectedCreateBodyAndDefaults()
        {
            var handler = new CapturingHttpMessageHandler(CreatePapiResponseJson());
            var client = CreateClient(handler);
            client.OrganizationId = 73;
            client.UserId = 9876;
            client.WorkstationId = 5432;

            var response = await client.HoldRequestCreateAsync(patronId: 123, bibId: 456, pickupBranchId: 7, cancellationToken: TestContext.CancellationToken);

            Assert.IsNotNull(response);
            Assert.AreEqual(HttpMethod.Post, GetLastRequest(handler).Method);
            AssertLastRequestPathContains(handler, "/public/v1/1033/100/73/holdrequest");

            using var document = JsonDocument.Parse(GetLastRequestBody(handler));
            var body = document.RootElement;

            Assert.AreEqual(JsonValueKind.Object, body.ValueKind);
            AssertJsonPropertyValue(body, "PatronID", 123);
            AssertJsonPropertyValue(body, "BibID", 456);
            AssertJsonPropertyValue(body, "PickupOrgID", 7);
            AssertJsonPropertyValue(body, "RequestingOrgID", 73);
            AssertJsonPropertyValue(body, "UserID", 9876);
            AssertJsonPropertyValue(body, "WorkstationID", 5432);
        }

        private static void AssertJsonPropertyValue(JsonElement body, string propertyName, int expectedValue)
        {
            Assert.IsTrue(body.TryGetProperty(propertyName, out var property), $"Expected JSON body property '{propertyName}'.");
            Assert.AreEqual(expectedValue, property.GetInt32());
        }
    }
}