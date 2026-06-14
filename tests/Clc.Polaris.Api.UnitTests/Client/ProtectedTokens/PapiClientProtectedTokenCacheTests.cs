namespace Clc.Polaris.Api.UnitTests
{
    [TestClass]
    [UnitTest]
    [DoNotParallelize]
    public sealed class PapiClientProtectedTokenCacheTests : PapiClientUnitTestBase
    {
        [TestInitialize]
        public void TestInitialize()
        {
            ProtectedTokenCache.ClearForTesting();
        }

        [TestCleanup]
        public void TestCleanup()
        {
            ProtectedTokenCache.ClearForTesting();
        }

        [TestMethod]
        public void PruneProtectedTokenCache_RemovesExpiredToken()
        {
            ProtectedTokenCache.AddForTesting("expired", CreateToken(ExpiredProtectedTokenExpirationDate));
            ProtectedTokenCache.AddForTesting("valid", CreateToken(ValidProtectedTokenExpirationDate));

            var removedCount = ProtectedTokenCache.PruneExpired();

            Assert.AreEqual(1, removedCount);
            Assert.IsFalse(ProtectedTokenCache.ContainsKey("expired"));
            Assert.IsTrue(ProtectedTokenCache.ContainsKey("valid"));
        }

        [TestMethod]
        public void PruneProtectedTokenCache_RemovesTokenWithoutExpirationDate()
        {
            ProtectedTokenCache.AddForTesting("missing-expiration", CreateToken(expirationDate: null));
            ProtectedTokenCache.AddForTesting("valid", CreateToken(ValidProtectedTokenExpirationDate));

            var removedCount = ProtectedTokenCache.PruneExpired();

            Assert.AreEqual(1, removedCount);
            Assert.IsFalse(ProtectedTokenCache.ContainsKey("missing-expiration"));
            Assert.IsTrue(ProtectedTokenCache.ContainsKey("valid"));
        }

        [TestMethod]
        public void PruneProtectedTokenCache_RemovesTokenWithoutAccessToken()
        {
            ProtectedTokenCache.AddForTesting("missing-access-token", CreateToken(ValidProtectedTokenExpirationDate, accessToken: ""));
            ProtectedTokenCache.AddForTesting("valid", CreateToken(ValidProtectedTokenExpirationDate));

            var removedCount = ProtectedTokenCache.PruneExpired();

            Assert.AreEqual(1, removedCount);
            Assert.IsFalse(ProtectedTokenCache.ContainsKey("missing-access-token"));
            Assert.IsTrue(ProtectedTokenCache.ContainsKey("valid"));
        }

        [TestMethod]
        public void PruneProtectedTokenCache_RemovesTokenWithoutAccessSecret()
        {
            ProtectedTokenCache.AddForTesting("missing-access-secret", CreateToken(ValidProtectedTokenExpirationDate, accessSecret: ""));
            ProtectedTokenCache.AddForTesting("valid", CreateToken(ValidProtectedTokenExpirationDate));

            var removedCount = ProtectedTokenCache.PruneExpired();

            Assert.AreEqual(1, removedCount);
            Assert.IsFalse(ProtectedTokenCache.ContainsKey("missing-access-secret"));
            Assert.IsTrue(ProtectedTokenCache.ContainsKey("valid"));
        }

        [TestMethod]
        public void PruneProtectedTokenCache_RemovesTokenInsideExpirationSkew()
        {
            ProtectedTokenCache.AddForTesting("inside-skew", CreateToken(DateTime.UtcNow.AddSeconds(30)));
            ProtectedTokenCache.AddForTesting("valid", CreateToken(ValidProtectedTokenExpirationDate));

            var removedCount = ProtectedTokenCache.PruneExpired();

            Assert.AreEqual(1, removedCount);
            Assert.IsFalse(ProtectedTokenCache.ContainsKey("inside-skew"));
            Assert.IsTrue(ProtectedTokenCache.ContainsKey("valid"));
        }

        [TestMethod]
        public void PruneProtectedTokenCache_DoesNotRemoveUsableTokens()
        {
            ProtectedTokenCache.AddForTesting("valid-1", CreateToken(ValidProtectedTokenExpirationDate));
            ProtectedTokenCache.AddForTesting("valid-2", CreateToken(ValidProtectedTokenExpirationDate));

            var removedCount = ProtectedTokenCache.PruneExpired();

            Assert.AreEqual(0, removedCount);
            Assert.IsTrue(ProtectedTokenCache.ContainsKey("valid-1"));
            Assert.IsTrue(ProtectedTokenCache.ContainsKey("valid-2"));
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
