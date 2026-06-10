using System;
using Clc.Polaris.Api.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Clc.Polaris.Api.Tests
{
    [TestClass]
    [UnitTest]
    [DoNotParallelize]
    public sealed class PapiClientProtectedTokenCacheTests
    {
        [TestInitialize]
        public void TestInitialize()
        {
            PapiClient.ClearProtectedTokenCache();
        }

        [TestCleanup]
        public void TestCleanup()
        {
            PapiClient.ClearProtectedTokenCache();
        }

        [TestMethod]
        public void PruneProtectedTokenCache_RemovesExpiredToken()
        {
            PapiClient.AddProtectedTokenToCacheForTesting("expired", CreateToken(DateTime.UtcNow.AddMinutes(-5)));
            PapiClient.AddProtectedTokenToCacheForTesting("valid", CreateToken(DateTime.UtcNow.AddMinutes(5)));

            var removedCount = PapiClient.PruneProtectedTokenCache();

            Assert.AreEqual(1, removedCount);
            Assert.IsFalse(PapiClient.ProtectedTokenCacheContainsKey("expired"));
            Assert.IsTrue(PapiClient.ProtectedTokenCacheContainsKey("valid"));
        }

        [TestMethod]
        public void PruneProtectedTokenCache_RemovesTokenWithoutExpirationDate()
        {
            PapiClient.AddProtectedTokenToCacheForTesting("missing-expiration", CreateToken(expirationDate: null));
            PapiClient.AddProtectedTokenToCacheForTesting("valid", CreateToken(DateTime.UtcNow.AddMinutes(5)));

            var removedCount = PapiClient.PruneProtectedTokenCache();

            Assert.AreEqual(1, removedCount);
            Assert.IsFalse(PapiClient.ProtectedTokenCacheContainsKey("missing-expiration"));
            Assert.IsTrue(PapiClient.ProtectedTokenCacheContainsKey("valid"));
        }

        [TestMethod]
        public void PruneProtectedTokenCache_RemovesTokenWithoutAccessToken()
        {
            PapiClient.AddProtectedTokenToCacheForTesting(
                "missing-access-token",
                CreateToken(DateTime.UtcNow.AddMinutes(5), accessToken: ""));

            PapiClient.AddProtectedTokenToCacheForTesting("valid", CreateToken(DateTime.UtcNow.AddMinutes(5)));

            var removedCount = PapiClient.PruneProtectedTokenCache();

            Assert.AreEqual(1, removedCount);
            Assert.IsFalse(PapiClient.ProtectedTokenCacheContainsKey("missing-access-token"));
            Assert.IsTrue(PapiClient.ProtectedTokenCacheContainsKey("valid"));
        }

        [TestMethod]
        public void PruneProtectedTokenCache_RemovesTokenWithoutAccessSecret()
        {
            PapiClient.AddProtectedTokenToCacheForTesting(
                "missing-access-secret",
                CreateToken(DateTime.UtcNow.AddMinutes(5), accessSecret: ""));

            PapiClient.AddProtectedTokenToCacheForTesting("valid", CreateToken(DateTime.UtcNow.AddMinutes(5)));

            var removedCount = PapiClient.PruneProtectedTokenCache();

            Assert.AreEqual(1, removedCount);
            Assert.IsFalse(PapiClient.ProtectedTokenCacheContainsKey("missing-access-secret"));
            Assert.IsTrue(PapiClient.ProtectedTokenCacheContainsKey("valid"));
        }

        [TestMethod]
        public void PruneProtectedTokenCache_RemovesTokenInsideExpirationSkew()
        {
            PapiClient.AddProtectedTokenToCacheForTesting("inside-skew", CreateToken(DateTime.UtcNow.AddSeconds(30)));
            PapiClient.AddProtectedTokenToCacheForTesting("valid", CreateToken(DateTime.UtcNow.AddMinutes(5)));

            var removedCount = PapiClient.PruneProtectedTokenCache();

            Assert.AreEqual(1, removedCount);
            Assert.IsFalse(PapiClient.ProtectedTokenCacheContainsKey("inside-skew"));
            Assert.IsTrue(PapiClient.ProtectedTokenCacheContainsKey("valid"));
        }

        [TestMethod]
        public void PruneProtectedTokenCache_DoesNotRemoveUsableTokens()
        {
            PapiClient.AddProtectedTokenToCacheForTesting("valid-1", CreateToken(DateTime.UtcNow.AddMinutes(5)));
            PapiClient.AddProtectedTokenToCacheForTesting("valid-2", CreateToken(DateTime.UtcNow.AddMinutes(10)));

            var removedCount = PapiClient.PruneProtectedTokenCache();

            Assert.AreEqual(0, removedCount);
            Assert.IsTrue(PapiClient.ProtectedTokenCacheContainsKey("valid-1"));
            Assert.IsTrue(PapiClient.ProtectedTokenCacheContainsKey("valid-2"));
        }

        private static ProtectedToken CreateToken(
            DateTime? expirationDate,
            string accessToken = "access-token",
            string accessSecret = "access-secret")
        {
            return new ProtectedToken
            {
                AccessToken = accessToken,
                AccessSecret = accessSecret,
                ExpirationDate = expirationDate
            };
        }
    }
}