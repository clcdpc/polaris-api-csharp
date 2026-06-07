using Clc.Polaris.Api.Configuration;
using Clc.Polaris.Api.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
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
            request.QueryParameters.Add("q", "custom search");

            var response = await client.ExecutePapiAsync<PapiResponseCommon>(request);

            Assert.IsNotNull(response);
            Assert.IsNotNull(handler.LastRequest);
            Assert.AreEqual(HttpMethod.Get, handler.LastRequest!.Method);
            Assert.AreEqual("https://example.test/PAPIService/REST/public/v1/1033/100/1/custom/endpoint?q=custom%20search", handler.LastRequest.RequestUri!.AbsoluteUri);
            Assert.IsTrue(handler.LastRequest.Headers.Contains("PolarisDate"));
            Assert.IsTrue(handler.LastRequest.Headers.Contains("Authorization"));
            AssertAuthorizationHashesSentUri(handler.LastRequest, string.Empty);
        }

        [TestMethod]
        public async Task ExecutePapiAsync_CustomPost_SerializesBody()
        {
            var handler = new CapturingHttpMessageHandler("{\"PAPIErrorCode\":0}");
            var client = CreateClient(handler);
            var body = new { Name = "Custom", Count = 2 };
            var request = PapiRestRequest.Post("/public/v1/1033/100/1/custom/post", body: body);

            var response = await client.ExecutePapiAsync<PapiResponseCommon>(request);

            Assert.IsNotNull(response);
            Assert.IsNotNull(handler.LastRequest);
            Assert.AreEqual(HttpMethod.Post, handler.LastRequest!.Method);
            Assert.IsNotNull(handler.LastRequest.Content);
            Assert.IsNotNull(handler.LastRequestContent);
            StringAssert.Contains(handler.LastRequestContent!, "Custom");
            StringAssert.Contains(handler.LastRequestContent!, "Count");
            AssertAuthorizationHashesSentUri(handler.LastRequest, string.Empty);
        }

        [TestMethod]
        public void ExecutePapiAsync_IsPublicOnPapiClientOnly()
        {
            var clientMethod = typeof(PapiClient).GetMethods()
                .SingleOrDefault(method => method.Name == "ExecutePapiAsync" && method.IsPublic && method.GetParameters().Length == 2);

            Assert.IsNotNull(clientMethod);
            Assert.IsNull(typeof(IPapiClient).GetMethod("ExecutePapiAsync"));
        }

        [TestMethod]
        public async Task ExecutePapiAsync_NullRequest_ThrowsArgumentNullException()
        {
            var handler = new CapturingHttpMessageHandler("{\"PAPIErrorCode\":0}");
            var client = CreateClient(handler);

            await Assert.ThrowsExceptionAsync<ArgumentNullException>(async () =>
                await client.ExecutePapiAsync<PapiResponseCommon>(null!));

            Assert.IsNull(handler.LastRequest);
        }

        [TestMethod]
        [DataRow(null)]
        [DataRow("")]
        [DataRow("   ")]
        public async Task ExecutePapiAsync_NullEmptyOrWhitespacePath_ThrowsArgumentException(string path)
        {
            var handler = new CapturingHttpMessageHandler("{\"PAPIErrorCode\":0}");
            var client = CreateClient(handler);
            var request = PapiRestRequest.Get(path!);

            await Assert.ThrowsExceptionAsync<ArgumentException>(async () =>
                await client.ExecutePapiAsync<PapiResponseCommon>(request));

            Assert.IsNull(handler.LastRequest);
        }

        [TestMethod]
        public async Task ExecutePapiAsync_AbsoluteUrlPath_ThrowsArgumentException()
        {
            var handler = new CapturingHttpMessageHandler("{\"PAPIErrorCode\":0}");
            var client = CreateClient(handler);
            var request = PapiRestRequest.Get("https://malicious.example/public/v1/1033/100/1/custom");

            await Assert.ThrowsExceptionAsync<ArgumentException>(async () =>
                await client.ExecutePapiAsync<PapiResponseCommon>(request));

            Assert.IsNull(handler.LastRequest);
        }

        [TestMethod]
        public async Task ExecutePapiAsync_ProtocolRelativePath_ThrowsArgumentException()
        {
            var handler = new CapturingHttpMessageHandler("{\"PAPIErrorCode\":0}");
            var client = CreateClient(handler);
            var request = PapiRestRequest.Get("//example.com/foo");

            await Assert.ThrowsExceptionAsync<ArgumentException>(async () =>
                await client.ExecutePapiAsync<PapiResponseCommon>(request));

            Assert.IsNull(handler.LastRequest);
        }

        [TestMethod]
        public async Task ExecutePapiAsync_AuthenticatedPathOutsidePapiScope_ThrowsArgumentException()
        {
            var handler = new CapturingHttpMessageHandler("{\"PAPIErrorCode\":0}");
            var client = CreateClient(handler);
            var request = PapiRestRequest.Get("/admin/v1/1033/100/1/custom");

            await Assert.ThrowsExceptionAsync<ArgumentException>(async () =>
                await client.ExecutePapiAsync<PapiResponseCommon>(request));

            Assert.IsNull(handler.LastRequest);
        }

        [TestMethod]
        public async Task ExecutePapiAsync_PublicCustomRequest_UsesStaffOverrideWhenAllowed()
        {
            var handler = new CapturingHttpMessageHandler("{\"PAPIErrorCode\":0}");
            var client = CreateClient(handler);
            client.AllowStaffOverrideRequests = true;
            client.Token = new ProtectedToken
            {
                AccessToken = "override-token",
                AccessSecret = "override-secret",
                ExpirationDate = DateTime.Now.AddHours(1)
            };
            var request = PapiRestRequest.Get("/public/v1/1033/100/1/custom/staff-override");

            var response = await client.ExecutePapiAsync<PapiResponseCommon>(request);

            Assert.IsNotNull(response);
            Assert.IsNotNull(handler.LastRequest);
            Assert.AreEqual("override-token", handler.LastRequest!.Headers.GetValues("X-PAPI-AccessToken").Single());
            AssertAuthorizationHashesSentUri(handler.LastRequest, "override-secret");
        }

        [TestMethod]
        public async Task ExecutePapiAsync_BlockStaffOverride_PreventsAccessTokenHeader()
        {
            var handler = new CapturingHttpMessageHandler("{\"PAPIErrorCode\":0}");
            var client = CreateClient(handler);
            client.AllowStaffOverrideRequests = true;
            client.Token = new ProtectedToken
            {
                AccessToken = "override-token",
                AccessSecret = "override-secret",
                ExpirationDate = DateTime.Now.AddHours(1)
            };
            var request = PapiRestRequest.Get("/public/v1/1033/100/1/custom/no-override");
            request.BlockStaffOverride = true;

            var response = await client.ExecutePapiAsync<PapiResponseCommon>(request);

            Assert.IsNotNull(response);
            Assert.IsNotNull(handler.LastRequest);
            Assert.IsFalse(handler.LastRequest!.Headers.Contains("X-PAPI-AccessToken"));
            AssertAuthorizationHashesSentUri(handler.LastRequest, string.Empty);
        }

        private static PapiClient CreateClient(HttpMessageHandler handler)
        {
            var httpClient = new HttpClient(handler);
            return new PapiClient(httpClient, new TestPapiSettings())
            {
                AllowStaffOverrideRequests = false,
                UseProtectedTokenCache = false
            };
        }

        private static void AssertAuthorizationHashesSentUri(HttpRequestMessage request, string password)
        {
            var date = request.Headers.GetValues("PolarisDate").Single();
            var expectedHash = ComputePapiHash(request.Method.Method, request.RequestUri!.AbsoluteUri, date, password, "access-key");
            Assert.AreEqual($"PWS access-id:{expectedHash}", request.Headers.GetValues("Authorization").Single());
        }

        private static string ComputePapiHash(string httpMethod, string uri, string date, string password, string accessKey)
        {
            var hashString = httpMethod + uri + date + password;
            var computedHash = HMACSHA1.HashData(Encoding.UTF8.GetBytes(accessKey), Encoding.UTF8.GetBytes(hashString));
            return Convert.ToBase64String(computedHash);
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
