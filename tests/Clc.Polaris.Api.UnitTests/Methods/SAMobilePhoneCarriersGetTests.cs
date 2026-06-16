namespace Clc.Polaris.Api.UnitTests.Methods
{
    [TestClass]
    [UnitTest]
    public sealed class SAMobilePhoneCarriersGetTests : PapiClientUnitTestBase
    {
        [TestMethod]
        public async Task SAMobilePhoneCarriersGetAsync_SendsProtectedGetRequest()
        {
            var handler = new CapturingHttpMessageHandler(CreatePapiResponseJson());
            var client = CreateConfiguredClient(handler);
            client.Token = CreateToken();

            await client.SAMobilePhoneCarriersGetAsync(cancellationToken: TestContext.CancellationToken);

            Assert.AreEqual(HttpMethod.Get, GetLastRequest(handler).Method);
            Assert.AreEqual("/PAPIService/REST/protected/v1/1033/100/101/protected-token/sysadmin/mobilephonecarriers", GetLastRequestUri(handler).AbsolutePath);
        }

        private static PapiClient CreateConfiguredClient(CapturingHttpMessageHandler handler)
        {
            var client = CreateClient(handler);
            client.OrganizationId = 101;
            return client;
        }

        private static ProtectedToken CreateToken()
        {
            return new ProtectedToken
            {
                AccessToken = "protected-token",
                AccessSecret = "protected-secret",
                ExpirationDate = ValidProtectedTokenExpirationDate
            };
        }
    }
}
