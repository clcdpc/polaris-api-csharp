using Clc.Polaris.Api.Configuration;
using Clc.Polaris.Api.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Clc.Polaris.Api.Tests
{
    [TestClass]
    [TestCategory("Unit")]
    public class PapiClientCustomExecutionTests
    {
        [TestMethod]
        public async Task ExecutePapiAsync_CustomGet_UsesPapiPipelineAndAddsAuthHeaders()
        {
            var handler = new CapturingHttpMessageHandler("{\"PAPIErrorCode\":0}");
            var client = CreateClient(handler);
            var request = PapiRestRequest.Get("/public/v1/1033/100/1/custom/endpoint");

            var response = await client.ExecutePapiAsync<PapiResponseCommon>(request);

            Assert.IsNotNull(response);
            Assert.IsNotNull(handler.LastRequest);
            Assert.AreEqual(HttpMethod.Get, handler.LastRequest!.Method);
            Assert.AreEqual("https://example.test/PAPIService/REST/public/v1/1033/100/1/custom/endpoint", handler.LastRequest.RequestUri!.AbsoluteUri);
            Assert.IsNull(handler.LastRequest.Content);
            Assert.IsTrue(handler.LastRequest.Headers.Contains("PolarisDate"));
            Assert.IsTrue(handler.LastRequest.Headers.Contains("Authorization"));
            StringAssert.StartsWith(handler.LastRequest.Headers.GetValues("Authorization").Single(), "PWS access-id:");
        }

        [TestMethod]
        public async Task ExecutePapiAsync_CustomPost_SerializesBody()
        {
            var handler = new CapturingHttpMessageHandler("{\"PAPIErrorCode\":0}");
            var client = CreateClient(handler);
            var request = PapiRestRequest.Post("/public/v1/1033/100/1/custom/post", new { Value = "example" });

            var response = await client.ExecutePapiAsync<PapiResponseCommon>(request);

            Assert.IsNotNull(response);
            Assert.IsNotNull(handler.LastRequest);
            Assert.AreEqual(HttpMethod.Post, handler.LastRequest!.Method);
            Assert.IsNotNull(handler.LastRequest.Content);
            Assert.IsNotNull(handler.LastRequestContent);
            StringAssert.Contains(handler.LastRequestContent!, "Value");
            StringAssert.Contains(handler.LastRequestContent!, "example");
            Assert.IsTrue(handler.LastRequest.Headers.Contains("PolarisDate"));
            Assert.IsTrue(handler.LastRequest.Headers.Contains("Authorization"));
        }

        [TestMethod]
        public void ExecutePapiAsync_DoesNotAppearOnIPapiClient()
        {
            var interfaceMethodNames = typeof(IPapiClient).GetMethods().Select(method => method.Name).ToArray();

            CollectionAssert.DoesNotContain(interfaceMethodNames, "ExecutePapiAsync");
        }

        [TestMethod]
        public async Task ExecutePapiAsync_NullRequest_ThrowsArgumentNullException()
        {
            var handler = new CapturingHttpMessageHandler("{\"PAPIErrorCode\":0}");
            var client = CreateClient(handler);

            await Assert.ThrowsExceptionAsync<ArgumentNullException>(
                () => client.ExecutePapiAsync<PapiResponseCommon>(null!));

            Assert.IsNull(handler.LastRequest);
        }

        [DataTestMethod]
        [DataRow(null)]
        [DataRow("")]
        [DataRow("   ")]
        public async Task ExecutePapiAsync_NullEmptyOrWhitespacePath_ThrowsArgumentException(string path)
        {
            var handler = new CapturingHttpMessageHandler("{\"PAPIErrorCode\":0}");
            var client = CreateClient(handler);
            var request = new PapiRestRequest { Path = path! };

            await Assert.ThrowsExceptionAsync<ArgumentException>(
                () => client.ExecutePapiAsync<PapiResponseCommon>(request));

            Assert.IsNull(handler.LastRequest);
        }

        [TestMethod]
        public async Task ExecutePapiAsync_AbsoluteUrlPath_ThrowsArgumentException()
        {
            var handler = new CapturingHttpMessageHandler("{\"PAPIErrorCode\":0}");
            var client = CreateClient(handler);
            var request = PapiRestRequest.Get("https://evil.example/public/v1/1033/100/1/foo");

            await Assert.ThrowsExceptionAsync<ArgumentException>(
                () => client.ExecutePapiAsync<PapiResponseCommon>(request));

            Assert.IsNull(handler.LastRequest);
        }

        [TestMethod]
        public async Task ExecutePapiAsync_ProtocolRelativeUrlPath_ThrowsArgumentException()
        {
            var handler = new CapturingHttpMessageHandler("{\"PAPIErrorCode\":0}");
            var client = CreateClient(handler);
            var request = PapiRestRequest.Get("//example.com/foo");

            await Assert.ThrowsExceptionAsync<ArgumentException>(
                () => client.ExecutePapiAsync<PapiResponseCommon>(request));

            Assert.IsNull(handler.LastRequest);
        }

        [TestMethod]
        public async Task ExecutePapiAsync_PathNotBeginningWithSlash_ThrowsArgumentException()
        {
            var handler = new CapturingHttpMessageHandler("{\"PAPIErrorCode\":0}");
            var client = CreateClient(handler);
            var request = PapiRestRequest.Get("public/v1/1033/100/1/foo");

            await Assert.ThrowsExceptionAsync<ArgumentException>(
                () => client.ExecutePapiAsync<PapiResponseCommon>(request));

            Assert.IsNull(handler.LastRequest);
        }

        [TestMethod]
        public async Task ExecutePapiAsync_AuthenticatedPathOutsidePapiScopes_ThrowsArgumentException()
        {
            var handler = new CapturingHttpMessageHandler("{\"PAPIErrorCode\":0}");
            var client = CreateClient(handler);
            var request = PapiRestRequest.Get("/other/v1/1033/100/1/foo");

            await Assert.ThrowsExceptionAsync<ArgumentException>(
                () => client.ExecutePapiAsync<PapiResponseCommon>(request));

            Assert.IsNull(handler.LastRequest);
        }

        [TestMethod]
        public async Task ExecutePapiAsync_PublicCustomRequest_AddsStaffOverrideToken_WhenAllowedAndUnblocked()
        {
            var handler = new CapturingHttpMessageHandler("{\"PAPIErrorCode\":0}");
            var client = CreateClient(handler);
            client.AllowStaffOverrideRequests = true;
            client.Token = new ProtectedToken
            {
                AccessToken = "staff-token",
                AccessSecret = "staff-secret",
                ExpirationDate = DateTime.Now.AddHours(1)
            };
            var request = PapiRestRequest.Get("/public/v1/1033/100/1/custom/staff-override");

            var response = await client.ExecutePapiAsync<PapiResponseCommon>(request);

            Assert.IsNotNull(response);
            Assert.IsNotNull(handler.LastRequest);
            Assert.IsTrue(handler.LastRequest!.Headers.Contains("X-PAPI-AccessToken"));
            Assert.AreEqual("staff-token", handler.LastRequest.Headers.GetValues("X-PAPI-AccessToken").Single());
        }

        [TestMethod]
        public async Task ExecutePapiAsync_PublicCustomRequest_BlockStaffOverride_PreventsStaffOverrideToken()
        {
            var handler = new CapturingHttpMessageHandler("{\"PAPIErrorCode\":0}");
            var client = CreateClient(handler);
            client.AllowStaffOverrideRequests = true;
            client.Token = new ProtectedToken
            {
                AccessToken = "staff-token",
                AccessSecret = "staff-secret",
                ExpirationDate = DateTime.Now.AddHours(1)
            };
            var request = PapiRestRequest.Get("/public/v1/1033/100/1/custom/staff-override");
            request.BlockStaffOverride = true;

            var response = await client.ExecutePapiAsync<PapiResponseCommon>(request);

            Assert.IsNotNull(response);
            Assert.IsNotNull(handler.LastRequest);
            Assert.IsFalse(handler.LastRequest!.Headers.Contains("X-PAPI-AccessToken"));
        }

        private static PapiClient CreateClient(HttpMessageHandler handler)
        {
            return new PapiClient(new HttpClient(handler), new TestPapiSettings())
            {
                AllowStaffOverrideRequests = false,
                UseProtectedTokenCache = false
            };
        }

        private sealed class TestPapiSettings : IPapiSettings
        {
            public string AccessId { get; set; } = "access-id";
            public string AccessKey { get; set; } = "access-key";
            public string Hostname { get; set; } = "https://example.test";
            public int UserId { get; set; } = 1;
            public int WorkstationId { get; set; } = 1;
            public int OrganizationId { get; set; } = 1;
            public PolarisUser? PolarisOverrideAccount { get; set; }
        }

        private sealed class CapturingHttpMessageHandler : HttpMessageHandler
        {
            private readonly string _responseJson;

            public HttpRequestMessage? LastRequest { get; private set; }
            public string? LastRequestContent { get; private set; }

            public CapturingHttpMessageHandler(string responseJson)
            {
                _responseJson = responseJson;
            }

            protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            {
                LastRequest = request;
                LastRequestContent = request.Content == null
                    ? null
                    : await request.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);

                return new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(_responseJson, Encoding.UTF8, "application/json")
                };
            }
        }
    }
}
