namespace Clc.Polaris.Api.Tests
{
    [TestClass]
    [UnitTest]
    public sealed class PapiClientProtectedTokenExpirationTests : PapiClientTestBase
    {
        [TestMethod]
        public void Token_WhenTokenIsNull_ReturnsNull()
        {
            var client = new PapiClient();

            Assert.IsNull(client.Token);
        }

        [TestMethod]
        public void Token_WhenExpirationDateIsMissing_ReturnsNull()
        {
            var client = new PapiClient
            {
                Token = CreateToken(expirationDate: null)
            };

            Assert.IsNull(client.Token);
        }

        [TestMethod]
        public void Token_WhenUtcExpirationDateIsExpired_ReturnsNull()
        {
            var client = new PapiClient
            {
                Token = CreateToken(DateTime.UtcNow.AddMinutes(-5))
            };

            Assert.IsNull(client.Token);
        }

        [TestMethod]
        public void Token_WhenUtcExpirationDateIsInsideExpirationSkew_ReturnsNull()
        {
            var client = new PapiClient
            {
                Token = CreateToken(DateTime.UtcNow.AddSeconds(30))
            };

            Assert.IsNull(client.Token);
        }

        [TestMethod]
        public void Token_WhenUtcExpirationDateIsOutsideExpirationSkew_ReturnsToken()
        {
            var token = CreateToken(DateTime.UtcNow.AddMinutes(5));
            var client = new PapiClient
            {
                Token = token
            };

            Assert.AreSame(token, client.Token);
        }

        [TestMethod]
        public void Token_WhenUnspecifiedExpirationDateIsExpired_ReturnsNull()
        {
            var expirationDate = DateTime.SpecifyKind(DateTime.UtcNow.AddMinutes(-5), DateTimeKind.Unspecified);

            var client = new PapiClient
            {
                Token = CreateToken(expirationDate)
            };

            Assert.IsNull(client.Token);
        }

        [TestMethod]
        public void Token_WhenUnspecifiedExpirationDateIsOutsideExpirationSkew_ReturnsToken()
        {
            var expirationDate = DateTime.SpecifyKind(DateTime.UtcNow.AddMinutes(5), DateTimeKind.Unspecified);
            var token = CreateToken(expirationDate);
            var client = new PapiClient
            {
                Token = token
            };

            Assert.AreSame(token, client.Token);
        }

        [TestMethod]
        public void Token_WhenLocalExpirationDateIsOutsideExpirationSkew_ReturnsToken()
        {
            var token = CreateToken(ValidProtectedTokenExpirationDate);
            var client = new PapiClient
            {
                Token = token
            };

            Assert.AreSame(token, client.Token);
        }

        private static ProtectedToken CreateToken(DateTime? expirationDate)
        {
            return new ProtectedToken
            {
                AccessToken = "access-token",
                AccessSecret = "access-secret",
                ExpirationDate = expirationDate
            };
        }
    }
}
