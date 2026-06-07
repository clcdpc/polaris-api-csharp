using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Clc.Polaris.Api;
using Clc.Polaris.Api.Configuration;
using Clc.Polaris.Api.Models;
using Clc.Rest.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Clc.Polaris.Api.Tests
{
    public abstract class PapiClientTokenTestBase
    {
        [TestInitialize]
        public void TestInitialize()
        {
            ClearProtectedTokenState();
        }

        protected static async Task ExecuteRawPapiRequestAsync(PapiClient client, PapiRestRequest request)
        {
            await client.ExecutePapiAsync<PapiResponseCommon>(request).ConfigureAwait(false);
        }

        protected static bool InvokeIsStaffAuthenticatorRequest(PapiRestRequest? request)
        {
            var isStaffAuthenticatorRequest = typeof(PapiClient)
                .GetMethod("IsStaffAuthenticatorRequest", BindingFlags.Static | BindingFlags.NonPublic)!;

            return (bool)isStaffAuthenticatorRequest.Invoke(null, new object?[] { request })!;
        }

        protected static PapiRestRequest CreateStaffAuthenticatorRequestWithPath(string? path)
        {
            var request = PapiRestRequest.Post("/protected/v1/1033/100/1/authenticator/staff");
            request.Path = path!;

            return request;
        }

        protected static PapiClient CreateClient(CapturingHttpMessageHandler handler)
        {
            var hostname = $"https://example-{Guid.NewGuid():N}.test";
            var httpClient = new HttpClient(handler)
            {
                BaseAddress = new Uri($"{hostname}/")
            };

            return new PapiClient(httpClient, null)
            {
                Hostname = hostname,
                AccessID = "access-id",
                AccessKey = "access-key",
                UseProtectedTokenCache = false,
                AllowStaffOverrideRequests = false
            };
        }

        protected static PapiClient CreateProtectedClient(ProtectedTokenHttpMessageHandler handler)
        {
            return CreateProtectedClient(handler, $"https://example-{Guid.NewGuid():N}.test", CreateStaffUser());
        }

        protected static PapiClient CreateProtectedClient(ProtectedTokenHttpMessageHandler handler, string hostname, PolarisUser staffUser)
        {
            var httpClient = new HttpClient(handler)
            {
                BaseAddress = new Uri($"{hostname}/")
            };

            return new PapiClient(httpClient, null)
            {
                Hostname = hostname,
                AccessID = "access-id",
                AccessKey = "access-key",
                OrganizationId = 1,
                UseProtectedTokenCache = true,
                AllowStaffOverrideRequests = true,
                StaffOverrideAccount = staffUser
            };
        }

        protected static PolarisUser CreateStaffUser(string password = "password")
        {
            return new PolarisUser
            {
                Domain = "domain",
                Username = "user",
                Password = password
            };
        }

        protected static string CreateProtectedTokenJson(string accessToken, string accessSecret, DateTime expirationDate)
        {
            return
                "{" +
                $"\"PAPIErrorCode\":0," +
                $"\"AccessToken\":\"{accessToken}\"," +
                $"\"AccessSecret\":\"{accessSecret}\"," +
                $"\"AuthExpDate\":\"{expirationDate:O}\"" +
                "}";
        }

        protected static string CreateProtectedTokenJson(ProtectedToken token)
        {
            return CreateProtectedTokenJson(token.AccessToken!, token.AccessSecret!, token.ExpirationDate!.Value);
        }

        protected static void AssertFinalRequestsUseExpectedToken(ProtectedTokenHttpMessageHandler handler, string queryMarker, string expectedToken, string expectedSecret)
        {
            var matchingRequests = handler.CapturedRequests
                .Where(request => !request.IsStaffAuthenticationRequest && request.AbsoluteUri.Contains(queryMarker, StringComparison.Ordinal))
                .ToArray();
            Assert.IsTrue(matchingRequests.Length > 0, $"Expected at least one final request containing '{queryMarker}'.");
            foreach (var request in matchingRequests)
            {
                StringAssert.Contains(request.Path, $"/protected/v1/1033/100/1/{expectedToken}/search/patrons/Boolean");
                Assert.IsFalse(request.Path.Contains(ProtectedToken.Placeholder, StringComparison.Ordinal));
                AssertAuthorizationHash(request, expectedSecret, "access-key", "access-id");
            }
        }

        protected static void AssertAuthorizationHash(CapturedPapiRequest request, string secret, string accessKey, string accessId, string? absoluteUri = null)
        {
            Assert.IsTrue(request.Headers.TryGetValue("PolarisDate", out var date), "The request did not include a PolarisDate header.");
            Assert.IsTrue(request.Headers.TryGetValue("Authorization", out var authorization), "The request did not include an Authorization header.");
            Assert.AreEqual($"PWS {accessId}:{PapiSignature.ComputeHash(accessKey, request.Method, absoluteUri ?? request.AbsoluteUri, date, secret)}", authorization);
        }

        protected static void AssertAuthorizationHashDoesNotMatch(CapturedPapiRequest request, string secret, string accessKey, string accessId, string? absoluteUri = null)
        {
            Assert.IsTrue(request.Headers.TryGetValue("PolarisDate", out var date), "The request did not include a PolarisDate header.");
            Assert.IsTrue(request.Headers.TryGetValue("Authorization", out var authorization), "The request did not include an Authorization header.");
            Assert.AreNotEqual($"PWS {accessId}:{PapiSignature.ComputeHash(accessKey, request.Method, absoluteUri ?? request.AbsoluteUri, date, secret)}", authorization);
        }


        protected sealed class CapturingHttpMessageHandler : HttpMessageHandler
        {
            private readonly string _responseJson;
            private int _requestCount;

            public int RequestCount => _requestCount;
            public HttpRequestMessage? LastRequest { get; private set; }

            public CapturingHttpMessageHandler(string? responseJson = null)
            {
                _responseJson = responseJson ?? CreateProtectedTokenJson("new-token", "new-secret", DateTime.Now.AddHours(1));
            }

            protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            {
                Interlocked.Increment(ref _requestCount);
                LastRequest = request;

                return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(_responseJson, Encoding.UTF8, "application/json")
                });
            }
        }

        protected sealed class CapturedPapiRequest
        {
            public CapturedPapiRequest(HttpRequestMessage request, string body)
            {
                Method = request.Method.ToString();
                AbsoluteUri = request.RequestUri!.AbsoluteUri;
                Path = request.RequestUri.AbsolutePath;
                Body = body;
                IsStaffAuthenticationRequest = request.Method == HttpMethod.Post &&
                    Path.EndsWith("/authenticator/staff", StringComparison.OrdinalIgnoreCase) &&
                    Path.IndexOf(ProtectedToken.Placeholder, StringComparison.Ordinal) < 0;
                Headers = request.Headers
                    .ToDictionary(header => header.Key, header => string.Join(",", header.Value), StringComparer.OrdinalIgnoreCase);
            }

            public string Method { get; }
            public string AbsoluteUri { get; }
            public string Path { get; }
            public string Body { get; }
            public bool IsStaffAuthenticationRequest { get; }
            public IReadOnlyDictionary<string, string> Headers { get; }
        }

        protected sealed class ProtectedTokenHttpMessageHandler : HttpMessageHandler
        {
            private readonly HttpStatusCode _authenticationStatusCode;
            private readonly string _authenticationResponseJson;
            private readonly Func<CapturedPapiRequest, (HttpStatusCode StatusCode, string ResponseJson)>? _authenticationResponseFactory;
            private int _authenticationRequestCount;
            private int _nonAuthenticationRequestCount;
            private readonly ConcurrentQueue<string> _requestPaths = new ConcurrentQueue<string>();
            private readonly ConcurrentQueue<CapturedPapiRequest> _capturedRequests = new ConcurrentQueue<CapturedPapiRequest>();

            public int AuthenticationRequestCount => _authenticationRequestCount;
            public int NonAuthenticationRequestCount => _nonAuthenticationRequestCount;
            public string[] RequestPaths => _requestPaths.ToArray();
            public CapturedPapiRequest[] CapturedRequests => _capturedRequests.ToArray();

            public ProtectedTokenHttpMessageHandler(HttpStatusCode authenticationStatusCode = HttpStatusCode.OK, string? authenticationResponseJson = null)
            {
                _authenticationStatusCode = authenticationStatusCode;
                _authenticationResponseJson = authenticationResponseJson ?? CreateProtectedTokenJson("protected-token", "protected-secret", DateTime.Now.AddHours(1));
            }

            public ProtectedTokenHttpMessageHandler(Func<CapturedPapiRequest, (HttpStatusCode StatusCode, string ResponseJson)> authenticationResponseFactory)
                : this()
            {
                _authenticationResponseFactory = authenticationResponseFactory;
            }

            protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            {
                var body = request.Content == null ? string.Empty : await request.Content.ReadAsStringAsync().ConfigureAwait(false);
                var capturedRequest = new CapturedPapiRequest(request, body);
                _requestPaths.Enqueue(capturedRequest.Path);
                _capturedRequests.Enqueue(capturedRequest);

                if (capturedRequest.IsStaffAuthenticationRequest)
                {
                    Interlocked.Increment(ref _authenticationRequestCount);
                    await Task.Delay(50, cancellationToken).ConfigureAwait(false);
                    var authenticationResponse = _authenticationResponseFactory?.Invoke(capturedRequest) ?? (_authenticationStatusCode, _authenticationResponseJson);
                    return new HttpResponseMessage(authenticationResponse.StatusCode)
                    {
                        Content = new StringContent(authenticationResponse.ResponseJson, Encoding.UTF8, "application/json")
                    };
                }

                Interlocked.Increment(ref _nonAuthenticationRequestCount);
                return new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent("{\"PAPIErrorCode\":0}", Encoding.UTF8, "application/json")
                };
            }
        }

        protected static void ClearProtectedTokenState()
        {
            var cache = GetPrivateStaticProperty<ConcurrentDictionary<string, ProtectedToken>>("ProtectedTokenCache");
            cache?.Clear();

            var locks = GetPrivateStaticProperty<ConcurrentDictionary<string, SemaphoreSlim>>("ProtectedTokenCacheLocks");
            locks?.Clear();
        }

        protected static void SetCachedToken(string hostname, string accessId, string accessKey, PolarisUser? staffUser, ProtectedToken token)
        {
            var cacheKey = BuildCacheKey(hostname, accessId, accessKey, staffUser);
            if (cacheKey != null)
            {
                GetProtectedTokenCache().TryAdd(cacheKey, token);
            }
        }

        protected static bool TryGetCachedToken(string hostname, string accessId, string accessKey, PolarisUser? staffUser, out ProtectedToken? token)
        {
            token = null;
            var cacheKey = BuildCacheKey(hostname, accessId, accessKey, staffUser);
            return cacheKey != null && GetProtectedTokenCache().TryGetValue(cacheKey, out token);
        }

        protected static ConcurrentDictionary<string, ProtectedToken> GetProtectedTokenCache()
        {
            return GetPrivateStaticProperty<ConcurrentDictionary<string, ProtectedToken>>("ProtectedTokenCache")
                ?? throw new InvalidOperationException("Protected token cache was not available.");
        }

        protected static T? GetPrivateStaticProperty<T>(string propertyName) where T : class
        {
            var cacheProperty = typeof(PapiClient).GetProperty(propertyName, BindingFlags.NonPublic | BindingFlags.Static);
            return cacheProperty?.GetValue(null) as T;
        }

        protected static string? BuildCacheKey(string hostname, string accessId, string accessKey, PolarisUser? staffUser)
        {
            if (staffUser == null) { return null; }

            var client = new PapiClient
            {
                Hostname = hostname,
                AccessID = accessId,
                AccessKey = accessKey,
                StaffOverrideAccount = staffUser
            };

            return BuildCacheKey(client);
        }

        protected static string? BuildCacheKey(PapiClient client)
        {
            var buildCacheKeyMethod = typeof(PapiClient).GetMethod("BuildProtectedTokenCacheKey", BindingFlags.Instance | BindingFlags.NonPublic)
                ?? throw new InvalidOperationException("Protected token cache-key builder was not available.");
            return buildCacheKeyMethod.Invoke(client, Array.Empty<object>()) as string;
        }
    }
}
