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
    [TestCategory("Unit")]
    [TestCategory("RestClientMigration")]
    [DoNotParallelize]
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
            var handler = new ProtectedTokenHttpMessageHandler();
            var client = CreateClient(handler);

            var token = client.Token;

            Assert.IsNull(token);
            Assert.AreEqual(0, handler.RequestCount);
        }

        [TestMethod]
        public void Token_WhenCacheContainsToken_ReturnsCurrentInMemoryTokenWithoutReadingCache()
        {
            var handler = new ProtectedTokenHttpMessageHandler();
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
            var handler = new ProtectedTokenHttpMessageHandler();
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
        public void Token_WhenExistingTokenIsExpiredAndNoStaffOverrideAccount_ReturnsExistingTokenWithoutAuthenticating()
        {
            var handler = new ProtectedTokenHttpMessageHandler();
            var client = CreateClient(handler);
            client.Token = new ProtectedToken
            {
                AccessToken = "expired-token",
                AccessSecret = "expired-secret",
                ExpirationDate = DateTime.Now.AddHours(-1)
            };

            var token = client.Token;

            Assert.IsNotNull(token);
            Assert.AreEqual("expired-token", token.AccessToken);
            Assert.AreEqual(0, handler.RequestCount);
        }

        [TestMethod]
        public async Task ProtectedRequest_WhenCacheHasValidToken_DoesNotAuthenticateStaffUser()
        {
            var handler = new ProtectedTokenHttpMessageHandler();
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

            await client.PatronSearchAsync("name=Smith").ConfigureAwait(false);

            Assert.AreEqual("cached-token", client.Token.AccessToken);
            Assert.AreEqual(0, handler.StaffAuthenticationRequestCount);
            Assert.AreEqual(1, handler.ProtectedRequestCount);
            StringAssert.Contains(handler.Requests.Single().RequestUri!.AbsolutePath, "/protected/v1/1033/100/1/cached-token/search/patrons/Boolean");
        }

        [TestMethod]
        public async Task ProtectedRequest_WhenStaffAuthenticationReturnsNullData_DoesNotCacheToken()
        {
            var handler = new ProtectedTokenHttpMessageHandler(authResponseJson: "{\"PAPIErrorCode\":0}");
            var client = CreateClient(handler);
            client.StaffOverrideAccount = CreateStaffUser();
            client.AllowStaffOverrideRequests = true;
            client.UseProtectedTokenCache = true;

            await client.PatronBasicDataGetAsync("123").ConfigureAwait(false);

            Assert.IsNull(client.Token);
            Assert.AreEqual(1, handler.StaffAuthenticationRequestCount);
            Assert.IsNull(GetCachedToken(client.Hostname, client.StaffOverrideAccount));
        }

        [TestMethod]
        public async Task ProtectedRequest_WhenStaffAuthenticationFails_DoesNotCacheToken()
        {
            var handler = new ProtectedTokenHttpMessageHandler(HttpStatusCode.Unauthorized);
            var client = CreateClient(handler);
            client.StaffOverrideAccount = CreateStaffUser();
            client.AllowStaffOverrideRequests = true;
            client.UseProtectedTokenCache = true;

            await client.PatronBasicDataGetAsync("123").ConfigureAwait(false);

            Assert.IsNull(client.Token);
            Assert.AreEqual(1, handler.StaffAuthenticationRequestCount);
            Assert.IsNull(GetCachedToken(client.Hostname, client.StaffOverrideAccount));
        }

        [TestMethod]
        public async Task ProtectedRequests_WhenConcurrentForSameCacheKey_AuthenticateStaffUserOnce()
        {
            var handler = new ProtectedTokenHttpMessageHandler(authResponseDelay: TimeSpan.FromMilliseconds(100));
            var client = CreateClient(handler);
            client.StaffOverrideAccount = CreateStaffUser();
            client.UseProtectedTokenCache = true;
            var requests = Enumerable.Range(0, 8)
                .Select(i => client.PatronSearchAsync($"name=Smith{i}"))
                .ToArray();

            await Task.WhenAll(requests).ConfigureAwait(false);

            Assert.AreEqual(1, handler.StaffAuthenticationRequestCount);
            Assert.AreEqual(8, handler.ProtectedRequestCount);
            Assert.AreEqual("new-token", client.Token.AccessToken);
            Assert.IsNotNull(GetCachedToken(client.Hostname, client.StaffOverrideAccount));
        }

        [TestMethod]
        public void PreformatRestRequest_WhenTokenIsSetManually_UsesTokenForStaffOverrideFormatting()
        {
            var handler = new ProtectedTokenHttpMessageHandler();
            var client = CreateClient(handler);
            client.AllowStaffOverrideRequests = true;
            client.Token = new ProtectedToken
            {
                AccessToken = "manual-token",
                AccessSecret = "manual-secret",
                ExpirationDate = DateTime.Now.AddHours(1)
            };
            var request = new PapiRestRequest(HttpMethod.Get, "/public/v1/1033/100/1/patron/123/basicdata");

            var formatted = (PapiRestRequest)client.PreformatRestRequest(request);

            Assert.AreEqual("manual-token", formatted.Headers["X-PAPI-AccessToken"]);
            Assert.IsTrue(formatted.Headers.ContainsKey("Authorization"));
            Assert.AreEqual(0, handler.RequestCount);
        }

        private static PapiClient CreateClient(ProtectedTokenHttpMessageHandler handler)
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

        private sealed class ProtectedTokenHttpMessageHandler : HttpMessageHandler
        {
            private readonly string _authResponseJson;
            private readonly HttpStatusCode _authStatusCode;
            private readonly TimeSpan _authResponseDelay;
            private int _requestCount;
            private int _staffAuthenticationRequestCount;
            private int _protectedRequestCount;

            public int RequestCount => _requestCount;
            public int StaffAuthenticationRequestCount => _staffAuthenticationRequestCount;
            public int ProtectedRequestCount => _protectedRequestCount;
            public ConcurrentBag<HttpRequestMessage> Requests { get; } = new ConcurrentBag<HttpRequestMessage>();

            public ProtectedTokenHttpMessageHandler(
                HttpStatusCode authStatusCode = HttpStatusCode.OK,
                string? authResponseJson = null,
                TimeSpan? authResponseDelay = null)
            {
                _authStatusCode = authStatusCode;
                _authResponseJson = authResponseJson ?? CreateProtectedTokenJson("new-token", "new-secret", DateTime.Now.AddHours(1));
                _authResponseDelay = authResponseDelay ?? TimeSpan.Zero;
            }

            protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            {
                Interlocked.Increment(ref _requestCount);
                Requests.Add(request);

                if (request.RequestUri!.AbsolutePath.Contains("/authenticator/staff"))
                {
                    Interlocked.Increment(ref _staffAuthenticationRequestCount);
                    if (_authResponseDelay > TimeSpan.Zero)
                    {
                        await Task.Delay(_authResponseDelay, cancellationToken).ConfigureAwait(false);
                    }

                    return new HttpResponseMessage(_authStatusCode)
                    {
                        Content = new StringContent(_authResponseJson, Encoding.UTF8, "application/json")
                    };
                }

                Interlocked.Increment(ref _protectedRequestCount);
                return new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent("{\"PAPIErrorCode\":0}", Encoding.UTF8, "application/json")
                };
            }
        }

        private static void ClearProtectedTokenCache()
        {
            var cache = GetProtectedTokenCache();
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
            GetProtectedTokenCache()?.TryAdd(BuildCacheKey(hostname, staffUser), token);
        }

        private static ProtectedToken? GetCachedToken(string hostname, PolarisUser staffUser)
        {
            var cache = GetProtectedTokenCache();
            return cache != null && cache.TryGetValue(BuildCacheKey(hostname, staffUser), out var token) ? token : null;
        }

        private static ConcurrentDictionary<string, ProtectedToken>? GetProtectedTokenCache()
        {
            var cacheProperty = typeof(PapiClient).GetProperty("ProtectedTokenCache", BindingFlags.NonPublic | BindingFlags.Static);
            return cacheProperty?.GetValue(null) as ConcurrentDictionary<string, ProtectedToken>;
        }

        private static string BuildCacheKey(string hostname, PolarisUser staffUser)
        {
            return $"{hostname}{staffUser.Domain}{staffUser.Username}";
        }
    }
}
