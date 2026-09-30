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

        [TestMethod]
        public async Task HoldRequestCreateAsync_MapsDocumentedMisspelledQualifierResponseField()
        {
            var requestGuid = Guid.Parse("7f279d6e-cfde-4ae3-be96-ee12305cdf11");
            var handler = new CapturingHttpMessageHandler(CreateJson(new
            {
                PAPIErrorCode = 0,
                RequestGUID = requestGuid,
                TxnGroupQualifer = "group",
                TxnQualifier = "qualifier",
                StatusType = 3,
                StatusValue = 5,
                QueuePosition = 2,
                QueueTotal = 3
            }));
            var client = CreateClient(handler);

            var response = await client.HoldRequestCreateAsync(patronId: 123, bibId: 456,
                pickupBranchId: 7, cancellationToken: TestContext.CancellationToken);

            Assert.IsNotNull(response.Data);
            Assert.AreEqual(requestGuid, response.Data.RequestGuid);
            Assert.AreEqual("group", response.Data.TxnGroupQualifer);
            Assert.AreEqual("qualifier", response.Data.TxnQualifier);
            Assert.AreEqual(3, response.Data.StatusType);
            Assert.AreEqual(5, response.Data.StatusValue);
            Assert.AreEqual(2, response.Data.QueuePosition);
            Assert.AreEqual(2, response.Data.QueuePostition);
            Assert.AreEqual(3, response.Data.QueueTotal);
        }

        private static void AssertJsonPropertyValue(JsonElement body, string propertyName, int expectedValue)
        {
            Assert.IsTrue(body.TryGetProperty(propertyName, out var property), $"Expected JSON body property '{propertyName}'.");
            Assert.AreEqual(expectedValue, property.GetInt32());
        }
    }
}
