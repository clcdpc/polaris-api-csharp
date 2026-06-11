using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Clc.Polaris.Api;
using Clc.Polaris.Api.Models;
using Clc.Polaris.Api.Tests;
using Clc.Polaris.Api.Tests.TestInfrastructure;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Clc.Polaris.Api.Tests.Methods.ProtectedTokens
{
    [TestClass]
    [DoNotParallelize]
    [UnitTest]
    public class PatronSearchTests : PapiClientTestBase
    {
        [TestInitialize]
        public void TestInitialize()
        {
            ClearProtectedTokenState();
        }

        [TestMethod]
        public async Task ProtectedTokenPathRequest_ReplacesPlaceholderBeforeSending()
        {
            var handler = new CapturingHttpMessageHandler("{\"PAPIErrorCode\":0}");
            var client = CreateClient(handler);
            client.Token = new ProtectedToken { AccessToken = "real-token", AccessSecret = "real-secret", ExpirationDate = DateTime.Now.AddHours(1) };

            var response = await client.PatronSearchAsync("name=Smith", orgId: 9);

            Assert.IsNotNull(response);
            Assert.IsNotNull(handler.LastRequest);
            Assert.IsFalse(handler.LastRequest!.RequestUri!.AbsoluteUri.Contains(ProtectedToken.Placeholder, StringComparison.Ordinal));
            StringAssert.Contains(handler.LastRequest.RequestUri.AbsolutePath, "/protected/v1/1033/100/9/real-token/search/patrons/Boolean");
        }

        [TestMethod]
        public async Task ProtectedTokenPathRequest_HashesReplacedUrlInsteadOfPlaceholderUrl()
        {
            var handler = new CapturingHttpMessageHandler("{\"PAPIErrorCode\":0}");
            var client = CreateClient(handler);
            client.Token = new ProtectedToken { AccessToken = "hash-token", AccessSecret = "hash-secret", ExpirationDate = DateTime.Now.AddHours(1) };

            await client.PatronSearchAsync("name=Smith", orgId: 9);

            Assert.IsNotNull(handler.LastRequest);
            var date = handler.LastRequest!.Headers.GetValues("PolarisDate").Single();
            var expectedUri = "https://example.test/PAPIService/REST/protected/v1/1033/100/9/hash-token/search/patrons/Boolean?q=name%3DSmith&patronsperpage=10&page=1&sort=PATN";
            var expectedHash = PapiSignature.ComputeHash("access-key", "GET", expectedUri, date, "hash-secret");
            var placeholderUri = expectedUri.Replace("hash-token", ProtectedToken.Placeholder, StringComparison.Ordinal);
            var placeholderHash = PapiSignature.ComputeHash("access-key", "GET", placeholderUri, date, "hash-secret");
            Assert.AreEqual($"PWS access-id:{expectedHash}", handler.LastRequest.Headers.GetValues("Authorization").Single());
            Assert.AreNotEqual($"PWS access-id:{placeholderHash}", handler.LastRequest.Headers.GetValues("Authorization").Single());
        }

        [TestMethod]
        public async Task ProtectedTokenPathRequest_AcquiresProtectedTokenAutomaticallyWhenMissing()
        {
            var handler = new ProtectedTokenCancellationHttpMessageHandler();
            var client = CreateClient(handler);
            client.AllowStaffOverrideRequests = true;
            client.StaffOverrideAccount = new PolarisUser
            {
                Domain = "main",
                Username = "auto-placeholder",
                Password = "secret"
            };

            var response = await client.PatronSearchAsync("name=Smith", orgId: 9);

            Assert.IsNotNull(response);
            Assert.AreEqual(2, handler.Requests.Count);
            StringAssert.Contains(handler.Requests[0].RequestUri!.AbsolutePath, "/protected/v1/1033/100/1/authenticator/staff");
            StringAssert.Contains(handler.Requests[1].RequestUri!.AbsolutePath, "/protected/v1/1033/100/9/protected-token/search/patrons/Boolean");
            Assert.IsFalse(handler.Requests[1].RequestUri!.AbsoluteUri.Contains(ProtectedToken.Placeholder, StringComparison.Ordinal));
        }

        [TestMethod]
        public async Task ProtectedTokenPathRequest_WithExpiredToken_AcquiresProtectedTokenBeforePlaceholderReplacement()
        {
            var handler = new ProtectedTokenCancellationHttpMessageHandler();
            var client = CreateClient(handler);
            client.AllowStaffOverrideRequests = true;
            client.StaffOverrideAccount = new PolarisUser
            {
                Domain = "main",
                Username = $"expired-placeholder-{Guid.NewGuid():N}",
                Password = "secret"
            };
            client.Token = new ProtectedToken
            {
                AccessToken = "expired-token",
                AccessSecret = "expired-secret",
                ExpirationDate = DateTime.Now.AddHours(-1)
            };

            var response = await client.PatronSearchAsync("name=Smith", orgId: 9);

            Assert.IsNotNull(response);
            Assert.AreEqual(2, handler.Requests.Count);
            StringAssert.Contains(handler.Requests[0].RequestUri!.AbsolutePath, "/protected/v1/1033/100/1/authenticator/staff");
            StringAssert.Contains(handler.Requests[1].RequestUri!.AbsolutePath, "/protected/v1/1033/100/9/protected-token/search/patrons/Boolean");
            Assert.IsFalse(handler.Requests[1].RequestUri!.AbsoluteUri.Contains(ProtectedToken.Placeholder, StringComparison.Ordinal));
            Assert.AreEqual("protected-token", client.Token!.AccessToken);
        }

        [TestMethod]
        public async Task ProtectedTokenPathRequest_WhenAuthenticationCannotProvideValidToken_ThrowsPlaceholderExceptionBeforeProtectedRequest()
        {
            var handler = new FailingStaffAuthenticationHttpMessageHandler();
            var client = CreateClient(handler);
            client.AllowStaffOverrideRequests = true;
            client.StaffOverrideAccount = new PolarisUser
            {
                Domain = "main",
                Username = $"failed-placeholder-{Guid.NewGuid():N}",
                Password = "secret"
            };
            client.Token = new ProtectedToken
            {
                AccessToken = "expired-token",
                AccessSecret = "expired-secret",
                ExpirationDate = DateTime.Now.AddHours(-1)
            };

            var exception = await Assert.ThrowsExactlyAsync<InvalidOperationException>(() => client.PatronSearchAsync("name=Smith", orgId: 9));

            StringAssert.Contains(exception.Message, "valid protected access token");
            StringAssert.Contains(exception.Message, "ProtectedToken.Placeholder");
            Assert.AreEqual(1, handler.AuthenticationRequestCount);
            Assert.AreEqual(0, handler.ProtectedRequestCount);
            Assert.IsNull(client.Token);
        }

        [TestMethod]
        public async Task ProtectedTokenPathRequest_UsesCachedProtectedTokenForPlaceholderReplacement()
        {
            var handler = new ProtectedTokenCancellationHttpMessageHandler();
            var firstClient = CreateClient(handler);
            firstClient.AllowStaffOverrideRequests = true;
            firstClient.UseProtectedTokenCache = true;
            firstClient.StaffOverrideAccount = new PolarisUser
            {
                Domain = "main",
                Username = $"cached-placeholder-{Guid.NewGuid():N}",
                Password = "secret"
            };

            await firstClient.PatronSearchAsync("name=Smith", orgId: 9);

            var secondClient = CreateClient(handler);
            secondClient.AllowStaffOverrideRequests = true;
            secondClient.UseProtectedTokenCache = true;
            secondClient.StaffOverrideAccount = new PolarisUser
            {
                Domain = firstClient.StaffOverrideAccount.Domain,
                Username = firstClient.StaffOverrideAccount.Username,
                Password = "secret"
            };

            var response = await secondClient.PatronSearchAsync("name=Jones", orgId: 9);

            Assert.IsNotNull(response);
            Assert.AreEqual(3, handler.Requests.Count);
            StringAssert.Contains(handler.Requests[0].RequestUri!.AbsolutePath, "/protected/v1/1033/100/1/authenticator/staff");
            StringAssert.Contains(handler.Requests[1].RequestUri!.AbsolutePath, "/protected/v1/1033/100/9/protected-token/search/patrons/Boolean");
            StringAssert.Contains(handler.Requests[2].RequestUri!.AbsolutePath, "/protected/v1/1033/100/9/protected-token/search/patrons/Boolean");
            Assert.IsFalse(handler.Requests[2].RequestUri!.AbsoluteUri.Contains(ProtectedToken.Placeholder, StringComparison.Ordinal));
        }

        [TestMethod]
        public async Task ProtectedTokenPathRequest_DoesNotShareCachedTokenAcrossStaffUsers()
        {
            var handler = new SequencedProtectedTokenHttpMessageHandler();
            var cacheSuffix = Guid.NewGuid().ToString("N");
            var firstClient = CreateClient(handler);
            firstClient.AllowStaffOverrideRequests = true;
            firstClient.UseProtectedTokenCache = true;
            firstClient.StaffOverrideAccount = new PolarisUser
            {
                Domain = "main",
                Username = $"cache-isolation-a-{cacheSuffix}",
                Password = "secret"
            };
            var secondClient = CreateClient(handler);
            secondClient.AllowStaffOverrideRequests = true;
            secondClient.UseProtectedTokenCache = true;
            secondClient.StaffOverrideAccount = new PolarisUser
            {
                Domain = "main",
                Username = $"cache-isolation-b-{cacheSuffix}",
                Password = "secret"
            };

            await firstClient.PatronSearchAsync("name=Smith", orgId: 9);
            await secondClient.PatronSearchAsync("name=Jones", orgId: 9);

            Assert.AreEqual(2, handler.AuthenticationRequestCount);
            Assert.AreEqual(2, handler.ProtectedRequests.Count);
            StringAssert.Contains(handler.ProtectedRequests[0].RequestUri!.AbsolutePath, "/protected/v1/1033/100/9/protected-token-1/search/patrons/Boolean");
            StringAssert.Contains(handler.ProtectedRequests[1].RequestUri!.AbsolutePath, "/protected/v1/1033/100/9/protected-token-2/search/patrons/Boolean");
        }

        [TestMethod]
        public async Task ProtectedTokenPathRequest_WithoutTokenOrStaffOverride_FailsBeforeSendingProtectedRequest()
        {
            var handler = new FailingStaffAuthenticationHttpMessageHandler();
            var client = CreateClient(handler);
            client.AllowStaffOverrideRequests = true;
            client.StaffOverrideAccount = null;
            client.Token = null;

            var exception = await Assert.ThrowsExactlyAsync<InvalidOperationException>(() => client.PatronSearchAsync("name=Smith", orgId: 9));

            StringAssert.Contains(exception.Message, "valid protected access token");
            StringAssert.Contains(exception.Message, "ProtectedToken.Placeholder");
            Assert.AreEqual(0, handler.AuthenticationRequestCount);
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
            Assert.AreEqual(8, handler.NonAuthenticationRequestCount);
            Assert.IsNotNull(client.Token);
            Assert.AreEqual("protected-token", client.Token.AccessToken);
        }

        [TestMethod]
        public async Task PatronSearchAsync_ConcurrentProtectedRequestsAcrossClientsForSameCacheKey_AuthenticateOnceAndUseSharedToken()
        {
            var hostname = $"https://example-{Guid.NewGuid():N}.test";
            var staff = CreateStaffUser(password: "shared-password");
            var handler = new ProtectedTokenHttpMessageHandler(HttpStatusCode.OK, CreateProtectedTokenJson("shared-token", "shared-secret", DateTime.Now.AddHours(1)));
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

            await Task.WhenAll(passwordAReuseClient.PatronSearchAsync("name=reuse-a"), passwordBReuseClient.PatronSearchAsync("name=reuse-b"));

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
            var handler = new ProtectedTokenHttpMessageHandler(HttpStatusCode.OK, CreateProtectedTokenJson("fresh-token", "fresh-secret", DateTime.Now.AddHours(1)));
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
            Assert.AreEqual(1, handler.NonAuthenticationRequestCount);
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
            Assert.AreEqual(1, handler.NonAuthenticationRequestCount);
            Assert.AreEqual("cached-token", client.Token?.AccessToken);
        }

        [TestMethod]
        public async Task PatronSearchAsync_CachedTokenWithSameUserButBlankPassword_IsNotReusedOrWrittenToCache()
        {
            var hostname = $"https://example-{Guid.NewGuid():N}.test";
            var staffWithPassword = CreateStaffUser(password: "correct-1");

            var handlerA = new ProtectedTokenHttpMessageHandler(HttpStatusCode.OK, CreateProtectedTokenJson("token-a", "secret-a", DateTime.Now.AddHours(1)));
            var clientA = CreateProtectedClient(handlerA);
            clientA.Hostname = hostname;
            clientA.StaffOverrideAccount = staffWithPassword;

            await clientA.PatronSearchAsync("name=Smith");

            var handlerB = new ProtectedTokenHttpMessageHandler(HttpStatusCode.OK, CreateProtectedTokenJson("token-b", "secret-b", DateTime.Now.AddHours(1)));
            var clientB = CreateProtectedClient(handlerB);
            clientB.Hostname = hostname;
            clientB.StaffOverrideAccount = CreateStaffUser(password: string.Empty);

            await clientB.PatronSearchAsync("name=Jones");

            Assert.AreEqual(1, handlerA.AuthenticationRequestCount);
            Assert.AreEqual(1, handlerB.AuthenticationRequestCount);
            Assert.AreEqual("token-b", clientB.Token?.AccessToken);
            Assert.AreEqual(1, GetProtectedTokenCacheCount());
            Assert.IsFalse(TryGetCachedToken(hostname, clientB.AccessID, clientB.AccessKey, clientB.StaffOverrideAccount, out _));
        }

        [TestMethod]
        public async Task PatronSearchAsync_CachedTokenWithSameUserButDifferentPassword_IsNotReused()
        {
            var hostname = $"https://example-{Guid.NewGuid():N}.test";

            var handlerA = new ProtectedTokenHttpMessageHandler(HttpStatusCode.OK, CreateProtectedTokenJson("token-a", "secret-a", DateTime.Now.AddHours(1)));
            var clientA = CreateProtectedClient(handlerA);
            clientA.Hostname = hostname;
            clientA.StaffOverrideAccount = CreateStaffUser(password: "correct-1");

            await clientA.PatronSearchAsync("name=Smith");

            var handlerB = new ProtectedTokenHttpMessageHandler(HttpStatusCode.OK, CreateProtectedTokenJson("token-b", "secret-b", DateTime.Now.AddHours(1)));
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

            var handlerA = new ProtectedTokenHttpMessageHandler(HttpStatusCode.OK, CreateProtectedTokenJson("token-a", "secret-a", DateTime.Now.AddHours(1)));
            var clientA = CreateProtectedClient(handlerA);
            clientA.Hostname = hostname;
            clientA.StaffOverrideAccount = staff;

            await clientA.PatronSearchAsync("name=Smith");

            var handlerB = new ProtectedTokenHttpMessageHandler(HttpStatusCode.OK, CreateProtectedTokenJson("token-b", "secret-b", DateTime.Now.AddHours(1)));
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

            await Assert.ThrowsExactlyAsync<InvalidOperationException>(() => client.PatronSearchAsync("name=Smith"));

            Assert.AreEqual(1, handler.AuthenticationRequestCount);
            Assert.IsFalse(TryGetCachedToken(client.Hostname, client.AccessID, client.AccessKey, client.StaffOverrideAccount, out _));
        }

        [TestMethod]
        public async Task PatronSearchAsync_CachedTokenWithBlankAccessToken_IsRemovedAndNewTokenIsUsed()
        {
            var handler = new ProtectedTokenHttpMessageHandler(HttpStatusCode.OK, CreateProtectedTokenJson("new-token", "new-secret", DateTime.Now.AddHours(1)));
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

            await Assert.ThrowsExactlyAsync<InvalidOperationException>(() => client.PatronSearchAsync("name=Smith"));

            Assert.AreEqual(1, handler.AuthenticationRequestCount);
            Assert.AreEqual(0, handler.NonAuthenticationRequestCount);
            Assert.IsFalse(TryGetCachedToken(client.Hostname, client.AccessID, client.AccessKey, client.StaffOverrideAccount, out _));
            Assert.IsNull(client.Token);
        }

        [TestMethod]
        public async Task PatronSearchAsync_FailedStaffAuthentication_ThrowsBeforeFinalProtectedRequest()
        {
            var handler = new ProtectedTokenHttpMessageHandler(HttpStatusCode.InternalServerError, "{\"PAPIErrorCode\":1}");
            var client = CreateProtectedClient(handler);

            var exception = await Assert.ThrowsExactlyAsync<InvalidOperationException>(() => client.PatronSearchAsync("name=Smith"));

            StringAssert.Contains(exception.Message, "Staff authentication did not succeed");
            Assert.AreEqual(1, handler.AuthenticationRequestCount);
            Assert.AreEqual(0, handler.NonAuthenticationRequestCount);
        }

        [TestMethod]
        public async Task PatronSearchAsync_NullDataStaffAuthentication_ThrowsBeforeFinalProtectedRequest()
        {
            var handler = new ProtectedTokenHttpMessageHandler(HttpStatusCode.OK, "null");
            var client = CreateProtectedClient(handler);

            var exception = await Assert.ThrowsExactlyAsync<InvalidOperationException>(() => client.PatronSearchAsync("name=Smith"));

            StringAssert.Contains(exception.Message, "did not return a usable protected access token");
            Assert.AreEqual(1, handler.AuthenticationRequestCount);
            Assert.AreEqual(0, handler.NonAuthenticationRequestCount);
        }

        [TestMethod]
        public async Task PatronSearchAsync_BlankAccessTokenStaffAuthentication_ThrowsBeforeFinalProtectedRequest()
        {
            var handler = new ProtectedTokenHttpMessageHandler(HttpStatusCode.OK, CreateProtectedTokenJson(" ", "protected-secret", DateTime.Now.AddHours(1)));
            var client = CreateProtectedClient(handler);

            await Assert.ThrowsExactlyAsync<InvalidOperationException>(() => client.PatronSearchAsync("name=Smith"));

            Assert.AreEqual(1, handler.AuthenticationRequestCount);
            Assert.AreEqual(0, handler.NonAuthenticationRequestCount);
        }

        [TestMethod]
        public async Task PatronSearchAsync_BlankAccessSecretStaffAuthentication_ThrowsBeforeFinalProtectedRequest()
        {
            var handler = new ProtectedTokenHttpMessageHandler(HttpStatusCode.OK, CreateProtectedTokenJson("protected-token", " ", DateTime.Now.AddHours(1)));
            var client = CreateProtectedClient(handler);

            await Assert.ThrowsExactlyAsync<InvalidOperationException>(() => client.PatronSearchAsync("name=Smith"));

            Assert.AreEqual(1, handler.AuthenticationRequestCount);
            Assert.AreEqual(0, handler.NonAuthenticationRequestCount);
        }

        [TestMethod]
        public async Task PatronSearchAsync_ExpiredTokenStaffAuthentication_ThrowsBeforeFinalProtectedRequest()
        {
            var handler = new ProtectedTokenHttpMessageHandler(
                HttpStatusCode.OK,
                CreateProtectedTokenJson("protected-token", "protected-secret", DateTime.Now.AddMinutes(-1)));
            var client = CreateProtectedClient(handler);

            await Assert.ThrowsExactlyAsync<InvalidOperationException>(() => client.PatronSearchAsync("name=Smith"));

            Assert.AreEqual(1, handler.AuthenticationRequestCount);
            Assert.AreEqual(0, handler.NonAuthenticationRequestCount);
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

            var exception = await Assert.ThrowsExactlyAsync<InvalidOperationException>(() => client.PatronSearchAsync("name=Smith"));

            Assert.IsFalse(exception.Message.Contains(client.StaffOverrideAccount.Password, StringComparison.Ordinal));
            Assert.IsFalse(exception.Message.Contains(client.AccessKey, StringComparison.Ordinal));
            Assert.IsFalse(exception.Message.Contains(returnedToken.AccessToken, StringComparison.Ordinal));
            Assert.IsFalse(exception.Message.Contains(returnedToken.AccessSecret, StringComparison.Ordinal));
            Assert.AreEqual(1, handler.AuthenticationRequestCount);
            Assert.AreEqual(0, handler.NonAuthenticationRequestCount);
            Assert.IsFalse(handler.RequestPaths.Any(path => path.Contains(ProtectedToken.Placeholder, StringComparison.Ordinal)));
        }

        private sealed class FailingStaffAuthenticationHttpMessageHandler : HttpMessageHandler
        {
            public int AuthenticationRequestCount { get; private set; }
            public int ProtectedRequestCount { get; private set; }
            public int PublicRequestCount { get; private set; }
            public HttpRequestMessage? LastNonAuthenticationRequest { get; private set; }

            protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            {
                if (request.RequestUri!.AbsolutePath.Contains("/authenticator/staff", StringComparison.Ordinal))
                {
                    AuthenticationRequestCount++;
                    return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
                    {
                        Content = new StringContent("{}", Encoding.UTF8, "application/json")
                    });
                }

                LastNonAuthenticationRequest = request;
                if (request.RequestUri!.AbsolutePath.Contains("/protected/", StringComparison.Ordinal))
                {
                    ProtectedRequestCount++;
                }
                else
                {
                    PublicRequestCount++;
                }

                return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent("{\"PAPIErrorCode\":0}", Encoding.UTF8, "application/json")
                });
            }
        }

        private sealed class SequencedProtectedTokenHttpMessageHandler : HttpMessageHandler
        {
            private readonly object _syncRoot = new();
            private int _authenticationRequestCount;

            public int AuthenticationRequestCount => _authenticationRequestCount;
            public List<HttpRequestMessage> ProtectedRequests { get; } = new();

            protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            {
                if (request.RequestUri!.AbsolutePath.Contains("/authenticator/staff", StringComparison.Ordinal))
                {
                    var requestNumber = Interlocked.Increment(ref _authenticationRequestCount);
                    return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
                    {
                        Content = new StringContent($"{{\"PAPIErrorCode\":0,\"AccessToken\":\"protected-token-{requestNumber}\",\"AccessSecret\":\"protected-secret-{requestNumber}\",\"AuthExpDate\":\"2030-01-01T00:00:00Z\"}}", Encoding.UTF8, "application/json")
                    });
                }

                lock (_syncRoot)
                {
                    ProtectedRequests.Add(request);
                }

                return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent("{\"PAPIErrorCode\":0}", Encoding.UTF8, "application/json")
                });
            }
        }
    }
}
