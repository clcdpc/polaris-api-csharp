namespace Clc.Polaris.Api.Tests.Methods.RequestShape
{
    [TestClass]
    [UnitTest]
    public class ApiKeyValidateTests : PapiClientTestBase
    {
        [TestMethod]
        public async Task ApiKeyValidate_RequestShape_IsStable()
        {
            var handler = new CapturingHttpMessageHandler(CreatePapiResponseJson());
            var client = CreateClient(handler);

            var response = await client.ApiKeyValidateAsync(TestContext.CancellationToken);

            Assert.IsNotNull(response);
            Assert.AreEqual(HttpMethod.Get, handler.LastRequest!.Method);
            Assert.Contains("/public/v1/1033/100/1/apikeyvalidate", handler.LastRequest.RequestUri!.AbsolutePath);
            Assert.AreEqual(string.Empty, handler.LastRequest.RequestUri.Query);
            Assert.IsNull(handler.LastRequest.Content);
            Assert.IsTrue(handler.LastRequest.Headers.Contains("PolarisDate"));
            Assert.IsTrue(handler.LastRequest.Headers.Contains("Authorization"));
        }
    }
}
