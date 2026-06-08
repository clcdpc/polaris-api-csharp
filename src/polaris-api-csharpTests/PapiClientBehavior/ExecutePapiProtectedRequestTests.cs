using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Clc.Polaris.Api.Models;
using Clc.Polaris.Api.Tests;
using Clc.Polaris.Api.Tests.TestInfrastructure;
using Clc.Rest.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Clc.Polaris.Api.Tests.PapiClientBehavior
{
    [TestClass]
    [DoNotParallelize]
    [UnitCategory]
    public class ExecutePapiProtectedRequestTests : PapiClientTestBase
    {
        [TestInitialize]
        public void TestInitialize()
        {
            ClearProtectedTokenState();
        }

        [TestMethod]
        public async Task ExecutePapiAsync_ProtectedRequestWithoutPlaceholderOrPassword_AcquiresProtectedToken()
        {
            var handler = new ProtectedTokenHttpMessageHandler(
                HttpStatusCode.OK,
                CreateProtectedTokenJson("route-token", "route-secret", DateTime.Now.AddHours(1)));
            var client = CreateProtectedClient(handler);
            var request = PapiRestRequest.Get("/protected/v1/1033/100/1/patron/ABC123/account/outstanding");

            await ExecuteRawPapiRequestAsync(client, request);

            Assert.AreEqual(1, handler.AuthenticationRequestCount);
            Assert.AreEqual(1, handler.NonAuthenticationRequestCount);
            Assert.AreEqual("route-token", client.Token?.AccessToken);
            var finalRequest = handler.CapturedRequests.Single(capturedRequest => !capturedRequest.IsStaffAuthenticationRequest);
            StringAssert.EndsWith(finalRequest.Path, "/protected/v1/1033/100/1/patron/ABC123/account/outstanding");
            AssertAuthorizationHash(finalRequest, "route-secret", client.AccessKey, client.AccessID);
        }

        [TestMethod]
        public async Task ExecutePapiAsync_ProtectedPathEndingDifferentlyThanStaffAuthenticator_AcquiresProtectedToken()
        {
            var handler = new ProtectedTokenHttpMessageHandler(
                HttpStatusCode.OK,
                CreateProtectedTokenJson("malformed-token", "malformed-secret", DateTime.Now.AddHours(1)));
            var client = CreateProtectedClient(handler);
            var request = PapiRestRequest.Post("/protected/v1/1033/100/1/authenticator/staff/extra");

            await ExecuteRawPapiRequestAsync(client, request);

            Assert.AreEqual(1, handler.AuthenticationRequestCount);
            Assert.AreEqual(1, handler.NonAuthenticationRequestCount);
            Assert.AreEqual("malformed-token", client.Token?.AccessToken);
            var finalRequest = handler.CapturedRequests.Single(capturedRequest => !capturedRequest.IsStaffAuthenticationRequest);
            StringAssert.EndsWith(finalRequest.Path, "/protected/v1/1033/100/1/authenticator/staff/extra");
            AssertAuthorizationHash(finalRequest, "malformed-secret", client.AccessKey, client.AccessID);
        }

        [TestMethod]
        public async Task ExecutePapiAsync_NullRequest_ThrowsBeforeSendingHttpRequest()
        {
            var handler = new CapturingHttpMessageHandler();
            var client = CreateClient(handler);

            await Assert.ThrowsExceptionAsync<ArgumentNullException>(() => client.ExecutePapiAsync<PapiResponseCommon>(null!));

            Assert.AreEqual(0, handler.RequestCount);
        }

        [TestMethod]
        [DataRow(null)]
        [DataRow("")]
        [DataRow("   ")]
        public async Task ExecutePapiAsync_NullEmptyOrWhitespacePath_ThrowsBeforeSendingHttpRequest(string? path)
        {
            var handler = new CapturingHttpMessageHandler();
            var client = CreateClient(handler);
            var request = PapiRestRequest.Get("/public/v1/1033/100/1/custom");
            request.Path = path!;

            await Assert.ThrowsExceptionAsync<ArgumentException>(() => client.ExecutePapiAsync<PapiResponseCommon>(request));

            Assert.AreEqual(0, handler.RequestCount);
        }

        [TestMethod]
        [DataRow("https://example.com/PAPIService/REST/public/v1/1033/100/1/custom")]
        [DataRow("//example.com/foo")]
        [DataRow("public/v1/1033/100/1/custom")]
        public async Task ExecutePapiAsync_InvalidCustomPath_ThrowsBeforeSendingHttpRequest(string path)
        {
            var handler = new CapturingHttpMessageHandler();
            var client = CreateClient(handler);
            var request = PapiRestRequest.Get(path);

            await Assert.ThrowsExceptionAsync<ArgumentException>(() => client.ExecutePapiAsync<PapiResponseCommon>(request));

            Assert.AreEqual(0, handler.RequestCount);
        }

        [TestMethod]
        public async Task ExecutePapiAsync_AuthenticatedPathOutsidePublicOrProtected_ThrowsBeforeSendingHttpRequest()
        {
            var handler = new CapturingHttpMessageHandler();
            var client = CreateClient(handler);
            var request = PapiRestRequest.Get("/other/v1/1033/100/1/custom");

            await Assert.ThrowsExceptionAsync<ArgumentException>(() => client.ExecutePapiAsync<PapiResponseCommon>(request));

            Assert.AreEqual(0, handler.RequestCount);
        }

        [TestMethod]
        public async Task ExecutePapiAsync_CustomPublicGet_SendsThroughPapiPipelineWithDateAndAuthorizationHeaders()
        {
            var handler = new ProtectedTokenHttpMessageHandler();
            var client = CreateProtectedClient(handler);
            client.StaffOverrideAccount = null;
            var request = PapiRestRequest.Get("/public/v1/1033/100/1/custom/unsupported");

            await client.ExecutePapiAsync<PapiResponseCommon>(request);

            Assert.AreEqual(0, handler.AuthenticationRequestCount);
            Assert.AreEqual(1, handler.NonAuthenticationRequestCount);
            var finalRequest = handler.CapturedRequests.Single();
            Assert.AreEqual("GET", finalRequest.Method);
            StringAssert.Contains(finalRequest.Path, "/public/v1/1033/100/1/custom/unsupported");
            AssertAuthorizationHash(finalRequest, string.Empty, client.AccessKey, client.AccessID);
        }

        [TestMethod]
        public async Task ExecutePapiAsync_CustomPublicPost_SerializesBodyAndSignsRequest()
        {
            var handler = new ProtectedTokenHttpMessageHandler();
            var client = CreateProtectedClient(handler);
            client.StaffOverrideAccount = null;
            var request = PapiRestRequest.Post(
                "/public/v1/1033/100/1/custom/unsupported",
                body: new { Message = "custom-body", Count = 3 });

            await client.ExecutePapiAsync<PapiResponseCommon>(request);

            Assert.AreEqual(0, handler.AuthenticationRequestCount);
            Assert.AreEqual(1, handler.NonAuthenticationRequestCount);
            var finalRequest = handler.CapturedRequests.Single();
            Assert.AreEqual("POST", finalRequest.Method);
            StringAssert.Contains(finalRequest.Body, "custom-body");
            StringAssert.Contains(finalRequest.Body, "3");
            AssertAuthorizationHash(finalRequest, string.Empty, client.AccessKey, client.AccessID);
        }

        [TestMethod]
        public async Task ExecutePapiAsync_CustomProtectedRequestWithPlaceholder_AcquiresAndReplacesProtectedToken()
        {
            var handler = new ProtectedTokenHttpMessageHandler(
                HttpStatusCode.OK,
                CreateProtectedTokenJson("custom-placeholder-token", "custom-placeholder-secret", DateTime.Now.AddHours(1)));
            var client = CreateProtectedClient(handler);
            var request = PapiRestRequest.Get($"/protected/v1/1033/100/1/{ProtectedToken.Placeholder}/custom/unsupported");

            await client.ExecutePapiAsync<PapiResponseCommon>(request);

            Assert.AreEqual(1, handler.AuthenticationRequestCount);
            Assert.AreEqual(1, handler.NonAuthenticationRequestCount);
            var finalRequest = handler.CapturedRequests.Single(capturedRequest => !capturedRequest.IsStaffAuthenticationRequest);
            StringAssert.Contains(finalRequest.Path, "/protected/v1/1033/100/1/custom-placeholder-token/custom/unsupported");
            Assert.IsFalse(finalRequest.Path.Contains(ProtectedToken.Placeholder, StringComparison.Ordinal));
            AssertAuthorizationHash(finalRequest, "custom-placeholder-secret", client.AccessKey, client.AccessID);
        }

        [TestMethod]
        public async Task ExecutePapiAsync_CustomPublicRequest_UsesStaffOverrideTokenWhenAllowed()
        {
            var handler = new ProtectedTokenHttpMessageHandler(
                HttpStatusCode.OK,
                CreateProtectedTokenJson("override-token", "override-secret", DateTime.Now.AddHours(1)));
            var client = CreateProtectedClient(handler);
            var request = PapiRestRequest.Get("/public/v1/1033/100/1/custom/unsupported");

            await client.ExecutePapiAsync<PapiResponseCommon>(request);

            Assert.AreEqual(1, handler.AuthenticationRequestCount);
            Assert.AreEqual(1, handler.NonAuthenticationRequestCount);
            var finalRequest = handler.CapturedRequests.Single(capturedRequest => !capturedRequest.IsStaffAuthenticationRequest);
            Assert.IsTrue(finalRequest.Headers.TryGetValue("X-PAPI-AccessToken", out var accessToken));
            Assert.AreEqual("override-token", accessToken);
            AssertAuthorizationHash(finalRequest, "override-secret", client.AccessKey, client.AccessID);
        }

        [TestMethod]
        public async Task ExecutePapiAsync_CustomPublicRequestWithBlockStaffOverride_DoesNotSendStaffOverrideToken()
        {
            var handler = new ProtectedTokenHttpMessageHandler();
            var client = CreateProtectedClient(handler);
            var request = PapiRestRequest.Get("/public/v1/1033/100/1/custom/unsupported");
            request.BlockStaffOverride = true;

            await client.ExecutePapiAsync<PapiResponseCommon>(request);

            Assert.AreEqual(0, handler.AuthenticationRequestCount);
            Assert.AreEqual(1, handler.NonAuthenticationRequestCount);
            var finalRequest = handler.CapturedRequests.Single();
            Assert.IsFalse(finalRequest.Headers.ContainsKey("X-PAPI-AccessToken"));
            AssertAuthorizationHash(finalRequest, string.Empty, client.AccessKey, client.AccessID);
        }
    }
}
