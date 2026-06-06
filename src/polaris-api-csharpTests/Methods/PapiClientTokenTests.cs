using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Security.Cryptography;
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
            SetCachedToken(client.Hostname, client.AccessID, client.AccessKey, staff, new ProtectedToken
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
        public void Token_WhenExistingTokenIsExpiredAndStaffOverrideAccountExistsAndCacheHasValidToken_ReturnsNullAndClearsTokenWithoutCacheLookupOrAuthenticating()
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
            SetCachedToken(client.Hostname, client.AccessID, client.AccessKey, staff, new ProtectedToken
            {
                AccessToken = "cached-token",
                AccessSecret = "cached-secret",
                ExpirationDate = DateTime.Now.AddHours(1)
            });

            var token = client.Token;

            Assert.IsNull(token);
            Assert.IsNull(client.Token);
            Assert.AreEqual(0, handler.RequestCount);
        }

        [TestMethod]
        public void Token_WhenExistingTokenIsExpiredAndNoStaffOverrideAccount_ReturnsNullAndClearsTokenWithoutAuthenticating()
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

            Assert.IsNull(token);
            Assert.IsNull(client.Token);
            Assert.AreEqual(0, handler.RequestCount);
        }

        [TestMethod]
        public void Token_WhenExistingTokenHasNoExpiration_ReturnsNullAndClearsTokenWithoutAuthenticating()
        {
            var handler = new CapturingHttpMessageHandler();
            var client = CreateClient(handler);
            client.Token = new ProtectedToken
            {
                AccessToken = "missing-expiration-token",
                AccessSecret = "missing-expiration-secret"
            };

            var token = client.Token;

            Assert.IsNull(token);
            Assert.IsNull(client.Token);
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
            client.Token = new ProtectedToken
            {
                AccessToken = "existing-token",
                AccessSecret = "existing-secret",
                ExpirationDate = DateTime.Now.AddHours(1)
            };

            var response = await client.AuthenticateStaffUserAsync(CreateStaffUser());

            Assert.IsNotNull(response.Data);
            Assert.AreEqual("returned-token", response.Data.AccessToken);
            Assert.IsNotNull(client.Token);
            Assert.AreEqual("existing-token", client.Token.AccessToken);
            Assert.AreEqual(1, handler.RequestCount);
            Assert.IsNotNull(handler.LastRequest);
            Assert.AreEqual(HttpMethod.Post, handler.LastRequest.Method);
            StringAssert.Contains(handler.LastRequest.RequestUri!.AbsolutePath, "/protected/v1/1033/100/1/authenticator/staff");
            Assert.IsTrue(handler.LastRequest.Headers.Contains("PolarisDate"));
            Assert.IsTrue(handler.LastRequest.Headers.Contains("Authorization"));
            Assert.IsFalse(handler.LastRequest.Headers.Contains("X-PAPI-AccessToken"));
        }

        [TestMethod]
        public async Task AuthenticateStaffUserAsync_WithStaffOverrideAccountAndNoToken_DoesNotRecursivelyAcquireProtectedToken()
        {
            var handler = new ProtectedTokenHttpMessageHandler();
            var client = CreateProtectedClient(handler);

            var response = await client.AuthenticateStaffUserAsync(CreateStaffUser());

            Assert.IsNotNull(response.Data);
            Assert.AreEqual("protected-token", response.Data.AccessToken);
            Assert.IsNull(client.Token);
            Assert.AreEqual(1, handler.AuthenticationRequestCount);
            Assert.AreEqual(0, handler.ProtectedRequestCount);
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
        public async Task PatronSearchAsync_ConcurrentProtectedRequestsAcrossClientsForSameCacheKey_AuthenticateOnceAndUseSharedToken()
        {
            var hostname = $"https://example-{Guid.NewGuid():N}.test";
            var staff = CreateStaffUser(password: "shared-password");
            var handler = new ProtectedTokenHttpMessageHandler(
                HttpStatusCode.OK,
                CreateProtectedTokenJson("shared-token", "shared-secret", DateTime.Now.AddHours(1)));
            var clients = Enumerable.Range(0, 6)
                .Select(_ => CreateProtectedClient(handler, hostname, staff))
                .ToArray();

            await Task.WhenAll(clients.Select((client, index) => client.PatronSearchAsync($"name=shared-{index}")));

            Assert.AreEqual(1, handler.AuthenticationRequestCount);
            Assert.AreEqual(clients.Length, handler.ProtectedRequestCount);
            Assert.IsTrue(clients.All(client => client.Token?.AccessToken == "shared-token"));
            var finalRequests = handler.CapturedRequests.Where(request => !request.IsStaffAuthenticationRequest).ToArray();
            Assert.AreEqual(clients.Length, finalRequests.Length);
            foreach (var request in finalRequests)
            {
                StringAssert.Contains(request.Path, "/protected/v1/1033/100/1/shared-token/search/patrons/Boolean");
                Assert.IsFalse(request.Path.Contains(ProtectedToken.Placeholder, StringComparison.Ordinal));
                AssertAuthorizationHash(request, "shared-secret", "access-key", "access-id");
            }
        }

        [TestMethod]
        public async Task PatronSearchAsync_ConcurrentProtectedRequestsAcrossClientsForDifferentPasswords_AuthenticateAndCacheSeparately()
        {
            var hostname = $"https://example-{Guid.NewGuid():N}.test";
            var passwordAToken = new ProtectedToken
            {
                AccessToken = "token-for-password-a",
                AccessSecret = "secret-for-password-a",
                ExpirationDate = DateTime.Now.AddHours(1)
            };
            var passwordBToken = new ProtectedToken
            {
                AccessToken = "token-for-password-b",
                AccessSecret = "secret-for-password-b",
                ExpirationDate = DateTime.Now.AddHours(1)
            };
            var handler = new ProtectedTokenHttpMessageHandler(request =>
            {
                if (request.Body.Contains("password-a", StringComparison.Ordinal))
                {
                    return (HttpStatusCode.OK, CreateProtectedTokenJson(passwordAToken));
                }

                if (request.Body.Contains("password-b", StringComparison.Ordinal))
                {
                    return (HttpStatusCode.OK, CreateProtectedTokenJson(passwordBToken));
                }

                return (HttpStatusCode.BadRequest, "{\"PAPIErrorCode\":1}");
            });
            var clients = new[]
            {
                CreateProtectedClient(handler, hostname, CreateStaffUser(password: "password-a")),
                CreateProtectedClient(handler, hostname, CreateStaffUser(password: "password-b")),
                CreateProtectedClient(handler, hostname, CreateStaffUser(password: "password-a")),
                CreateProtectedClient(handler, hostname, CreateStaffUser(password: "password-b"))
            };

            await Task.WhenAll(
                clients[0].PatronSearchAsync("name=alpha-0"),
                clients[1].PatronSearchAsync("name=bravo-0"),
                clients[2].PatronSearchAsync("name=alpha-1"),
                clients[3].PatronSearchAsync("name=bravo-1"));

            Assert.AreEqual(2, handler.AuthenticationRequestCount);
            Assert.AreEqual(4, handler.ProtectedRequestCount);
            Assert.AreEqual("token-for-password-a", clients[0].Token?.AccessToken);
            Assert.AreEqual("token-for-password-b", clients[1].Token?.AccessToken);
            Assert.AreEqual("token-for-password-a", clients[2].Token?.AccessToken);
            Assert.AreEqual("token-for-password-b", clients[3].Token?.AccessToken);
            AssertFinalRequestsUseExpectedToken(handler, "alpha-", "token-for-password-a", "secret-for-password-a");
            AssertFinalRequestsUseExpectedToken(handler, "bravo-", "token-for-password-b", "secret-for-password-b");

            var passwordAReuseClient = CreateProtectedClient(handler, hostname, CreateStaffUser(password: "password-a"));
            var passwordBReuseClient = CreateProtectedClient(handler, hostname, CreateStaffUser(password: "password-b"));

            await Task.WhenAll(
                passwordAReuseClient.PatronSearchAsync("name=reuse-a"),
                passwordBReuseClient.PatronSearchAsync("name=reuse-b"));

            Assert.AreEqual(2, handler.AuthenticationRequestCount);
            Assert.AreEqual(6, handler.ProtectedRequestCount);
            Assert.AreEqual("token-for-password-a", passwordAReuseClient.Token?.AccessToken);
            Assert.AreEqual("token-for-password-b", passwordBReuseClient.Token?.AccessToken);
            AssertFinalRequestsUseExpectedToken(handler, "reuse-a", "token-for-password-a", "secret-for-password-a");
            AssertFinalRequestsUseExpectedToken(handler, "reuse-b", "token-for-password-b", "secret-for-password-b");
        }

        [TestMethod]
        public async Task PatronSearchAsync_WhenProtectedTokenCacheDisabled_IgnoresStaticCachedTokenAndUsesNewAuthenticationToken()
        {
            var handler = new ProtectedTokenHttpMessageHandler(
                HttpStatusCode.OK,
                CreateProtectedTokenJson("fresh-token", "fresh-secret", DateTime.Now.AddHours(1)));
            var client = CreateProtectedClient(handler);
            client.UseProtectedTokenCache = false;
            SetCachedToken(client.Hostname, client.AccessID, client.AccessKey, client.StaffOverrideAccount, new ProtectedToken
            {
                AccessToken = "cached-token",
                AccessSecret = "cached-secret",
                ExpirationDate = DateTime.Now.AddHours(1)
            });

            await client.PatronSearchAsync("name=Smith");

            Assert.AreEqual(1, handler.AuthenticationRequestCount);
            Assert.AreEqual(1, handler.ProtectedRequestCount);
            Assert.AreEqual("fresh-token", client.Token?.AccessToken);
            var finalRequest = handler.CapturedRequests.Single(request => !request.IsStaffAuthenticationRequest);
            StringAssert.Contains(finalRequest.Path, "/protected/v1/1033/100/1/fresh-token/search/patrons/Boolean");
            Assert.IsFalse(finalRequest.Path.Contains("cached-token", StringComparison.Ordinal));
            AssertAuthorizationHash(finalRequest, "fresh-secret", client.AccessKey, client.AccessID);
            AssertAuthorizationHashDoesNotMatch(finalRequest, "cached-secret", client.AccessKey, client.AccessID);
        }

        [TestMethod]
        public async Task PatronSearchAsync_WithExpiredInstanceToken_ReauthenticatesAndUsesNewTokenForRequestSigning()
        {
            var handler = new ProtectedTokenHttpMessageHandler(
                HttpStatusCode.OK,
                CreateProtectedTokenJson("new-protected-token", "new-protected-secret", DateTime.Now.AddHours(1)));
            var client = CreateProtectedClient(handler);
            client.Token = new ProtectedToken
            {
                AccessToken = "expired-token",
                AccessSecret = "expired-secret",
                ExpirationDate = DateTime.Now.AddMinutes(-1)
            };

            await client.PatronSearchAsync("name=Smith");

            Assert.AreEqual(1, handler.AuthenticationRequestCount);
            Assert.AreEqual(1, handler.ProtectedRequestCount);
            Assert.AreEqual("new-protected-token", client.Token?.AccessToken);
            var finalRequest = handler.CapturedRequests.Single(request => !request.IsStaffAuthenticationRequest);
            StringAssert.Contains(finalRequest.Path, "/protected/v1/1033/100/1/new-protected-token/search/patrons/Boolean");
            Assert.IsFalse(finalRequest.Path.Contains("expired-token", StringComparison.Ordinal));
            AssertAuthorizationHash(finalRequest, "new-protected-secret", client.AccessKey, client.AccessID);
            AssertAuthorizationHashDoesNotMatch(finalRequest, "expired-secret", client.AccessKey, client.AccessID);
        }

        [TestMethod]
        public async Task PatronSearchAsync_ProtectedTokenPlaceholder_ReplacesPlaceholderAfterSuccessfulTokenAcquisitionAndSignsFinalPath()
        {
            var handler = new ProtectedTokenHttpMessageHandler(
                HttpStatusCode.OK,
                CreateProtectedTokenJson("placeholder-token", "placeholder-secret", DateTime.Now.AddHours(1)));
            var client = CreateProtectedClient(handler);

            await client.PatronSearchAsync("name=Smith");

            var capturedRequests = handler.CapturedRequests;
            Assert.AreEqual(2, capturedRequests.Length);
            Assert.IsTrue(capturedRequests[0].IsStaffAuthenticationRequest);
            Assert.IsFalse(capturedRequests[1].IsStaffAuthenticationRequest);
            Assert.IsFalse(capturedRequests.Any(request => request.Path.Contains(ProtectedToken.Placeholder, StringComparison.Ordinal)));
            StringAssert.Contains(capturedRequests[1].Path, "/protected/v1/1033/100/1/placeholder-token/search/patrons/Boolean");
            AssertAuthorizationHash(capturedRequests[1], "placeholder-secret", client.AccessKey, client.AccessID);
            var placeholderUri = capturedRequests[1].AbsoluteUri.Replace("placeholder-token", ProtectedToken.Placeholder, StringComparison.Ordinal);
            AssertAuthorizationHashDoesNotMatch(capturedRequests[1], "placeholder-secret", client.AccessKey, client.AccessID, placeholderUri);
        }

        [TestMethod]
        public async Task PatronSearchAsync_WithCachedValidToken_AvoidsStaffAuthentication()
        {
            var handler = new ProtectedTokenHttpMessageHandler();
            var client = CreateProtectedClient(handler);
            SetCachedToken(client.Hostname, client.AccessID, client.AccessKey, client.StaffOverrideAccount, new ProtectedToken
            {
                AccessToken = "cached-token",
                AccessSecret = "cached-secret",
                ExpirationDate = DateTime.Now.AddHours(1)
            });

            await client.PatronSearchAsync("name=Smith");

            Assert.AreEqual(0, handler.AuthenticationRequestCount);
            Assert.AreEqual(1, handler.ProtectedRequestCount);
            Assert.AreEqual("cached-token", client.Token?.AccessToken);
        }

        [TestMethod]
        public async Task PatronSearchAsync_CachedTokenWithSameUserButBlankPassword_IsNotReusedOrWrittenToCache()
        {
            var hostname = $"https://example-{Guid.NewGuid():N}.test";
            var staffWithPassword = CreateStaffUser(password: "correct-1");

            var handlerA = new ProtectedTokenHttpMessageHandler(
                HttpStatusCode.OK,
                CreateProtectedTokenJson("token-a", "secret-a", DateTime.Now.AddHours(1)));
            var clientA = CreateProtectedClient(handlerA);
            clientA.Hostname = hostname;
            clientA.StaffOverrideAccount = staffWithPassword;

            await clientA.PatronSearchAsync("name=Smith");

            var handlerB = new ProtectedTokenHttpMessageHandler(
                HttpStatusCode.OK,
                CreateProtectedTokenJson("token-b", "secret-b", DateTime.Now.AddHours(1)));
            var clientB = CreateProtectedClient(handlerB);
            clientB.Hostname = hostname;
            clientB.StaffOverrideAccount = CreateStaffUser(password: string.Empty);

            await clientB.PatronSearchAsync("name=Jones");

            Assert.AreEqual(1, handlerA.AuthenticationRequestCount);
            Assert.AreEqual(1, handlerB.AuthenticationRequestCount);
            Assert.AreEqual("token-b", clientB.Token?.AccessToken);
            Assert.AreEqual(1, GetProtectedTokenCache().Count);
            Assert.IsFalse(TryGetCachedToken(hostname, clientB.AccessID, clientB.AccessKey, clientB.StaffOverrideAccount, out _));
        }

        [TestMethod]
        public async Task PatronSearchAsync_CachedTokenWithSameUserButDifferentPassword_IsNotReused()
        {
            var hostname = $"https://example-{Guid.NewGuid():N}.test";

            var handlerA = new ProtectedTokenHttpMessageHandler(
                HttpStatusCode.OK,
                CreateProtectedTokenJson("token-a", "secret-a", DateTime.Now.AddHours(1)));
            var clientA = CreateProtectedClient(handlerA);
            clientA.Hostname = hostname;
            clientA.StaffOverrideAccount = CreateStaffUser(password: "correct-1");

            await clientA.PatronSearchAsync("name=Smith");

            var handlerB = new ProtectedTokenHttpMessageHandler(
                HttpStatusCode.OK,
                CreateProtectedTokenJson("token-b", "secret-b", DateTime.Now.AddHours(1)));
            var clientB = CreateProtectedClient(handlerB);
            clientB.Hostname = hostname;
            clientB.StaffOverrideAccount = CreateStaffUser(password: "different-2");

            await clientB.PatronSearchAsync("name=Jones");

            Assert.AreEqual(1, handlerB.AuthenticationRequestCount);
            Assert.AreEqual("token-b", clientB.Token?.AccessToken);
            Assert.IsTrue(TryGetCachedToken(hostname, clientA.AccessID, clientA.AccessKey, clientA.StaffOverrideAccount, out var cachedTokenA));
            Assert.IsNotNull(cachedTokenA);
            Assert.AreEqual("token-a", cachedTokenA.AccessToken);
            Assert.IsTrue(TryGetCachedToken(hostname, clientB.AccessID, clientB.AccessKey, clientB.StaffOverrideAccount, out var cachedTokenB));
            Assert.IsNotNull(cachedTokenB);
            Assert.AreEqual("token-b", cachedTokenB.AccessToken);
        }

        [TestMethod]
        public async Task PatronSearchAsync_CachedTokenWithSameUserButDifferentAccessKey_IsNotReused()
        {
            var hostname = $"https://example-{Guid.NewGuid():N}.test";
            var staff = CreateStaffUser(password: "correct-1");

            var handlerA = new ProtectedTokenHttpMessageHandler(
                HttpStatusCode.OK,
                CreateProtectedTokenJson("token-a", "secret-a", DateTime.Now.AddHours(1)));
            var clientA = CreateProtectedClient(handlerA);
            clientA.Hostname = hostname;
            clientA.StaffOverrideAccount = staff;

            await clientA.PatronSearchAsync("name=Smith");

            var handlerB = new ProtectedTokenHttpMessageHandler(
                HttpStatusCode.OK,
                CreateProtectedTokenJson("token-b", "secret-b", DateTime.Now.AddHours(1)));
            var clientB = CreateProtectedClient(handlerB);
            clientB.Hostname = hostname;
            clientB.AccessKey = "different-access-key";
            clientB.StaffOverrideAccount = staff;

            await clientB.PatronSearchAsync("name=Jones");

            Assert.AreEqual(1, handlerB.AuthenticationRequestCount);
            Assert.AreEqual("token-b", clientB.Token?.AccessToken);
            Assert.IsTrue(TryGetCachedToken(hostname, clientA.AccessID, clientA.AccessKey, staff, out var cachedTokenA));
            Assert.IsNotNull(cachedTokenA);
            Assert.AreEqual("token-a", cachedTokenA.AccessToken);
            Assert.IsTrue(TryGetCachedToken(hostname, clientB.AccessID, clientB.AccessKey, staff, out var cachedTokenB));
            Assert.IsNotNull(cachedTokenB);
            Assert.AreEqual("token-b", cachedTokenB.AccessToken);
        }

        [TestMethod]
        public async Task PatronSearchAsync_ExpiredCachedToken_IsRemovedWhenEncountered()
        {
            var handler = new ProtectedTokenHttpMessageHandler(HttpStatusCode.InternalServerError, "{\"PAPIErrorCode\":1}");
            var client = CreateProtectedClient(handler);
            SetCachedToken(client.Hostname, client.AccessID, client.AccessKey, client.StaffOverrideAccount, new ProtectedToken
            {
                AccessToken = "expired-token",
                AccessSecret = "expired-secret",
                ExpirationDate = DateTime.Now.AddMinutes(-1)
            });

            await Assert.ThrowsExceptionAsync<InvalidOperationException>(() => client.PatronSearchAsync("name=Smith"));

            Assert.AreEqual(1, handler.AuthenticationRequestCount);
            Assert.IsFalse(TryGetCachedToken(client.Hostname, client.AccessID, client.AccessKey, client.StaffOverrideAccount, out _));
        }

        [TestMethod]
        public async Task PatronSearchAsync_CachedTokenWithBlankAccessToken_IsRemovedAndNewTokenIsUsed()
        {
            var handler = new ProtectedTokenHttpMessageHandler(
                HttpStatusCode.OK,
                CreateProtectedTokenJson("new-token", "new-secret", DateTime.Now.AddHours(1)));
            var client = CreateProtectedClient(handler);
            SetCachedToken(client.Hostname, client.AccessID, client.AccessKey, client.StaffOverrideAccount, new ProtectedToken
            {
                AccessToken = " ",
                AccessSecret = "cached-secret",
                ExpirationDate = DateTime.Now.AddHours(1)
            });

            await client.PatronSearchAsync("name=Smith");

            Assert.AreEqual(1, handler.AuthenticationRequestCount);
            Assert.AreEqual(1, handler.ProtectedRequestCount);
            Assert.AreEqual("new-token", client.Token?.AccessToken);
            Assert.AreNotEqual(" ", client.Token?.AccessToken);
            Assert.IsTrue(TryGetCachedToken(client.Hostname, client.AccessID, client.AccessKey, client.StaffOverrideAccount, out var cachedToken));
            Assert.IsNotNull(cachedToken);
            Assert.AreEqual("new-token", cachedToken.AccessToken);
            Assert.AreEqual("new-secret", cachedToken.AccessSecret);
            StringAssert.Contains(handler.RequestPaths.Last(), "/protected/v1/1033/100/1/new-token/search/patrons/Boolean");
        }

        [TestMethod]
        public async Task PatronSearchAsync_CachedTokenWithBlankAccessSecret_IsRemovedAndNotUsedWhenAuthenticationFails()
        {
            var handler = new ProtectedTokenHttpMessageHandler(HttpStatusCode.InternalServerError, "{\"PAPIErrorCode\":1}");
            var client = CreateProtectedClient(handler);
            SetCachedToken(client.Hostname, client.AccessID, client.AccessKey, client.StaffOverrideAccount, new ProtectedToken
            {
                AccessToken = "cached-token",
                AccessSecret = " ",
                ExpirationDate = DateTime.Now.AddHours(1)
            });

            await Assert.ThrowsExceptionAsync<InvalidOperationException>(() => client.PatronSearchAsync("name=Smith"));

            Assert.AreEqual(1, handler.AuthenticationRequestCount);
            Assert.AreEqual(0, handler.ProtectedRequestCount);
            Assert.IsFalse(TryGetCachedToken(client.Hostname, client.AccessID, client.AccessKey, client.StaffOverrideAccount, out _));
            Assert.IsNull(client.Token);
        }

        [TestMethod]
        public void BuildProtectedTokenCacheKey_UsesCredentialFingerprintWithoutRawSecrets()
        {
            var client = CreateProtectedClient(new ProtectedTokenHttpMessageHandler());
            client.AccessKey = "raw-access-key-secret";
            client.StaffOverrideAccount = CreateStaffUser(password: "correct-1");

            var cacheKey = BuildCacheKey(client);

            Assert.IsNotNull(cacheKey);
            Assert.IsFalse(cacheKey!.Contains(client.StaffOverrideAccount.Password, StringComparison.Ordinal), "Cache key exposed secret material.");
            Assert.IsFalse(cacheKey.Contains(client.AccessKey, StringComparison.Ordinal), "Cache key exposed secret material.");
            StringAssert.Contains(cacheKey, client.Hostname.Trim());
            StringAssert.Contains(cacheKey, client.AccessID.Trim());
            StringAssert.Contains(cacheKey, client.StaffOverrideAccount.Domain.Trim());
            StringAssert.Contains(cacheKey, client.StaffOverrideAccount.Username.Trim());
        }

        [TestMethod]
        public async Task PatronSearchAsync_SameHostAndStaffWithDifferentAccessIds_AuthenticatesAndCachesSeparateTokens()
        {
            var hostname = $"https://example-{Guid.NewGuid():N}.test";
            var staff = CreateStaffUser();

            var handlerA = new ProtectedTokenHttpMessageHandler(
                HttpStatusCode.OK,
                CreateProtectedTokenJson("token-a", "secret-a", DateTime.Now.AddHours(1)));
            var clientA = CreateProtectedClient(handlerA);
            clientA.Hostname = hostname;
            clientA.AccessID = "access-a";
            clientA.StaffOverrideAccount = staff;

            await clientA.PatronSearchAsync("name=Smith");

            Assert.AreEqual(1, handlerA.AuthenticationRequestCount);
            Assert.AreEqual(1, handlerA.ProtectedRequestCount);
            Assert.AreEqual("token-a", clientA.Token?.AccessToken);
            Assert.IsTrue(TryGetCachedToken(hostname, "access-a", clientA.AccessKey, staff, out var cachedTokenA));
            Assert.IsNotNull(cachedTokenA);
            Assert.AreEqual("token-a", cachedTokenA.AccessToken);

            var handlerB = new ProtectedTokenHttpMessageHandler(
                HttpStatusCode.OK,
                CreateProtectedTokenJson("token-b", "secret-b", DateTime.Now.AddHours(1)));
            var clientB = CreateProtectedClient(handlerB);
            clientB.Hostname = hostname;
            clientB.AccessID = "access-b";
            clientB.StaffOverrideAccount = staff;

            await clientB.PatronSearchAsync("name=Jones");

            Assert.AreEqual(1, handlerB.AuthenticationRequestCount);
            Assert.AreEqual(1, handlerB.ProtectedRequestCount);
            Assert.AreEqual("token-b", clientB.Token?.AccessToken);
            Assert.IsTrue(TryGetCachedToken(hostname, "access-b", clientB.AccessKey, staff, out var cachedTokenB));
            Assert.IsNotNull(cachedTokenB);
            Assert.AreEqual("token-b", cachedTokenB.AccessToken);
            Assert.AreNotEqual(cachedTokenA.AccessToken, cachedTokenB.AccessToken);
            Assert.IsTrue(TryGetCachedToken(hostname, "access-a", clientA.AccessKey, staff, out var cachedTokenAAfterClientB));
            Assert.IsNotNull(cachedTokenAAfterClientB);
            Assert.AreEqual("token-a", cachedTokenAAfterClientB.AccessToken);
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
            var paths = handler.RequestPaths;
            Assert.AreEqual(2, paths.Length);
            StringAssert.Contains(paths[0], "/protected/v1/1033/100/1/authenticator/staff");
            StringAssert.Contains(paths[1], "/protected/v1/1033/100/1/protected-token/search/patrons/Boolean");
            Assert.IsTrue(TryGetCachedToken(client.Hostname, client.AccessID, client.AccessKey, client.StaffOverrideAccount, out var cachedToken));
            Assert.IsNotNull(cachedToken);
            Assert.AreEqual("protected-token", cachedToken.AccessToken);
            Assert.AreNotSame(client.Token, cachedToken);
        }

        [TestMethod]
        public async Task PatronAccountGetAsync_FailedStaffAuthentication_DoesNotPopulateProtectedTokenCache()
        {
            var handler = new ProtectedTokenHttpMessageHandler(HttpStatusCode.InternalServerError, "{\"PAPIErrorCode\":1}");
            var client = CreateProtectedClient(handler);

            await Assert.ThrowsExceptionAsync<InvalidOperationException>(() => client.PatronAccountGetAsync("ABC123"));

            Assert.AreEqual(1, handler.AuthenticationRequestCount);
            Assert.AreEqual(0, handler.ProtectedRequestCount);
            Assert.IsNull(client.Token);
            Assert.IsFalse(TryGetCachedToken(client.Hostname, client.AccessID, client.AccessKey, client.StaffOverrideAccount, out _));
        }

        [TestMethod]
        public async Task PatronAccountGetAsync_NullDataStaffAuthentication_DoesNotPopulateProtectedTokenCache()
        {
            var handler = new ProtectedTokenHttpMessageHandler(HttpStatusCode.OK, "{}");
            var client = CreateProtectedClient(handler);

            await Assert.ThrowsExceptionAsync<InvalidOperationException>(() => client.PatronAccountGetAsync("ABC123"));

            Assert.AreEqual(1, handler.AuthenticationRequestCount);
            Assert.AreEqual(0, handler.ProtectedRequestCount);
            Assert.IsNull(client.Token);
            Assert.IsFalse(TryGetCachedToken(client.Hostname, client.AccessID, client.AccessKey, client.StaffOverrideAccount, out _));
        }

        [TestMethod]
        public async Task PatronAccountGetAsync_FailedStaffAuthentication_AfterExpiredExistingTokenClearsToken()
        {
            var handler = new ProtectedTokenHttpMessageHandler(HttpStatusCode.InternalServerError, "{\"PAPIErrorCode\":1}");
            var client = CreateProtectedClient(handler);
            client.Token = new ProtectedToken
            {
                AccessToken = "existing-token",
                AccessSecret = "existing-secret",
                ExpirationDate = DateTime.Now.AddHours(-1)
            };

            await Assert.ThrowsExceptionAsync<InvalidOperationException>(() => client.PatronAccountGetAsync("ABC123"));

            Assert.AreEqual(1, handler.AuthenticationRequestCount);
            Assert.AreEqual(0, handler.ProtectedRequestCount);
            Assert.IsNull(client.Token);
            Assert.IsFalse(TryGetCachedToken(client.Hostname, client.AccessID, client.AccessKey, client.StaffOverrideAccount, out _));
        }

        [TestMethod]
        public async Task PatronAccountGetAsync_NullDataStaffAuthentication_AfterExpiredExistingTokenClearsToken()
        {
            var handler = new ProtectedTokenHttpMessageHandler(HttpStatusCode.OK, "{}");
            var client = CreateProtectedClient(handler);
            client.Token = new ProtectedToken
            {
                AccessToken = "existing-token",
                AccessSecret = "existing-secret",
                ExpirationDate = DateTime.Now.AddHours(-1)
            };

            await Assert.ThrowsExceptionAsync<InvalidOperationException>(() => client.PatronAccountGetAsync("ABC123"));

            Assert.AreEqual(1, handler.AuthenticationRequestCount);
            Assert.AreEqual(0, handler.ProtectedRequestCount);
            Assert.IsNull(client.Token);
            Assert.IsFalse(TryGetCachedToken(client.Hostname, client.AccessID, client.AccessKey, client.StaffOverrideAccount, out _));
        }


        [TestMethod]
        public async Task PatronAccountGetAsync_InvalidStaffAuthentication_AfterExpiredExistingTokenClearsTokenAndDoesNotCache()
        {
            var handler = new ProtectedTokenHttpMessageHandler(
                HttpStatusCode.OK,
                CreateProtectedTokenJson(string.Empty, string.Empty, DateTime.Now.AddHours(1)));
            var client = CreateProtectedClient(handler);
            client.Token = new ProtectedToken
            {
                AccessToken = "existing-token",
                AccessSecret = "existing-secret",
                ExpirationDate = DateTime.Now.AddHours(-1)
            };

            await Assert.ThrowsExceptionAsync<InvalidOperationException>(() => client.PatronAccountGetAsync("ABC123"));

            Assert.AreEqual(1, handler.AuthenticationRequestCount);
            Assert.AreEqual(0, handler.ProtectedRequestCount);
            Assert.IsNull(client.Token);
            Assert.IsFalse(TryGetCachedToken(client.Hostname, client.AccessID, client.AccessKey, client.StaffOverrideAccount, out _));
        }

        [TestMethod]
        public async Task PatronSearchAsync_FailedStaffAuthentication_ThrowsBeforeFinalProtectedRequest()
        {
            var handler = new ProtectedTokenHttpMessageHandler(HttpStatusCode.InternalServerError, "{\"PAPIErrorCode\":1}");
            var client = CreateProtectedClient(handler);

            var exception = await Assert.ThrowsExceptionAsync<InvalidOperationException>(() => client.PatronSearchAsync("name=Smith"));

            StringAssert.Contains(exception.Message, "Staff authentication did not succeed");
            Assert.AreEqual(1, handler.AuthenticationRequestCount);
            Assert.AreEqual(0, handler.ProtectedRequestCount);
        }

        [TestMethod]
        public async Task PatronSearchAsync_NullDataStaffAuthentication_ThrowsBeforeFinalProtectedRequest()
        {
            var handler = new ProtectedTokenHttpMessageHandler(HttpStatusCode.OK, "null");
            var client = CreateProtectedClient(handler);

            var exception = await Assert.ThrowsExceptionAsync<InvalidOperationException>(() => client.PatronSearchAsync("name=Smith"));

            StringAssert.Contains(exception.Message, "did not return a usable protected access token");
            Assert.AreEqual(1, handler.AuthenticationRequestCount);
            Assert.AreEqual(0, handler.ProtectedRequestCount);
        }

        [TestMethod]
        public async Task PatronSearchAsync_BlankAccessTokenStaffAuthentication_ThrowsBeforeFinalProtectedRequest()
        {
            var handler = new ProtectedTokenHttpMessageHandler(
                HttpStatusCode.OK,
                CreateProtectedTokenJson(" ", "protected-secret", DateTime.Now.AddHours(1)));
            var client = CreateProtectedClient(handler);

            await Assert.ThrowsExceptionAsync<InvalidOperationException>(() => client.PatronSearchAsync("name=Smith"));

            Assert.AreEqual(1, handler.AuthenticationRequestCount);
            Assert.AreEqual(0, handler.ProtectedRequestCount);
        }

        [TestMethod]
        public async Task PatronSearchAsync_BlankAccessSecretStaffAuthentication_ThrowsBeforeFinalProtectedRequest()
        {
            var handler = new ProtectedTokenHttpMessageHandler(
                HttpStatusCode.OK,
                CreateProtectedTokenJson("protected-token", " ", DateTime.Now.AddHours(1)));
            var client = CreateProtectedClient(handler);

            await Assert.ThrowsExceptionAsync<InvalidOperationException>(() => client.PatronSearchAsync("name=Smith"));

            Assert.AreEqual(1, handler.AuthenticationRequestCount);
            Assert.AreEqual(0, handler.ProtectedRequestCount);
        }

        [TestMethod]
        public async Task PatronSearchAsync_ExpiredTokenStaffAuthentication_ThrowsBeforeFinalProtectedRequest()
        {
            var handler = new ProtectedTokenHttpMessageHandler(
                HttpStatusCode.OK,
                CreateProtectedTokenJson("protected-token", "protected-secret", DateTime.Now.AddMinutes(-1)));
            var client = CreateProtectedClient(handler);

            await Assert.ThrowsExceptionAsync<InvalidOperationException>(() => client.PatronSearchAsync("name=Smith"));

            Assert.AreEqual(1, handler.AuthenticationRequestCount);
            Assert.AreEqual(0, handler.ProtectedRequestCount);
        }

        [TestMethod]
        public async Task PatronSearchAsync_ProtectedTokenPlaceholder_ThrowsBeforeFinalRequestWhenTokenAcquisitionFails()
        {
            var returnedToken = new ProtectedToken
            {
                AccessToken = "failed-token",
                AccessSecret = "failed-secret",
                ExpirationDate = DateTime.Now.AddHours(1)
            };
            var handler = new ProtectedTokenHttpMessageHandler(HttpStatusCode.Unauthorized, CreateProtectedTokenJson(returnedToken));
            var client = CreateProtectedClient(handler);
            client.AccessKey = "failure-access-key";
            client.StaffOverrideAccount = CreateStaffUser(password: "failure-password");

            var exception = await Assert.ThrowsExceptionAsync<InvalidOperationException>(() => client.PatronSearchAsync("name=Smith"));

            Assert.IsFalse(exception.Message.Contains(client.StaffOverrideAccount.Password, StringComparison.Ordinal));
            Assert.IsFalse(exception.Message.Contains(client.AccessKey, StringComparison.Ordinal));
            Assert.IsFalse(exception.Message.Contains(returnedToken.AccessToken, StringComparison.Ordinal));
            Assert.IsFalse(exception.Message.Contains(returnedToken.AccessSecret, StringComparison.Ordinal));
            Assert.AreEqual(1, handler.AuthenticationRequestCount);
            Assert.AreEqual(0, handler.ProtectedRequestCount);
            Assert.IsFalse(handler.RequestPaths.Any(path => path.Contains(ProtectedToken.Placeholder, StringComparison.Ordinal)));
        }

        [TestMethod]
        public async Task PatronAccountGetAsync_PublicStaffOverride_FailedStaffAuthentication_ThrowsBeforeFinalRequest()
        {
            var handler = new ProtectedTokenHttpMessageHandler(HttpStatusCode.InternalServerError, "{\"PAPIErrorCode\":1}");
            var client = CreateProtectedClient(handler);

            var exception = await Assert.ThrowsExceptionAsync<InvalidOperationException>(() => client.PatronAccountGetAsync("ABC123"));

            StringAssert.Contains(exception.Message, "Staff authentication did not succeed");
            Assert.AreEqual(1, handler.AuthenticationRequestCount);
            Assert.AreEqual(0, handler.ProtectedRequestCount);
        }

        [TestMethod]
        public async Task PatronAccountGetAsync_PublicRequestWithoutStaffOverrideAccount_ContinuesAsOrdinaryPublicRequest()
        {
            var handler = new ProtectedTokenHttpMessageHandler(HttpStatusCode.InternalServerError, "{\"PAPIErrorCode\":1}");
            var client = CreateProtectedClient(handler);
            client.StaffOverrideAccount = null;
            client.AllowStaffOverrideRequests = true;

            var response = await client.PatronAccountGetAsync("ABC123");

            Assert.IsNotNull(response);
            Assert.AreEqual(0, handler.AuthenticationRequestCount);
            Assert.AreEqual(1, handler.ProtectedRequestCount);
            StringAssert.Contains(handler.RequestPaths.Single(), "/public/v1/1033/100/1/patron/ABC123/account/outstanding");
        }

        [TestMethod]
        public async Task PatronAccountGetAsync_PublicRequestWithExplicitPassword_DoesNotRequireStaffOverrideToken()
        {
            var handler = new ProtectedTokenHttpMessageHandler(HttpStatusCode.InternalServerError, "{\"PAPIErrorCode\":1}");
            var client = CreateProtectedClient(handler);

            var response = await client.PatronAccountGetAsync("ABC123", password: "patron-password");

            Assert.IsNotNull(response);
            Assert.AreEqual(0, handler.AuthenticationRequestCount);
            Assert.AreEqual(1, handler.ProtectedRequestCount);
            StringAssert.Contains(handler.RequestPaths.Single(), "/public/v1/1033/100/1/patron/ABC123/account/outstanding");
        }

        [TestMethod]
        public async Task PatronSearchAsync_CancellationDuringProtectedTokenAuthentication_PropagatesCancellation()
        {
            var handler = new ProtectedTokenHttpMessageHandler();
            var client = CreateProtectedClient(handler);
            using var cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromMilliseconds(10));

            try
            {
                await client.PatronSearchAsync("name=Smith", cancellationToken: cancellationTokenSource.Token);
                Assert.Fail("Expected cancellation to propagate.");
            }
            catch (OperationCanceledException)
            {
            }

            Assert.AreEqual(1, handler.AuthenticationRequestCount);
            Assert.AreEqual(0, handler.ProtectedRequestCount);
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
                ExpirationDate = DateTime.Now.AddHours(-1)
            };
            var request = new PapiRestRequest(HttpMethod.Get, "/protected/v1/1033/100/1/expired-token/search/patrons/Boolean");

            var formatted = (PapiRestRequest)client.PreformatRestRequest(request);

            var date = formatted.Headers["PolarisDate"].ToString();
            var uri = client.BuildRequestUri(formatted).AbsoluteUri;
            Assert.AreEqual($"PWS access-id:{ComputePapiHash("GET", uri, date, string.Empty, "access-key")}", formatted.Headers["Authorization"]);
            Assert.AreNotEqual($"PWS access-id:{ComputePapiHash("GET", uri, date, "expired-secret", "access-key")}", formatted.Headers["Authorization"]);
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
                ExpirationDate = DateTime.Now.AddHours(-1)
            };
            var request = new PapiRestRequest(HttpMethod.Get, "/public/v1/1033/100/1/patron/ABC");
            request.Headers["X-PAPI-AccessToken"] = "stale-token";

            var formatted = (PapiRestRequest)client.PreformatRestRequest(request);

            var date = formatted.Headers["PolarisDate"].ToString();
            var uri = client.BuildRequestUri(formatted).AbsoluteUri;
            var authorization = formatted.Headers["Authorization"];
            Assert.AreEqual($"PWS access-id:{ComputePapiHash("GET", uri, date, string.Empty, "access-key")}", authorization);
            Assert.AreNotEqual($"PWS access-id:{ComputePapiHash("GET", uri, date, "expired-secret", "access-key")}", authorization);
            Assert.IsFalse(formatted.Headers.ContainsKey("X-PAPI-AccessToken"));
            Assert.IsNull(client.Token);
        }

        [TestMethod]
        public void PreformatRestRequest_PublicStaffOverrideWithMissingTokenValues_DoesNotAddAccessTokenHeader()
        {
            var handler = new CapturingHttpMessageHandler();
            var client = CreateClient(handler);
            client.AllowStaffOverrideRequests = true;
            client.Token = new ProtectedToken
            {
                AccessToken = " ",
                AccessSecret = "manual-secret",
                ExpirationDate = DateTime.Now.AddHours(1)
            };

            var formatted = (PapiRestRequest)client.PreformatRestRequest(new PapiRestRequest(HttpMethod.Get, "/public/v1/1033/100/1/patron/ABC"));

            Assert.IsFalse(formatted.Headers.ContainsKey("X-PAPI-AccessToken"));
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
            return CreateProtectedClient(handler, $"https://example-{Guid.NewGuid():N}.test", CreateStaffUser());
        }

        private static PapiClient CreateProtectedClient(ProtectedTokenHttpMessageHandler handler, string hostname, PolarisUser staffUser)
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

        private static PolarisUser CreateStaffUser(string password = "password")
        {
            return new PolarisUser
            {
                Domain = "domain",
                Username = "user",
                Password = password
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

        private static string CreateProtectedTokenJson(ProtectedToken token)
        {
            return CreateProtectedTokenJson(token.AccessToken!, token.AccessSecret!, token.ExpirationDate!.Value);
        }

        private static void AssertFinalRequestsUseExpectedToken(ProtectedTokenHttpMessageHandler handler, string queryMarker, string expectedToken, string expectedSecret)
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

        private static void AssertAuthorizationHash(CapturedPapiRequest request, string secret, string accessKey, string accessId, string? absoluteUri = null)
        {
            Assert.IsTrue(request.Headers.TryGetValue("PolarisDate", out var date), "The request did not include a PolarisDate header.");
            Assert.IsTrue(request.Headers.TryGetValue("Authorization", out var authorization), "The request did not include an Authorization header.");
            Assert.AreEqual($"PWS {accessId}:{ComputePapiHash(request.Method, absoluteUri ?? request.AbsoluteUri, date, secret, accessKey)}", authorization);
        }

        private static void AssertAuthorizationHashDoesNotMatch(CapturedPapiRequest request, string secret, string accessKey, string accessId, string? absoluteUri = null)
        {
            Assert.IsTrue(request.Headers.TryGetValue("PolarisDate", out var date), "The request did not include a PolarisDate header.");
            Assert.IsTrue(request.Headers.TryGetValue("Authorization", out var authorization), "The request did not include an Authorization header.");
            Assert.AreNotEqual($"PWS {accessId}:{ComputePapiHash(request.Method, absoluteUri ?? request.AbsoluteUri, date, secret, accessKey)}", authorization);
        }

        private static string ComputePapiHash(string httpMethod, string uri, string date, string password, string accessKey)
        {
            var hashString = httpMethod + uri + date + password;
            var computedHash = HMACSHA1.HashData(Encoding.UTF8.GetBytes(accessKey), Encoding.UTF8.GetBytes(hashString));
            return Convert.ToBase64String(computedHash);
        }

        private sealed class CapturingHttpMessageHandler : HttpMessageHandler
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

        private sealed class CapturedPapiRequest
        {
            public CapturedPapiRequest(HttpRequestMessage request, string body)
            {
                Method = request.Method.ToString();
                AbsoluteUri = request.RequestUri!.AbsoluteUri;
                Path = request.RequestUri.AbsolutePath;
                Body = body;
                IsStaffAuthenticationRequest = Path.Contains("/authenticator/staff", StringComparison.Ordinal);
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

        private sealed class ProtectedTokenHttpMessageHandler : HttpMessageHandler
        {
            private readonly HttpStatusCode _authenticationStatusCode;
            private readonly string _authenticationResponseJson;
            private readonly Func<CapturedPapiRequest, (HttpStatusCode StatusCode, string ResponseJson)>? _authenticationResponseFactory;
            private int _authenticationRequestCount;
            private int _protectedRequestCount;
            private readonly ConcurrentQueue<string> _requestPaths = new ConcurrentQueue<string>();
            private readonly ConcurrentQueue<CapturedPapiRequest> _capturedRequests = new ConcurrentQueue<CapturedPapiRequest>();

            public int AuthenticationRequestCount => _authenticationRequestCount;
            public int ProtectedRequestCount => _protectedRequestCount;
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

        private static void SetCachedToken(string hostname, string accessId, string accessKey, PolarisUser? staffUser, ProtectedToken token)
        {
            var cacheKey = BuildCacheKey(hostname, accessId, accessKey, staffUser);
            if (cacheKey != null)
            {
                GetProtectedTokenCache().TryAdd(cacheKey, token);
            }
        }

        private static bool TryGetCachedToken(string hostname, string accessId, string accessKey, PolarisUser? staffUser, out ProtectedToken? token)
        {
            token = null;
            var cacheKey = BuildCacheKey(hostname, accessId, accessKey, staffUser);
            return cacheKey != null && GetProtectedTokenCache().TryGetValue(cacheKey, out token);
        }

        private static ConcurrentDictionary<string, ProtectedToken> GetProtectedTokenCache()
        {
            return GetPrivateStaticProperty<ConcurrentDictionary<string, ProtectedToken>>("ProtectedTokenCache")
                ?? throw new InvalidOperationException("Protected token cache was not available.");
        }

        private static T? GetPrivateStaticProperty<T>(string propertyName) where T : class
        {
            var cacheProperty = typeof(PapiClient).GetProperty(propertyName, BindingFlags.NonPublic | BindingFlags.Static);
            return cacheProperty?.GetValue(null) as T;
        }

        private static string? BuildCacheKey(string hostname, string accessId, string accessKey, PolarisUser? staffUser)
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

        private static string? BuildCacheKey(PapiClient client)
        {
            var buildCacheKeyMethod = typeof(PapiClient).GetMethod("BuildProtectedTokenCacheKey", BindingFlags.Instance | BindingFlags.NonPublic)
                ?? throw new InvalidOperationException("Protected token cache-key builder was not available.");
            return buildCacheKeyMethod.Invoke(client, Array.Empty<object>()) as string;
        }
    }
}
