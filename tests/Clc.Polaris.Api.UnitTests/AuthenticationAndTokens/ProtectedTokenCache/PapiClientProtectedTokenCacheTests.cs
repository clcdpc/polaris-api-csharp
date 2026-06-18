namespace Clc.Polaris.Api.UnitTests.AuthenticationAndTokens.ProtectedTokenCache
{
    [TestClass]
    [UnitTest]
    [DoNotParallelize]
    public sealed class PapiClientProtectedTokenCacheTests : PapiClientUnitTestBase
    {
        [TestInitialize]
        public void TestInitialize()
        {
            global::Clc.Polaris.Api.ProtectedTokenCache.ClearForTesting();
        }

        [TestCleanup]
        public void TestCleanup()
        {
            global::Clc.Polaris.Api.ProtectedTokenCache.ClearForTesting();
        }

        [TestMethod]
        public void PruneProtectedTokenCache_RemovesExpiredToken()
        {
            global::Clc.Polaris.Api.ProtectedTokenCache.AddForTesting("expired", CreateToken(ExpiredProtectedTokenExpirationDate));
            global::Clc.Polaris.Api.ProtectedTokenCache.AddForTesting("valid", CreateToken(ValidProtectedTokenExpirationDate));

            var removedCount = global::Clc.Polaris.Api.ProtectedTokenCache.PruneExpired();

            Assert.AreEqual(1, removedCount);
            Assert.IsFalse(global::Clc.Polaris.Api.ProtectedTokenCache.ContainsKey("expired"));
            Assert.IsTrue(global::Clc.Polaris.Api.ProtectedTokenCache.ContainsKey("valid"));
        }

        [TestMethod]
        public void PruneProtectedTokenCache_RemovesTokenWithoutExpirationDate()
        {
            global::Clc.Polaris.Api.ProtectedTokenCache.AddForTesting("missing-expiration", CreateToken(expirationDate: null));
            global::Clc.Polaris.Api.ProtectedTokenCache.AddForTesting("valid", CreateToken(ValidProtectedTokenExpirationDate));

            var removedCount = global::Clc.Polaris.Api.ProtectedTokenCache.PruneExpired();

            Assert.AreEqual(1, removedCount);
            Assert.IsFalse(global::Clc.Polaris.Api.ProtectedTokenCache.ContainsKey("missing-expiration"));
            Assert.IsTrue(global::Clc.Polaris.Api.ProtectedTokenCache.ContainsKey("valid"));
        }

        [TestMethod]
        public void PruneProtectedTokenCache_RemovesTokenWithoutAccessToken()
        {
            global::Clc.Polaris.Api.ProtectedTokenCache.AddForTesting("missing-access-token", CreateToken(ValidProtectedTokenExpirationDate, accessToken: ""));
            global::Clc.Polaris.Api.ProtectedTokenCache.AddForTesting("valid", CreateToken(ValidProtectedTokenExpirationDate));

            var removedCount = global::Clc.Polaris.Api.ProtectedTokenCache.PruneExpired();

            Assert.AreEqual(1, removedCount);
            Assert.IsFalse(global::Clc.Polaris.Api.ProtectedTokenCache.ContainsKey("missing-access-token"));
            Assert.IsTrue(global::Clc.Polaris.Api.ProtectedTokenCache.ContainsKey("valid"));
        }

        [TestMethod]
        public void PruneProtectedTokenCache_RemovesTokenWithoutAccessSecret()
        {
            global::Clc.Polaris.Api.ProtectedTokenCache.AddForTesting("missing-access-secret", CreateToken(ValidProtectedTokenExpirationDate, accessSecret: ""));
            global::Clc.Polaris.Api.ProtectedTokenCache.AddForTesting("valid", CreateToken(ValidProtectedTokenExpirationDate));

            var removedCount = global::Clc.Polaris.Api.ProtectedTokenCache.PruneExpired();

            Assert.AreEqual(1, removedCount);
            Assert.IsFalse(global::Clc.Polaris.Api.ProtectedTokenCache.ContainsKey("missing-access-secret"));
            Assert.IsTrue(global::Clc.Polaris.Api.ProtectedTokenCache.ContainsKey("valid"));
        }

        [TestMethod]
        public void PruneProtectedTokenCache_RemovesTokenInsideExpirationSkew()
        {
            global::Clc.Polaris.Api.ProtectedTokenCache.AddForTesting("inside-skew", CreateToken(DateTime.UtcNow.AddSeconds(30)));
            global::Clc.Polaris.Api.ProtectedTokenCache.AddForTesting("valid", CreateToken(ValidProtectedTokenExpirationDate));

            var removedCount = global::Clc.Polaris.Api.ProtectedTokenCache.PruneExpired();

            Assert.AreEqual(1, removedCount);
            Assert.IsFalse(global::Clc.Polaris.Api.ProtectedTokenCache.ContainsKey("inside-skew"));
            Assert.IsTrue(global::Clc.Polaris.Api.ProtectedTokenCache.ContainsKey("valid"));
        }

        [TestMethod]
        public void PruneProtectedTokenCache_DoesNotRemoveUsableTokens()
        {
            global::Clc.Polaris.Api.ProtectedTokenCache.AddForTesting("valid-1", CreateToken(ValidProtectedTokenExpirationDate));
            global::Clc.Polaris.Api.ProtectedTokenCache.AddForTesting("valid-2", CreateToken(ValidProtectedTokenExpirationDate));

            var removedCount = global::Clc.Polaris.Api.ProtectedTokenCache.PruneExpired();

            Assert.AreEqual(0, removedCount);
            Assert.IsTrue(global::Clc.Polaris.Api.ProtectedTokenCache.ContainsKey("valid-1"));
            Assert.IsTrue(global::Clc.Polaris.Api.ProtectedTokenCache.ContainsKey("valid-2"));
        }

        private static ProtectedToken CreateToken(DateTime? expirationDate, string accessToken = "access-token", string accessSecret = "access-secret")
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
