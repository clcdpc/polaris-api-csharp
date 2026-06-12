using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Clc.Polaris.Api.Models;
using Clc.Polaris.Api.Tests;
using Clc.Polaris.Api.Tests.TestInfrastructure;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Clc.Polaris.Api.Tests.PapiClientBehavior
{
    [TestClass]
    [DoNotParallelize]
    [UnitTest]
    public class ProtectedTokenCacheTests : PapiClientTestBase
    {
        [TestInitialize]
        public void TestInitialize()
        {
            ClearProtectedTokenState();
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
            Assert.Contains(client.Hostname.Trim(), cacheKey);
            Assert.Contains(client.AccessID.Trim(), cacheKey);
            Assert.Contains(client.StaffOverrideAccount.Domain.Trim(), cacheKey);
            Assert.Contains(client.StaffOverrideAccount.Username.Trim(), cacheKey);
        }

        [TestMethod]
        public async Task PatronSearchAsync_SameHostAndStaffWithDifferentAccessIds_AuthenticatesAndCachesSeparateTokens()
        {
            var hostname = $"https://example-{Guid.NewGuid():N}.test";
            var staff = CreateStaffUser();

            var handlerA = new ProtectedTokenHttpMessageHandler(HttpStatusCode.OK, CreateProtectedTokenJson(accessToken: "token-a", accessSecret: "secret-a", expirationDate: ValidProtectedTokenExpirationDate));
            var clientA = CreateProtectedClient(handlerA);
            clientA.Hostname = hostname;
            clientA.AccessID = "access-a";
            clientA.StaffOverrideAccount = staff;

            await clientA.PatronSearchAsync("name=Smith", cancellationToken: TestContext.CancellationToken);

            Assert.AreEqual(1, handlerA.AuthenticationRequestCount);
            Assert.AreEqual(1, handlerA.NonAuthenticationRequestCount);
            Assert.AreEqual("token-a", clientA.Token?.AccessToken);
            Assert.IsTrue(TryGetCachedToken(hostname, "access-a", clientA.AccessKey, staff, out var cachedTokenA));
            Assert.IsNotNull(cachedTokenA);
            Assert.AreEqual("token-a", cachedTokenA.AccessToken);

            var handlerB = new ProtectedTokenHttpMessageHandler(HttpStatusCode.OK, CreateProtectedTokenJson(accessToken: "token-b", accessSecret: "secret-b", expirationDate: ValidProtectedTokenExpirationDate));
            var clientB = CreateProtectedClient(handlerB);
            clientB.Hostname = hostname;
            clientB.AccessID = "access-b";
            clientB.StaffOverrideAccount = staff;

            await clientB.PatronSearchAsync("name=Jones", cancellationToken: TestContext.CancellationToken);

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

            await client.PatronSearchAsync("name=Smith", cancellationToken: TestContext.CancellationToken);

            Assert.AreEqual(1, handler.AuthenticationRequestCount);
            Assert.AreEqual(1, handler.NonAuthenticationRequestCount);
            Assert.IsNotNull(client.Token);
            Assert.AreEqual("protected-token", client.Token.AccessToken);
            var paths = handler.RequestPaths;
            Assert.HasCount(2, paths);
            Assert.Contains("/protected/v1/1033/100/1/authenticator/staff", paths[0]);
            Assert.Contains("/protected/v1/1033/100/1/protected-token/search/patrons/Boolean", paths[1]);
            Assert.IsTrue(TryGetCachedToken(client.Hostname, client.AccessID, client.AccessKey, client.StaffOverrideAccount, out var cachedToken));
            Assert.IsNotNull(cachedToken);
            Assert.AreEqual("protected-token", cachedToken.AccessToken);
            Assert.AreNotSame(client.Token, cachedToken);
        }

        [TestMethod]
        public async Task PatronSearchAsync_CacheDisabledAuthenticationFailure_DoesNotEvictSharedCachedToken()
        {
            var hostname = $"https://example-{Guid.NewGuid():N}.test";
            var staff = CreateStaffUser(password: "shared-password");
            var cachedToken = new ProtectedToken
            {
                AccessToken = "cached-token",
                AccessSecret = "cached-secret",
                ExpirationDate = ValidProtectedTokenExpirationDate
            };

            SetCachedToken(hostname, "access-id", "access-key", staff, cachedToken);

            var failingHandler = new ProtectedTokenHttpMessageHandler(HttpStatusCode.Unauthorized, CreatePapiResponseJson(1));
            var cacheDisabledClient = CreateProtectedClient(failingHandler, hostname, staff);
            cacheDisabledClient.UseProtectedTokenCache = false;

            await Assert.ThrowsExactlyAsync<InvalidOperationException>(
                async () => await cacheDisabledClient.PatronSearchAsync("name=Failure", cancellationToken: TestContext.CancellationToken).ConfigureAwait(false));

            Assert.AreEqual(1, failingHandler.AuthenticationRequestCount);
            Assert.AreEqual(0, failingHandler.NonAuthenticationRequestCount);
            Assert.IsTrue(TryGetCachedToken(hostname, "access-id", "access-key", staff, out var cachedTokenAfterFailure));
            Assert.IsNotNull(cachedTokenAfterFailure);
            Assert.AreEqual("cached-token", cachedTokenAfterFailure!.AccessToken);

            var cacheEnabledHandler = new ProtectedTokenHttpMessageHandler();
            var cacheEnabledClient = CreateProtectedClient(cacheEnabledHandler, hostname, staff);

            var response = await cacheEnabledClient.PatronSearchAsync("name=Cached", cancellationToken: TestContext.CancellationToken).ConfigureAwait(false);

            Assert.IsNotNull(response);
            Assert.AreEqual(0, cacheEnabledHandler.AuthenticationRequestCount);
            Assert.AreEqual(1, cacheEnabledHandler.NonAuthenticationRequestCount);
            Assert.AreEqual("cached-token", cacheEnabledClient.Token?.AccessToken);
            Assert.Contains(
                "/protected/v1/1033/100/1/cached-token/search/patrons/Boolean",
                cacheEnabledHandler.CapturedRequests.Single(request => !request.IsStaffAuthenticationRequest).Path);
        }}
}
