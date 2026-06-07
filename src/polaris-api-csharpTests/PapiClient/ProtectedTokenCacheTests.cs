using Clc.Polaris.Api;
using Clc.Polaris.Api.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace Clc.Polaris.Api.Tests
{
    [TestClass]
    [UnitCategory]
    [DoNotParallelize]
    public class ProtectedTokenCacheTests : PapiClientTestBase
    {
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
            Assert.AreEqual(8, handler.NonAuthenticationRequestCount);
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
            Assert.AreEqual(clients.Length, handler.NonAuthenticationRequestCount);
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
            Assert.AreEqual(4, handler.NonAuthenticationRequestCount);
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
            Assert.AreEqual(6, handler.NonAuthenticationRequestCount);
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
            Assert.AreEqual(1, handler.NonAuthenticationRequestCount);
            Assert.AreEqual("fresh-token", client.Token?.AccessToken);
            var finalRequest = handler.CapturedRequests.Single(request => !request.IsStaffAuthenticationRequest);
            StringAssert.Contains(finalRequest.Path, "/protected/v1/1033/100/1/fresh-token/search/patrons/Boolean");
            Assert.IsFalse(finalRequest.Path.Contains("cached-token", StringComparison.Ordinal));
            AssertAuthorizationHash(finalRequest, "fresh-secret", client.AccessKey, client.AccessID);
            AssertAuthorizationHashDoesNotMatch(finalRequest, "cached-secret", client.AccessKey, client.AccessID);
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
            Assert.AreEqual(1, handler.NonAuthenticationRequestCount);
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
            Assert.AreEqual(1, handler.NonAuthenticationRequestCount);
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
            Assert.AreEqual(0, handler.NonAuthenticationRequestCount);
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
            Assert.AreEqual(1, handlerA.NonAuthenticationRequestCount);
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
            Assert.AreEqual(1, handlerB.NonAuthenticationRequestCount);
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
            Assert.AreEqual(1, handler.NonAuthenticationRequestCount);
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
    }
}
