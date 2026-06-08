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
    [UnitCategory]
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
