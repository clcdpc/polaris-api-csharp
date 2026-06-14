namespace Clc.Polaris.Api.UnitTests.PapiClientBehavior
{
    [TestClass]
    [UnitTest]
    public class PreformatRestRequestHeaderTests : PapiClientUnitTestBase
    {
        [TestMethod]
        public void PreformatRestRequest_AddsPapiHeaders_WhenAuthRequired()
        {
            var client = CreateClient();
            var request = new PapiRestRequest(HttpMethod.Get, "/public/v1/1033/100/1/apikeyvalidate");

            var formatted = (PapiRestRequest)client.PreformatRestRequest(request);

            Assert.AreEqual(HttpMethod.Get, formatted.Method);
            Assert.AreEqual("/public/v1/1033/100/1/apikeyvalidate", formatted.Path);
            Assert.IsTrue(formatted.Headers.ContainsKey("PolarisDate"));
            Assert.IsTrue(formatted.Headers.ContainsKey("Authorization"));
            Assert.StartsWith("PWS access-id:", formatted.Headers["Authorization"]);
        }

        [TestMethod]
        public void PreformatRestRequest_DoesNotAddPapiHeaders_WhenAuthNotRequired()
        {
            var client = CreateClient();
            var request = new PapiRestRequest(HttpMethod.Get, "/public/v1/1033/100/1/apikeyvalidate")
            {
                AuthRequired = false
            };

            var formatted = (PapiRestRequest)client.PreformatRestRequest(request);

            Assert.IsFalse(formatted.Headers.ContainsKey("PolarisDate"));
            Assert.IsFalse(formatted.Headers.ContainsKey("Authorization"));
        }
    }
}
