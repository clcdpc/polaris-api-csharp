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
            ClearProtectedTokenState();
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
        public void Token_WhenTokenIsNullAndStaffOverrideAccountExistsAndCacheHasValidToken_ReturnsNullWithoutCacheLookupOrAuthenticating()
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
        public void Token_WhenExistingTokenIsExpiredAndStaffOverrideAccountExistsAndCacheHasValidToken_ReturnsExistingTokenWithoutCacheLookupOrAuthenticating()
        {
            var handler = new CapturingHttpMessageHandler();
            var client = CreateClient(handler);
            var staff = CreateStaffUser();
            client.StaffOverrideAccount = staff;
            client.UseProtectedTokenCache = true;
            client.Token = new ProtectedToken
            {
                AccessToken = "expired-token",
                AccessSecret = "expired-secret",
                ExpirationDate = DateTime.Now.AddHours(-1)
            };
            SetCachedToken(client.Hostname, staff, new ProtectedToken
            {
                AccessToken = "cached-token",
                AccessSecret = "cached-secret",
                ExpirationDate = DateTime.Now.AddHours(1)
            });

            var token = client.Token;

            Assert.IsNotNull(token);
            Assert.AreEqual("expired-token", token.AccessToken);
            Assert.AreEqual(0, handler.RequestCount);
        }

        [TestMethod]
        public void Token_WhenExistingTokenIsExpiredAndNoStaffOverrideAccount_ReturnsExistingTokenWithoutAuthenticating()
        {
            var handler = new CapturingHttpMessageHandler();
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
        public void Token_WhenCacheEnabledTokenNullAndNoStaffOverrideAccount_ReturnsNullWithoutThrowing()
        {
            var handler = new CapturingHttpMessageHandler();
            var client = CreateClient(handler);
            client.UseProtectedTokenCache = true;
            client.StaffOverrideAccount = null;
            client.Token = null;

            var token = client.Token;

            Assert.IsNull(token);
            Assert.AreEqual(0, handler.RequestCount);
        }

        [TestMethod]
        public async Task AuthenticateStaffUserAsync_ReturnsTokenButDoesNotSetClientToken()
        {
            var handler = new CapturingHttpMessageHandler(CreateProtectedTokenJson("returned-token", "returned-secret", DateTime.Now.AddHours(1)));
            var client = CreateClient(handler);
            var staffUser = CreateStaffUser();
            client.AllowStaffOverrideRequests = true;
            client.UseProtectedTokenCache = true;
            client.StaffOverrideAccount = staffUser;

            var response = await client.AuthenticateStaffUserAsync(staffUser);

            Assert.IsNotNull(response.Data);
            Assert.AreEqual("returned-token", response.Data.AccessToken);
            Assert.IsNull(client.Token);
            Assert.AreEqual(1, handler.RequestCount);
            Assert.IsFalse(TryGetCachedToken(client.Hostname, staffUser, out _));
        }

        [TestMethod]
        public async Task PatronSearchAsync_ConcurrentProtectedRequestsForSameCacheKey_AuthenticateOnce()
        {
            var handler = new ProtectedTokenHttpMessageHandler();
            var client = CreateProtectedClient(handler);

            var requests = Enumerable.Range(0, 8)
                .Select(index => client.PatronSearchAsync($"name={index}"))
                .ToArray();

            await Task.WhenAll(requests);

            Assert.AreEqual(1, handler.AuthenticationRequestCount);
            Assert.AreEqual(8, handler.ProtectedRequestCount);
            Assert.IsNotNull(client.Token);
            Assert.AreEqual("protected-token", client.Token.AccessToken);
        }

        [TestMethod]
        public async Task PatronSearchAsync_WithCachedValidToken_AvoidsStaffAuthentication()
        {
            var handler = new ProtectedTokenHttpMessageHandler();
            var client = CreateProtectedClient(handler);
            SetCachedToken(client.Hostname, client.StaffOverrideAccount, new ProtectedToken
            {
                AccessToken = "cached-token",
                AccessSecret = "cached-secret",
                ExpirationDate = DateTime.Now.AddHours(1)
            });

            await client.PatronSearchAsync("name=Smith");

            Assert.AreEqual(0, handler.AuthenticationRequestCount);
            Assert.AreEqual(1, handler.ProtectedRequestCount);
            Assert.AreEqual("cached-token", client.Token.AccessToken);
        }

        [TestMethod]
        public async Task PatronSearchAsync_SuccessfulStaffAuthentication_LoadsTokenAndCachesIt()
        {
            var handler = new ProtectedTokenHttpMessageHandler();
            var client = CreateProtectedClient(handler);

            await client.PatronSearchAsync("name=Smith");

            Assert.AreEqual(1, handler.AuthenticationRequestCount);
            Assert.AreEqual(1, handler.ProtectedRequestCount);
            Assert.IsNotNull(client.Token);
            Assert.AreEqual("protected-token", client.Token.AccessToken);
            Assert.IsTrue(TryGetCachedToken(client.Hostname, client.StaffOverrideAccount, out var cachedToken));
            Assert.IsNotNull(cachedToken);
            Assert.AreEqual("protected-token", cachedToken.AccessToken);
            Assert.AreNotSame(client.Token, cachedToken);
        }

        [TestMethod]
        public async Task PatronAccountGetAsync_FailedStaffAuthentication_DoesNotPopulateProtectedTokenCache()
        {
            var handler = new ProtectedTokenHttpMessageHandler(HttpStatusCode.InternalServerError, "{\"PAPIErrorCode\":1}");
            var client = CreateProtectedClient(handler);

            var response = await client.PatronAccountGetAsync("ABC123");

            Assert.IsNotNull(response);
            Assert.AreEqual(1, handler.AuthenticationRequestCount);
            Assert.IsNull(client.Token);
            Assert.IsFalse(TryGetCachedToken(client.Hostname, client.StaffOverrideAccount, out _));
        }

        [TestMethod]
        public async Task PatronAccountGetAsync_NullDataStaffAuthentication_DoesNotPopulateProtectedTokenCache()
        {
            var handler = new ProtectedTokenHttpMessageHandler(HttpStatusCode.OK, "{}");
            var client = CreateProtectedClient(handler);

            var response = await client.PatronAccountGetAsync("ABC123");

            Assert.IsNotNull(response);
            Assert.AreEqual(1, handler.AuthenticationRequestCount);
            Assert.IsNull(client.Token);
            Assert.IsFalse(TryGetCachedToken(client.Hostname, client.StaffOverrideAccount, out _));
        }

        [TestMethod]
        public async Task PatronAccountGetAsync_FailedStaffAuthentication_DoesNotOverwriteExistingToken()
        {
            var handler = new ProtectedTokenHttpMessageHandler(HttpStatusCode.InternalServerError, "{\"PAPIErrorCode\":1}");
            var client = CreateProtectedClient(handler);
            client.Token = new ProtectedToken
            {
                AccessToken = "existing-token",
                AccessSecret = "existing-secret",
                ExpirationDate = DateTime.Now.AddHours(-1)
            };

            var response = await client.PatronAccountGetAsync("ABC123");

            Assert.IsNotNull(response);
            Assert.AreEqual(1, handler.AuthenticationRequestCount);
            Assert.IsNotNull(client.Token);
            Assert.AreEqual("existing-token", client.Token.AccessToken);
            Assert.IsFalse(TryGetCachedToken(client.Hostname, client.StaffOverrideAccount, out _));
        }

        [TestMethod]
        public async Task PatronAccountGetAsync_NullDataStaffAuthentication_DoesNotOverwriteExistingToken()
        {
            var handler = new ProtectedTokenHttpMessageHandler(HttpStatusCode.OK, "{}");
            var client = CreateProtectedClient(handler);
            client.Token = new ProtectedToken
            {
                AccessToken = "existing-token",
                AccessSecret = "existing-secret",
                ExpirationDate = DateTime.Now.AddHours(-1)
            };

            var response = await client.PatronAccountGetAsync("ABC123");

            Assert.IsNotNull(response);
            Assert.AreEqual(1, handler.AuthenticationRequestCount);
            Assert.IsNotNull(client.Token);
            Assert.AreEqual("existing-token", client.Token.AccessToken);
            Assert.IsFalse(TryGetCachedToken(client.Hostname, client.StaffOverrideAccount, out _));
        }

        [TestMethod]
        public void PreformatRestRequest_ManualToken_UsesProtectedAndPublicOverrideFormatting()
        {
            var handler = new CapturingHttpMessageHandler();
            var client = CreateClient(handler);
            client.AllowStaffOverrideRequests = true;
            client.Token = new ProtectedToken
            {
                AccessToken = "manual-token",
                AccessSecret = "manual-secret",
                ExpirationDate = DateTime.Now.AddHours(1)
            };

            var publicRequest = (PapiRestRequest)client.PreformatRestRequest(new PapiRestRequest(HttpMethod.Get, "/public/v1/1033/100/1/patron/ABC"));
            var protectedRequest = (PapiRestRequest)client.PreformatRestRequest(new PapiRestRequest(HttpMethod.Get, "/protected/v1/1033/100/1/manual-token/search/patrons/Boolean"));

            Assert.AreEqual("manual-token", publicRequest.Headers["X-PAPI-AccessToken"]);
            Assert.IsTrue(publicRequest.Headers.ContainsKey("Authorization"));
            Assert.IsFalse(protectedRequest.Headers.ContainsKey("X-PAPI-AccessToken"));
            Assert.IsTrue(protectedRequest.Headers.ContainsKey("Authorization"));
        }

        private static PapiClient CreateClient(CapturingHttpMessageHandler handler)
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

        private static PapiClient CreateProtectedClient(ProtectedTokenHttpMessageHandler handler)
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
                OrganizationId = 1,
                UseProtectedTokenCache = true,
                AllowStaffOverrideRequests = true,
                StaffOverrideAccount = CreateStaffUser()
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
            private int _requestCount;

            public int RequestCount => _requestCount;
            public HttpRequestMessage? LastRequest { get; private set; }

            public CapturingHttpMessageHandler(string? responseJson = null)
            {
                _responseJson = responseJson ?? CreateProtectedTokenJson("new-token", "new-secret", DateTime.UtcNow.AddHours(1));
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

        private sealed class ProtectedTokenHttpMessageHandler : HttpMessageHandler
        {
            private readonly HttpStatusCode _authenticationStatusCode;
            private readonly string _authenticationResponseJson;
            private int _authenticationRequestCount;
            private int _protectedRequestCount;

            public int AuthenticationRequestCount => _authenticationRequestCount;
            public int ProtectedRequestCount => _protectedRequestCount;

            public ProtectedTokenHttpMessageHandler(HttpStatusCode authenticationStatusCode = HttpStatusCode.OK, string? authenticationResponseJson = null)
            {
                _authenticationStatusCode = authenticationStatusCode;
                _authenticationResponseJson = authenticationResponseJson ?? CreateProtectedTokenJson("protected-token", "protected-secret", DateTime.Now.AddHours(1));
            }

            protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            {
                if (request.RequestUri!.AbsolutePath.Contains("/authenticator/staff"))
                {
                    Interlocked.Increment(ref _authenticationRequestCount);
                    await Task.Delay(50, cancellationToken).ConfigureAwait(false);
                    return new HttpResponseMessage(_authenticationStatusCode)
                    {
                        Content = new StringContent(_authenticationResponseJson, Encoding.UTF8, "application/json")
                    };
                }

                Interlocked.Increment(ref _protectedRequestCount);
                return new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent("{\"PAPIErrorCode\":0}", Encoding.UTF8, "application/json")
                };
            }
        }

        private static void ClearProtectedTokenState()
        {
            var cache = GetPrivateStaticProperty<ConcurrentDictionary<string, ProtectedToken>>("ProtectedTokenCache");
            cache?.Clear();

            var locks = GetPrivateStaticProperty<ConcurrentDictionary<string, SemaphoreSlim>>("ProtectedTokenCacheLocks");
            locks?.Clear();
        }

        private static void SetCachedToken(string hostname, PolarisUser staffUser, ProtectedToken token)
        {
            var cache = GetPrivateStaticProperty<ConcurrentDictionary<string, ProtectedToken>>("ProtectedTokenCache");
            cache?.TryAdd(BuildCacheKey(hostname, staffUser), token);
        }

        private static bool TryGetCachedToken(string hostname, PolarisUser staffUser, out ProtectedToken? token)
        {
            token = null;
            var cache = GetPrivateStaticProperty<ConcurrentDictionary<string, ProtectedToken>>("ProtectedTokenCache");
            return cache?.TryGetValue(BuildCacheKey(hostname, staffUser), out token) == true;
        }

        private static T? GetPrivateStaticProperty<T>(string propertyName) where T : class
        {
            var cacheProperty = typeof(PapiClient).GetProperty(propertyName, BindingFlags.NonPublic | BindingFlags.Static);
            return cacheProperty?.GetValue(null) as T;
        }

        private static string BuildCacheKey(string hostname, PolarisUser staffUser)
        {
            return $"{hostname}|{staffUser.Domain}|{staffUser.Username}";
        }
    }
}
