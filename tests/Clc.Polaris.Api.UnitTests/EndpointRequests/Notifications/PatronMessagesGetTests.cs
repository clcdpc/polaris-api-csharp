using System.Net;
using System.Text;

namespace Clc.Polaris.Api.UnitTests.EndpointRequests.Notifications
{
    [TestClass]
    [UnitTest]
    public class PatronMessagesGetTests : PapiClientUnitTestBase
    {
        [TestMethod]
        public async Task PatronMessages_RequestShape_PreservesBooleanLikeQueryValue()
        {
            var handler = new CapturingHttpMessageHandler(CreatePapiResponseJson());
            var client = CreateClient(handler);

            var response = await client.PatronMessagesGetAsync("ABC 123", unreadOnly: true, password: "1234", TestContext.CancellationToken);

            Assert.IsNotNull(response);
            Assert.AreEqual(HttpMethod.Get, handler.LastRequest!.Method);
            Assert.Contains("/public/v1/1033/100/1/patron/ABC+123/messages", handler.LastRequest.RequestUri!.AbsolutePath);
            var query = ParseQuery(handler.LastRequest.RequestUri.Query);
            Assert.AreEqual("1", query["unreadonly"]);
            Assert.IsNull(handler.LastRequest.Content);
            Assert.IsTrue(handler.LastRequest.Headers.Contains("PolarisDate"));
            Assert.IsTrue(handler.LastRequest.Headers.Contains("Authorization"));
        }

        [TestMethod]
        public async Task PublicPatronMethod_WithPassword_DoesNotAuthenticateOrSendStaffOverrideHeader()
        {
            var handler = new FailingStaffAuthenticationHttpMessageHandler();
            var client = CreateClient(handler);
            client.AllowStaffOverrideRequests = true;
            client.StaffOverrideAccount = new PolarisUser
            {
                Domain = "main",
                Username = $"password-patron-{Guid.NewGuid():N}",
                Password = "secret"
            };

            var response = await client.PatronMessagesGetAsync("ABC123", password: "patron-password", cancellationToken: TestContext.CancellationToken);

            Assert.IsNotNull(response);
            Assert.AreEqual(0, handler.AuthenticationRequestCount);
            Assert.AreEqual(1, handler.PublicRequestCount);
            Assert.IsNotNull(handler.LastNonAuthenticationRequest);
            Assert.IsFalse(handler.LastNonAuthenticationRequest!.Headers.Contains("X-PAPI-AccessToken"));
            AssertAuthorizationHashesSentUri(handler.LastNonAuthenticationRequest, "patron-password");
        }

        private sealed class FailingStaffAuthenticationHttpMessageHandler : HttpMessageHandler
        {
            public int AuthenticationRequestCount { get; private set; }
            public int ProtectedRequestCount { get; private set; }
            public int PublicRequestCount { get; private set; }
            public HttpRequestMessage? LastNonAuthenticationRequest { get; private set; }

            protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            {
                if (request.RequestUri!.AbsolutePath.Contains("/authenticator/staff", StringComparison.Ordinal))
                {
                    AuthenticationRequestCount++;
                    return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
                    {
                        Content = new StringContent(CreateEmptyJsonObject(), Encoding.UTF8, "application/json")
                    });
                }

                LastNonAuthenticationRequest = request;
                if (request.RequestUri!.AbsolutePath.Contains("/protected/", StringComparison.Ordinal))
                {
                    ProtectedRequestCount++;
                }
                else
                {
                    PublicRequestCount++;
                }

                return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(CreatePapiResponseJson(), Encoding.UTF8, "application/json")
                });
            }
        }
    }
}
