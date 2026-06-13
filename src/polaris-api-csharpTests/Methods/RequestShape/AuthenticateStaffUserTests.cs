namespace Clc.Polaris.Api.Tests.Methods.RequestShape
{
    [TestClass]
    [UnitTest]
    public class AuthenticateStaffUserTests : PapiClientTestBase
    {
        [TestMethod]
        public async Task AuthenticateStaffUser_RequestShape_IsStable()
        {
            var handler = new CapturingHttpMessageHandler(CreateProtectedTokenJson(accessToken: "t", accessSecret: "s", expirationDate: ValidProtectedTokenExpirationDate));
            var client = CreateClient(handler);

            var response = await client.AuthenticateStaffUserAsync(new PolarisUser("main", "staff", "secret"), TestContext.CancellationToken);

            Assert.IsNotNull(response);
            Assert.AreEqual(HttpMethod.Post, handler.LastRequest!.Method);
            Assert.Contains("/protected/v1/1033/100/1/authenticator/staff", handler.LastRequest.RequestUri!.AbsolutePath);
            Assert.IsNotNull(handler.LastRequest.Content);
            Assert.IsTrue(handler.LastRequest.Headers.Contains("PolarisDate"));
            Assert.IsTrue(handler.LastRequest.Headers.Contains("Authorization"));
            Assert.IsNotNull(handler.LastRequestContent);
            Assert.Contains("main", handler.LastRequestContent);
            Assert.Contains("staff", handler.LastRequestContent);
            Assert.Contains("secret", handler.LastRequestContent);
        }
    }
}
