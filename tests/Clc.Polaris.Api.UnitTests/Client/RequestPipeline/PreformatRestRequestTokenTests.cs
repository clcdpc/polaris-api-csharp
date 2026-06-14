namespace Clc.Polaris.Api.UnitTests.PapiClientBehavior
{
    [TestClass]
    [DoNotParallelize]
    [UnitTest]
    public class PreformatRestRequestTokenTests : PapiClientUnitTestBase
    {
        [TestInitialize]
        public void TestInitialize()
        {
            ClearProtectedTokenState();
        }

        [TestMethod]
        public void PreformatRestRequest_ExpiredProtectedToken_DoesNotUseSecretForProtectedMethodSigning()
        {
            var handler = new CapturingHttpMessageHandler();
            var client = CreateClient(handler);
            client.Token = new ProtectedToken
            {
                AccessToken = "expired-token",
                AccessSecret = "expired-secret",
                ExpirationDate = ExpiredProtectedTokenExpirationDate
            };
            var request = new PapiRestRequest(HttpMethod.Get, "/protected/v1/1033/100/1/expired-token/search/patrons/Boolean");

            var formatted = (PapiRestRequest)client.PreformatRestRequest(request);

            var date = formatted.Headers["PolarisDate"].ToString();
            var uri = client.BuildRequestUri(formatted).AbsoluteUri;
            Assert.AreEqual($"PWS access-id:{PapiSignature.ComputeHash("access-key", "GET", uri, date, string.Empty)}", formatted.Headers["Authorization"]);
            Assert.AreNotEqual($"PWS access-id:{PapiSignature.ComputeHash("access-key", "GET", uri, date, "expired-secret")}", formatted.Headers["Authorization"]);
            Assert.IsFalse(formatted.Headers.ContainsKey("X-PAPI-AccessToken"));
            Assert.IsNull(client.Token);
        }

        [TestMethod]
        public void PreformatRestRequest_ExpiredProtectedToken_DoesNotUseSecretOrHeaderForPublicStaffOverrideSigning()
        {
            var handler = new CapturingHttpMessageHandler();
            var client = CreateClient(handler);
            client.AllowStaffOverrideRequests = true;
            client.Token = new ProtectedToken
            {
                AccessToken = "expired-token",
                AccessSecret = "expired-secret",
                ExpirationDate = ExpiredProtectedTokenExpirationDate
            };
            var request = new PapiRestRequest(HttpMethod.Get, "/public/v1/1033/100/1/patron/ABC");
            request.Headers["X-PAPI-AccessToken"] = "stale-token";

            var formatted = (PapiRestRequest)client.PreformatRestRequest(request);

            var date = formatted.Headers["PolarisDate"].ToString();
            var uri = client.BuildRequestUri(formatted).AbsoluteUri;
            var authorization = formatted.Headers["Authorization"];
            Assert.AreEqual($"PWS access-id:{PapiSignature.ComputeHash("access-key", "GET", uri, date, string.Empty)}", authorization);
            Assert.AreNotEqual($"PWS access-id:{PapiSignature.ComputeHash("access-key", "GET", uri, date, "expired-secret")}", authorization);
            Assert.IsFalse(formatted.Headers.ContainsKey("X-PAPI-AccessToken"));
            Assert.IsNull(client.Token);
        }

        [TestMethod]
        public void PreformatRestRequest_PublicStaffOverrideWithMissingTokenValues_DoesNotAddAccessTokenHeader()
        {
            var handler = new CapturingHttpMessageHandler();
            var client = CreateClient(handler);
            client.AllowStaffOverrideRequests = true;
            client.StaffOverrideAccount = CreateStaffUser();
            client.Token = new ProtectedToken
            {
                AccessToken = " ",
                AccessSecret = "manual-secret",
                ExpirationDate = ValidProtectedTokenExpirationDate
            };

            var formatted = (PapiRestRequest)client.PreformatRestRequest(new PapiRestRequest(HttpMethod.Get, "/public/v1/1033/100/1/patron/ABC"));

            Assert.IsFalse(formatted.Headers.ContainsKey("X-PAPI-AccessToken"));
        }
    }
}
