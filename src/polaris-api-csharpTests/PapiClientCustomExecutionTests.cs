using Clc.Polaris.Api.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
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
        public void ExecutePapiAsync_IsPublicOnPapiClient()
        {
            var method = typeof(PapiClient).GetMethod(
                "ExecutePapiAsync",
                BindingFlags.Instance | BindingFlags.Public);

            Assert.IsNotNull(method);
            Assert.IsTrue(method!.IsGenericMethodDefinition);
        }

        [TestMethod]
        public void ExecutePapiAsync_IsNotPresentOnIPapiClient()
        {
            var method = typeof(IPapiClient).GetMethods()
                .SingleOrDefault(candidate => candidate.Name == "ExecutePapiAsync");

            Assert.IsNull(method);
        }

        [TestMethod]
        public async Task ExecutePapiAsync_NullRequest_ThrowsBeforeSendingHttpRequest()
        {
            var handler = new RecordingHttpMessageHandler();
            var client = CreateClient(handler);

            await Assert.ThrowsExceptionAsync<ArgumentNullException>(() =>
                client.ExecutePapiAsync<PapiResponseCommon>(null!));

            Assert.AreEqual(0, handler.RequestCount);
        }

        [DataTestMethod]
        [DataRow(null)]
        [DataRow("")]
        [DataRow("   ")]
        public async Task ExecutePapiAsync_BlankPath_ThrowsBeforeSendingHttpRequest(string? path)
        {
            var handler = new RecordingHttpMessageHandler();
            var client = CreateClient(handler);
            var request = PapiRestRequest.Get("/public/v1/1033/100/1/custom");
            request.Path = path!;

            await Assert.ThrowsExceptionAsync<ArgumentException>(() =>
                client.ExecutePapiAsync<PapiResponseCommon>(request));

            Assert.AreEqual(0, handler.RequestCount);
        }

        [TestMethod]
        public async Task ExecutePapiAsync_AbsoluteUrlPath_ThrowsBeforeSendingHttpRequest()
        {
            var handler = new RecordingHttpMessageHandler();
            var client = CreateClient(handler);
            var request = PapiRestRequest.Get("https://example.com/public/v1/1033/100/1/custom");

            await Assert.ThrowsExceptionAsync<ArgumentException>(() =>
                client.ExecutePapiAsync<PapiResponseCommon>(request));

            Assert.AreEqual(0, handler.RequestCount);
        }

        [TestMethod]
        public async Task ExecutePapiAsync_ProtocolRelativeUrlPath_ThrowsBeforeSendingHttpRequest()
        {
            var handler = new RecordingHttpMessageHandler();
            var client = CreateClient(handler);
            var request = PapiRestRequest.Get("//example.com/foo");

            await Assert.ThrowsExceptionAsync<ArgumentException>(() =>
                client.ExecutePapiAsync<PapiResponseCommon>(request));

            Assert.AreEqual(0, handler.RequestCount);
        }

        [TestMethod]
        public async Task ExecutePapiAsync_PathWithoutLeadingSlash_ThrowsBeforeSendingHttpRequest()
        {
            var handler = new RecordingHttpMessageHandler();
            var client = CreateClient(handler);
            var request = PapiRestRequest.Get("public/v1/1033/100/1/custom");

            await Assert.ThrowsExceptionAsync<ArgumentException>(() =>
                client.ExecutePapiAsync<PapiResponseCommon>(request));

            Assert.AreEqual(0, handler.RequestCount);
        }

        [TestMethod]
        public async Task ExecutePapiAsync_AuthenticatedPathOutsidePublicOrProtected_ThrowsBeforeSendingHttpRequest()
        {
            var handler = new RecordingHttpMessageHandler();
            var client = CreateClient(handler);
            var request = PapiRestRequest.Get("/custom/v1/1033/100/1/endpoint");

            await Assert.ThrowsExceptionAsync<ArgumentException>(() =>
                client.ExecutePapiAsync<PapiResponseCommon>(request));

            Assert.AreEqual(0, handler.RequestCount);
        }

        [TestMethod]
        public async Task ExecutePapiAsync_CustomPublicGet_UsesPapiPipelineAndSignsRequest()
        {
            var handler = new RecordingHttpMessageHandler();
            var client = CreateClient(handler);
            var request = PapiRestRequest.Get("/public/v1/1033/100/1/custom/endpoint");

            await client.ExecutePapiAsync<PapiResponseCommon>(request);

            var captured = handler.SingleCustomRequest();
            Assert.AreEqual(HttpMethod.Get, captured.Method);
            Assert.AreEqual("https://example.test/PAPIService/REST/public/v1/1033/100/1/custom/endpoint", captured.AbsoluteUri);
            Assert.IsTrue(captured.Headers.ContainsKey("PolarisDate"));
            Assert.IsTrue(captured.Headers.ContainsKey("Authorization"));
            AssertAuthorizationHashesSentUri(captured, string.Empty);
        }

        [TestMethod]
        public async Task ExecutePapiAsync_CustomPublicPost_SerializesBodyAndSignsRequest()
        {
            var handler = new RecordingHttpMessageHandler();
            var client = CreateClient(handler);
            var request = PapiRestRequest.Post(
                "/public/v1/1033/100/1/custom/endpoint",
                new { Value = "example-body" });

            await client.ExecutePapiAsync<PapiResponseCommon>(request);

            var captured = handler.SingleCustomRequest();
            Assert.AreEqual(HttpMethod.Post, captured.Method);
            Assert.IsFalse(string.IsNullOrWhiteSpace(captured.Body));
            StringAssert.Contains(captured.Body, "example-body");
            Assert.IsTrue(captured.Headers.ContainsKey("Authorization"));
            AssertAuthorizationHashesSentUri(captured, string.Empty);
        }

        [TestMethod]
        public async Task ExecutePapiAsync_CustomProtectedRequestWithProtectedTokenPlaceholder_AcquiresAndReplacesToken()
        {
            var handler = new RecordingHttpMessageHandler();
            var client = CreateClient(handler);
            client.AllowStaffOverrideRequests = true;
            client.StaffOverrideAccount = CreateStaffUser();
            var request = PapiRestRequest.Get($"/protected/v1/1033/100/1/{ProtectedToken.Placeholder}/custom/endpoint");

            await client.ExecutePapiAsync<PapiResponseCommon>(request);

            Assert.AreEqual(1, handler.AuthenticationRequestCount);
            var captured = handler.SingleCustomRequest();
            Assert.AreEqual("https://example.test/PAPIService/REST/protected/v1/1033/100/1/protected-token/custom/endpoint", captured.AbsoluteUri);
            Assert.IsFalse(captured.AbsoluteUri.Contains(ProtectedToken.Placeholder, StringComparison.Ordinal));
            AssertAuthorizationHashesSentUri(captured, "protected-secret");
        }

        [TestMethod]
        public async Task ExecutePapiAsync_CustomPublicRequest_UsesStaffOverrideWhenAllowed()
        {
            var handler = new RecordingHttpMessageHandler();
            var client = CreateClient(handler);
            client.AllowStaffOverrideRequests = true;
            client.StaffOverrideAccount = CreateStaffUser();
            var request = PapiRestRequest.Get("/public/v1/1033/100/1/custom/endpoint");

            await client.ExecutePapiAsync<PapiResponseCommon>(request);

            Assert.AreEqual(1, handler.AuthenticationRequestCount);
            var captured = handler.SingleCustomRequest();
            Assert.AreEqual("protected-token", captured.Headers["X-PAPI-AccessToken"]);
            AssertAuthorizationHashesSentUri(captured, "protected-secret");
        }

        [TestMethod]
        public async Task ExecutePapiAsync_CustomPublicRequest_BlockStaffOverridePreventsAccessTokenHeader()
        {
            var handler = new RecordingHttpMessageHandler();
            var client = CreateClient(handler);
            client.AllowStaffOverrideRequests = true;
            client.StaffOverrideAccount = CreateStaffUser();
            var request = PapiRestRequest.Get("/public/v1/1033/100/1/custom/endpoint");
            request.BlockStaffOverride = true;

            await client.ExecutePapiAsync<PapiResponseCommon>(request);

            Assert.AreEqual(0, handler.AuthenticationRequestCount);
            var captured = handler.SingleCustomRequest();
            Assert.IsFalse(captured.Headers.ContainsKey("X-PAPI-AccessToken"));
            AssertAuthorizationHashesSentUri(captured, string.Empty);
        }

        private static PapiClient CreateClient(RecordingHttpMessageHandler handler)
        {
            var httpClient = new HttpClient(handler)
            {
                BaseAddress = new Uri("https://example.test/")
            };

            return new PapiClient(httpClient, null)
            {
                Hostname = "https://example.test",
                AccessID = "access-id",
                AccessKey = "access-key",
                OrganizationId = 1,
                UserId = 1,
                WorkstationId = 1,
                UseProtectedTokenCache = false,
                AllowStaffOverrideRequests = false
            };
        }

        private static PolarisUser CreateStaffUser()
        {
            return new PolarisUser
            {
                Domain = "domain",
                Username = "staff",
                Password = "password"
            };
        }

        private static void AssertAuthorizationHashesSentUri(CapturedRequest request, string password)
        {
            var date = request.Headers["PolarisDate"];
            var expectedHash = ComputePapiHash(request.Method.Method, request.AbsoluteUri, date, password, "access-key");
            Assert.AreEqual($"PWS access-id:{expectedHash}", request.Headers["Authorization"]);
        }

        private static string ComputePapiHash(string httpMethod, string uri, string date, string password, string accessKey)
        {
            var hashString = httpMethod + uri + date + password;
            var computedHash = HMACSHA1.HashData(Encoding.UTF8.GetBytes(accessKey), Encoding.UTF8.GetBytes(hashString));
            return Convert.ToBase64String(computedHash);
        }

        private sealed class CapturedRequest
        {
            public CapturedRequest(HttpRequestMessage request, string body)
            {
                Method = request.Method;
                AbsoluteUri = request.RequestUri!.AbsoluteUri;
                Path = request.RequestUri.AbsolutePath;
                Body = body;
                Headers = request.Headers.ToDictionary(
                    header => header.Key,
                    header => string.Join(",", header.Value),
                    StringComparer.OrdinalIgnoreCase);
            }

            public HttpMethod Method { get; }
            public string AbsoluteUri { get; }
            public string Path { get; }
            public string Body { get; }
            public IReadOnlyDictionary<string, string> Headers { get; }
        }

        private sealed class RecordingHttpMessageHandler : HttpMessageHandler
        {
            private readonly List<CapturedRequest> _requests = new();

            public int RequestCount => _requests.Count;
            public int AuthenticationRequestCount => _requests.Count(request => IsStaffAuthenticationRequest(request));

            public CapturedRequest SingleCustomRequest()
            {
                return _requests.Single(request => !IsStaffAuthenticationRequest(request));
            }

            protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            {
                var body = request.Content == null
                    ? string.Empty
                    : await request.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
                var capturedRequest = new CapturedRequest(request, body);
                _requests.Add(capturedRequest);

                var responseJson = IsStaffAuthenticationRequest(capturedRequest)
                    ? "{\"PAPIErrorCode\":0,\"AccessToken\":\"protected-token\",\"AccessSecret\":\"protected-secret\",\"AuthExpDate\":\"2030-01-01T00:00:00Z\"}"
                    : "{\"PAPIErrorCode\":0}";

                return new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
                };
            }

            private static bool IsStaffAuthenticationRequest(CapturedRequest request)
            {
                return request.Method == HttpMethod.Post &&
                    request.Path.EndsWith("/authenticator/staff", StringComparison.OrdinalIgnoreCase);
            }
        }
    }
}
