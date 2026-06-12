using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Clc.Polaris.Api.Models;
using Clc.Polaris.Api.Tests;
using Clc.Polaris.Api.Tests.TestInfrastructure;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Clc.Polaris.Api.Tests.PapiClientBehavior
{
    [TestClass]
    [DoNotParallelize]
    [UnitTest]
    public class AuthenticateStaffUserTests : PapiClientTestBase
    {
        [TestInitialize]
        public void TestInitialize()
        {
            ClearProtectedTokenState();
        }

        [TestMethod]
        public async Task AuthenticateStaffUserAsync_ReturnsTokenButDoesNotSetClientToken()
        {
            var handler = new CapturingHttpMessageHandler(CreateProtectedTokenJson(accessToken: "returned-token", accessSecret: "returned-secret", expirationDate: ValidProtectedTokenExpirationDate));
            var client = CreateClient(handler);
            client.Token = new ProtectedToken
            {
                AccessToken = "existing-token",
                AccessSecret = "existing-secret",
                ExpirationDate = ValidProtectedTokenExpirationDate
            };

            var response = await client.AuthenticateStaffUserAsync(CreateStaffUser(), TestContext.CancellationToken);

            Assert.IsNotNull(response.Data);
            Assert.AreEqual("returned-token", response.Data.AccessToken);
            Assert.IsNotNull(client.Token);
            Assert.AreEqual("existing-token", client.Token.AccessToken);
            Assert.AreEqual(1, handler.RequestCount);
            Assert.IsNotNull(handler.LastRequest);
            Assert.AreEqual(HttpMethod.Post, handler.LastRequest.Method);
            Assert.Contains("/protected/v1/1033/100/1/authenticator/staff", handler.LastRequest.RequestUri!.AbsolutePath);
            Assert.IsTrue(handler.LastRequest.Headers.Contains("PolarisDate"));
            Assert.IsTrue(handler.LastRequest.Headers.Contains("Authorization"));
            Assert.IsFalse(handler.LastRequest.Headers.Contains("X-PAPI-AccessToken"));
        }

        [TestMethod]
        public async Task AuthenticateStaffUserAsync_WithStaffOverrideAccountAndNoToken_DoesNotRecursivelyAcquireProtectedToken()
        {
            var handler = new ProtectedTokenHttpMessageHandler();
            var client = CreateProtectedClient(handler);

            var response = await client.AuthenticateStaffUserAsync(CreateStaffUser(), TestContext.CancellationToken);

            Assert.IsNotNull(response.Data);
            Assert.AreEqual("protected-token", response.Data.AccessToken);
            Assert.IsNull(client.Token);
            Assert.AreEqual(1, handler.AuthenticationRequestCount);
            Assert.AreEqual(0, handler.NonAuthenticationRequestCount);
            var request = handler.CapturedRequests.Single();
            Assert.IsTrue(request.IsStaffAuthenticationRequest);
            Assert.AreEqual("POST", request.Method);
            Assert.EndsWith("/protected/v1/1033/100/1/authenticator/staff", request.Path);
            AssertAuthorizationHash(request, string.Empty, client.AccessKey, client.AccessID);
        }
    }
}
