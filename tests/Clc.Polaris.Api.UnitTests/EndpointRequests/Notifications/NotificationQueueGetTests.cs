namespace Clc.Polaris.Api.UnitTests.EndpointRequests.Notifications
{
    [TestClass]
    [UnitTest]
    public class NotificationQueueGetTests : PapiClientUnitTestBase
    {
        [TestMethod]
        public async Task NotificationQueueGet_RequestsCorrectUrl()
        {
            var handler = new CapturingHttpMessageHandler();
            var client = CreateUrlEncodingClient(handler);
            client.Token = new ProtectedToken
            {
                AccessToken = "token-segment",
                AccessSecret = "token-secret",
                ExpirationDate = ValidProtectedTokenExpirationDate
            };

            await client.NotificationQueueGetAsync(1, TestContext.CancellationToken);

            var expectedPath = $"/PAPIService/REST/protected/v1/1033/24/1/token-segment/notification/";
            Assert.IsNotNull(handler.LastRequest);
            Assert.AreEqual(expectedPath, handler.LastRequest!.RequestUri!.AbsolutePath);
        }
    }
}
