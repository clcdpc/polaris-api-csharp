using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Clc.Polaris.Api;
using Clc.Polaris.Api.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Clc.Polaris.Api.Tests
{
    [TestClass]
    [DoNotParallelize]
    [TestCategory("RestClientMigration")]
    public class PapiClientTokenTests
    {
        [TestInitialize]
        public void TestInitialize()
        {
            ClearProtectedTokenCache();
            ClearProtectedTokenLocks();
        }

        [TestMethod]
        public void Token_WhenTokenIsNullAndNoStaffOverrideAccount_ReturnsNullWithoutAuthenticating()
        {
            var handler = new CapturingHttpMessageHandler();
            var client = CreateClient(handler);

            var token = client.Token;

            Assert.IsNull(token);
            Assert.AreEqual(0, handler.RequestCount);
        }

        [TestMethod]
        public void Token_WhenCacheHasValidToken_ReturnsCurrentTokenOnlyWithoutCacheLookupOrAuthenticating()
        {
            var handler = new CapturingHttpMessageHandler();
            var client = CreateClient(handler);
            var staff = CreateStaffUser();
            client.StaffOverrideAccount = staff;
            client.UseProtectedTokenCache = true;
            SetCachedToken(client.Hostname, staff, new ProtectedToken
            {
                AccessToken = "cached-token",
                AccessSecret = "cached-secret",
                ExpirationDate = DateTime.Now.AddHours(1)
            });

            var token = client.Token;

            Assert.IsNull(token);
            Assert.AreEqual(0, handler.RequestCount);
        }

        [TestMethod]
        public void Token_WhenExistingTokenIsNotExpired_ReturnsExistingTokenWithoutAuthenticating()
        {
            var handler = new CapturingHttpMessageHandler();
            var client = CreateClient(handler);
            client.StaffOverrideAccount = CreateStaffUser();
            client.Token = new ProtectedToken
            {
                AccessToken = "existing-token",
                AccessSecret = "existing-secret",
                ExpirationDate = DateTime.Now.AddHours(1)
            };

            var token = client.Token;

            Assert.IsNotNull(token);
            Assert.AreEqual("existing-token", token.AccessToken);
            Assert.AreEqual(0, handler.RequestCount);
        }

        [TestMethod]
        public async Task ProtectedRequest_WhenCacheHasValidToken_UsesCachedTokenWithoutStaffAuthentication()
        {
            var handler = new CapturingHttpMessageHandler(responseJson: "{\"PAPIErrorCode\":0}");
            var client = CreateClient(handler);
            var staff = CreateStaffUser();
            client.StaffOverrideAccount = staff;
            client.UseProtectedTokenCache = true;
            SetCachedToken(client.Hostname, staff, new ProtectedToken
            {
                AccessToken = "cached-token",
                AccessSecret = "cached-secret",
                ExpirationDate = DateTime.Now.AddHours(1)
            });

            var response = await client.PatronSearchAsync("name=Smith");

            Assert.IsNotNull(response);
            Assert.AreEqual(1, handler.RequestCount);
            Assert.AreEqual(0, handler.StaffAuthenticationRequestCount);
            StringAssert.Contains(handler.LastRequest!.RequestUri!.AbsolutePath, "/cached-token/search/patrons/Boolean");
            Assert.AreEqual("cached-token", client.Token!.AccessToken);
        }

        [TestMethod]
        public async Task ProtectedRequests_WhenConcurrentAndNoCachedToken_AuthenticateOnlyOnceForSameCacheKey()
        {
            var handler = new CapturingHttpMessageHandler(authDelay: TimeSpan.FromMilliseconds(100));
            var hostname = $"https://example-{Guid.NewGuid():N}.test";
            var clients = Enumerable.Range(0, 8)
                .Select(_ =>
                {
                    var client = CreateClient(handler, hostname);
                    client.StaffOverrideAccount = CreateStaffUser();
                    client.UseProtectedTokenCache = true;
                    client.AllowStaffOverrideRequests = true;
                    return client;
                })
                .ToArray();

            var tasks = clients
                .Select((client, index) => client.PatronSearchAsync($"name=Smith{index}"))
                .ToArray();

            await Task.WhenAll(tasks);

            Assert.AreEqual(1, handler.StaffAuthenticationRequestCount);
            Assert.AreEqual(8, handler.ProtectedSearchRequestCount);
            Assert.AreEqual(9, handler.RequestCount);
            Assert.IsTrue(handler.Requests.All(request =>
                IsStaffAuthenticationRequest(request) || request.RequestUri!.AbsolutePath.Contains("/new-token/search/patrons/Boolean")));
        }

        [TestMethod]
        public async Task PublicOverride_WhenStaffAuthenticationFails_DoesNotPopulateCacheOrThrowNullReferenceException()
        {
            var handler = new CapturingHttpMessageHandler(
                responseJson: "{\"PAPIErrorCode\":0}",
                staffAuthenticationStatusCode: HttpStatusCode.InternalServerError,
                staffAuthenticationResponseJson: "{\"PAPIErrorCode\":-1,\"AccessToken\":\"failed-token\",\"AccessSecret\":\"failed-secret\",\"AuthExpDate\":\"2030-01-01T00:00:00Z\"}");
            var client = CreateClient(handler);
            client.StaffOverrideAccount = CreateStaffUser();
            client.UseProtectedTokenCache = true;
            client.AllowStaffOverrideRequests = true;

            var response = await client.ApiVersionGetAsync();

            Assert.IsNotNull(response);
            Assert.AreEqual(2, handler.RequestCount);
            Assert.AreEqual(1, handler.StaffAuthenticationRequestCount);
            Assert.IsFalse(TryGetCachedToken(client.Hostname, client.StaffOverrideAccount, out _));
            Assert.IsNull(client.Token);
        }

        [TestMethod]
        public async Task PublicOverride_WhenStaffAuthenticationReturnsNullData_DoesNotPopulateCache()
        {
            var handler = new CapturingHttpMessageHandler(
                responseJson: "{\"PAPIErrorCode\":0}",
                staffAuthenticationResponseJson: "null");
            var client = CreateClient(handler);
            client.StaffOverrideAccount = CreateStaffUser();
            client.UseProtectedTokenCache = true;
            client.AllowStaffOverrideRequests = true;

            var response = await client.ApiVersionGetAsync();

            Assert.IsNotNull(response);
            Assert.AreEqual(2, handler.RequestCount);
            Assert.AreEqual(1, handler.StaffAuthenticationRequestCount);
            Assert.IsFalse(TryGetCachedToken(client.Hostname, client.StaffOverrideAccount, out _));
            Assert.IsNull(client.Token);
        }

        [TestMethod]
        public async Task PublicOverride_WhenTokenSetManually_FormatsOverrideWithManualToken()
        {
            var handler = new CapturingHttpMessageHandler(responseJson: "{\"PAPIErrorCode\":0}");
            var client = CreateClient(handler);
            client.AllowStaffOverrideRequests = true;
            client.StaffOverrideAccount = null;
            client.Token = new ProtectedToken
            {
                AccessToken = "manual-token",
                AccessSecret = "manual-secret",
                ExpirationDate = DateTime.Now.AddHours(1)
            };

            var response = await client.ApiVersionGetAsync();

            Assert.IsNotNull(response);
            Assert.AreEqual(1, handler.RequestCount);
            Assert.AreEqual(0, handler.StaffAuthenticationRequestCount);
            Assert.IsTrue(handler.LastRequest!.Headers.TryGetValues("X-PAPI-AccessToken", out var values));
            Assert.AreEqual("manual-token", values.Single());
            Assert.IsTrue(handler.LastRequest.Headers.Contains("Authorization"));
        }

        private static PapiClient CreateClient(CapturingHttpMessageHandler handler, string? hostname = null)
        {
            hostname ??= $"https://example-{Guid.NewGuid():N}.test";
            var httpClient = new HttpClient(handler, disposeHandler: false)
            {
                BaseAddress = new Uri($"{hostname}/")
            };

            return new PapiClient(httpClient, null)
            {
                Hostname = hostname,
                AccessID = "access-id",
                AccessKey = "access-key",
                OrganizationId = 1,
                UseProtectedTokenCache = false,
                AllowStaffOverrideRequests = false
            };
        }

        private static PolarisUser CreateStaffUser()
        {
            return new PolarisUser
            {
                Domain = "domain",
                Username = "user",
                Password = "password"
            };
        }

        private static string CreateProtectedTokenJson(string accessToken, string accessSecret, DateTime expirationDate)
        {
            return
                "{" +
                $"\"PAPIErrorCode\":0," +
                $"\"AccessToken\":\"{accessToken}\"," +
                $"\"AccessSecret\":\"{accessSecret}\"," +
                $"\"AuthExpDate\":\"{expirationDate:O}\"" +
                "}";
        }

        private sealed class CapturingHttpMessageHandler : HttpMessageHandler
        {
            private readonly string _responseJson;
            private readonly string _staffAuthenticationResponseJson;
            private readonly HttpStatusCode _staffAuthenticationStatusCode;
            private readonly TimeSpan _authDelay;
            private int _requestCount;
            private int _staffAuthenticationRequestCount;
            private int _protectedSearchRequestCount;

            public int RequestCount => _requestCount;
            public int StaffAuthenticationRequestCount => _staffAuthenticationRequestCount;
            public int ProtectedSearchRequestCount => _protectedSearchRequestCount;
            public HttpRequestMessage? LastRequest { get; private set; }
            public ConcurrentBag<HttpRequestMessage> Requests { get; } = new ConcurrentBag<HttpRequestMessage>();

            public CapturingHttpMessageHandler(
                string? responseJson = null,
                HttpStatusCode staffAuthenticationStatusCode = HttpStatusCode.OK,
                string? staffAuthenticationResponseJson = null,
                TimeSpan? authDelay = null)
            {
                _responseJson = responseJson ?? "{\"PAPIErrorCode\":0}";
                _staffAuthenticationResponseJson = staffAuthenticationResponseJson ?? CreateProtectedTokenJson("new-token", "new-secret", DateTime.Now.AddHours(1));
                _staffAuthenticationStatusCode = staffAuthenticationStatusCode;
                _authDelay = authDelay ?? TimeSpan.Zero;
            }

            protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            {
                Interlocked.Increment(ref _requestCount);
                LastRequest = request;
                Requests.Add(request);

                if (IsStaffAuthenticationRequest(request))
                {
                    Interlocked.Increment(ref _staffAuthenticationRequestCount);

                    if (_authDelay > TimeSpan.Zero)
                    {
                        await Task.Delay(_authDelay, cancellationToken).ConfigureAwait(false);
                    }

                    return new HttpResponseMessage(_staffAuthenticationStatusCode)
                    {
                        Content = new StringContent(_staffAuthenticationResponseJson, Encoding.UTF8, "application/json")
                    };
                }

                if (request.RequestUri!.AbsolutePath.Contains("/search/patrons/Boolean"))
                {
                    Interlocked.Increment(ref _protectedSearchRequestCount);
                }

                return new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(_responseJson, Encoding.UTF8, "application/json")
                };
            }
        }

        private static bool IsStaffAuthenticationRequest(HttpRequestMessage request)
        {
            return request.RequestUri!.AbsolutePath.EndsWith("/protected/v1/1033/100/1/authenticator/staff", StringComparison.OrdinalIgnoreCase);
        }

        private static void ClearProtectedTokenCache()
        {
            var cacheProperty = typeof(PapiClient).GetProperty("ProtectedTokenCache", BindingFlags.NonPublic | BindingFlags.Static);
            var cache = cacheProperty?.GetValue(null) as ConcurrentDictionary<string, ProtectedToken>;
            cache?.Clear();
        }

        private static void ClearProtectedTokenLocks()
        {
            var locksProperty = typeof(PapiClient).GetProperty("ProtectedTokenLocks", BindingFlags.NonPublic | BindingFlags.Static);
            var locks = locksProperty?.GetValue(null) as ConcurrentDictionary<string, SemaphoreSlim>;
            locks?.Clear();
        }

        private static void SetCachedToken(string hostname, PolarisUser staffUser, ProtectedToken token)
        {
            var cacheProperty = typeof(PapiClient).GetProperty("ProtectedTokenCache", BindingFlags.NonPublic | BindingFlags.Static);
            var cache = cacheProperty?.GetValue(null) as ConcurrentDictionary<string, ProtectedToken>;
            cache?.TryAdd($"{hostname}{staffUser.Domain}{staffUser.Username}", token);
        }

        private static bool TryGetCachedToken(string hostname, PolarisUser staffUser, out ProtectedToken? token)
        {
            var cacheProperty = typeof(PapiClient).GetProperty("ProtectedTokenCache", BindingFlags.NonPublic | BindingFlags.Static);
            var cache = cacheProperty?.GetValue(null) as ConcurrentDictionary<string, ProtectedToken>;

            if (cache == null)
            {
                token = null;
                return false;
            }

            return cache.TryGetValue($"{hostname}{staffUser.Domain}{staffUser.Username}", out token);
        }
    }
}
