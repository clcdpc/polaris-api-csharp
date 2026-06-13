namespace Clc.Polaris.Api.Tests
{
    [TestClass]
    [UnitTest]
    public class AuthenticatePatronTests : PapiClientTestBase
    {
        [TestMethod]
        public async Task AuthenticatePatron_SendsPostRequestWithJsonBody()
        {
            var handler = new CapturingHttpMessageHandler(CreateJson(new { PAPIErrorCode = 0, AccessToken = "mock-token", AccessSecret = "mock-secret", PatronID = 123 }));
            var client = CreateClient(handler);
            var barcode = "21945001234567";
            var password = "mypassword";

            var response = await client.AuthenticatePatronAsync(barcode, password, TestContext.CancellationToken);

            Assert.AreEqual(HttpMethod.Post, GetLastRequest(handler).Method);
            Assert.AreEqual("/PAPIService/REST/public/v1/1033/100/1/authenticator/patron", GetLastRequestUri(handler).AbsolutePath);
            AssertLastRequestBodyJsonPropertyValue(handler, "Barcode", barcode);
            AssertLastRequestBodyJsonPropertyValue(handler, "Password", password);

            Assert.IsNotNull(response.Data);
            Assert.AreEqual(0, response.Data.PAPIErrorCode);
            Assert.AreEqual("mock-token", response.Data.AccessToken);
            Assert.AreEqual("mock-secret", response.Data.AccessSecret);
            Assert.AreEqual(123, response.Data.PatronID);
        }
    }
}
