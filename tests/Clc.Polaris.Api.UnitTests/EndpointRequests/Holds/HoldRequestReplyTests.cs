using System.Text.Json;

namespace Clc.Polaris.Api.UnitTests.EndpointRequests.Holds
{
    [TestClass]
    [UnitTest]
    public sealed class HoldRequestReplyTests : PapiClientUnitTestBase
    {
        [TestMethod]
        public async Task HoldRequestReplyAsync_MapsDocumentedResultAndSendsConversationContext()
        {
            var requestGuid = Guid.Parse("7f279d6e-cfde-4ae3-be96-ee12305cdf11");
            var handler = new CapturingHttpMessageHandler(CreateJson(new
            {
                PAPIErrorCode = 0,
                RequestGUID = requestGuid,
                TxnGroupQualifer = "next-group",
                TxnQualifier = "next-qualifier",
                StatusType = 2,
                StatusValue = 1,
                Message = "Your request has been placed.",
                QueuePosition = 2,
                QueueTotal = 3
            }));
            var client = CreateClient(handler);
            client.OrganizationId = 73;
            var conversation = new HoldRequestCreateResult
            {
                RequestGuid = requestGuid,
                TxnGroupQualifier = "prior-group",
                TxnQualifier = "prior-qualifier"
            };

            var response = await client.HoldRequestReplyAsync(conversation, 73, HoldRequestReplyAnswer.Yes,
                HoldRequestReplyState.AcceptEvenWithExistingHolds, TestContext.CancellationToken);

            Assert.AreEqual(HttpMethod.Put, GetLastRequest(handler).Method);
            AssertLastRequestPathContains(handler, $"/public/v1/1033/100/73/holdrequest/{requestGuid}");
            using var document = JsonDocument.Parse(GetLastRequestBody(handler));
            var body = document.RootElement;
            Assert.AreEqual("prior-group", body.GetProperty("TxnGroupQualifier").GetString());
            Assert.AreEqual("prior-qualifier", body.GetProperty("TxnQualifier").GetString());
            Assert.AreEqual(73, body.GetProperty("RequestingOrgID").GetInt32());
            Assert.AreEqual(1, body.GetProperty("Answer").GetInt32());
            Assert.AreEqual(3, body.GetProperty("State").GetInt32());
            Assert.IsNotNull(response.Data);
            Assert.AreEqual(requestGuid, response.Data.RequestGuid);
            Assert.AreEqual("next-group", response.Data.TxnGroupQualifier);
            Assert.AreEqual("next-qualifier", response.Data.TxnQualifier);
            Assert.AreEqual(2, response.Data.StatusType);
            Assert.AreEqual(1, response.Data.StatusValue);
            Assert.AreEqual("Your request has been placed.", response.Data.Message);
            Assert.AreEqual(2, response.Data.QueuePosition);
            Assert.AreEqual(3, response.Data.QueueTotal);
        }

        [TestMethod]
        public async Task HoldRequestReplyAsync_MapsFailureWithoutInventingRequestIdentity()
        {
            var handler = new CapturingHttpMessageHandler(CreateJson(new
            {
                PAPIErrorCode = -4006,
                RequestGUID = "",
                TxnGroupQualifer = "",
                TxnQualifier = "",
                StatusType = 0,
                StatusValue = 0,
                QueuePosition = 0,
                QueueTotal = 0
            }));
            var client = CreateClient(handler);
            var conversation = new HoldRequestCreateResult
            {
                RequestGuid = Guid.Parse("7f279d6e-cfde-4ae3-be96-ee12305cdf11"),
                TxnGroupQualifier = "group",
                TxnQualifier = "qualifier"
            };

            var response = await client.HoldRequestReplyAsync(conversation, 73, HoldRequestReplyAnswer.Yes,
                HoldRequestReplyState.AcceptEvenWithExistingHolds, TestContext.CancellationToken);

            Assert.IsNotNull(response.Data);
            Assert.AreEqual(-4006, response.Data.PAPIErrorCode);
            Assert.IsNull(response.Data.RequestGuid);
            Assert.AreEqual(0, response.Data.StatusType);
            Assert.AreEqual(0, response.Data.StatusValue);
        }
    }
}
