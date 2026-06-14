using System.Net;

namespace Clc.Polaris.Api.UnitTests.Methods.RequestShape
{
    [TestClass]
    [UnitTest]
    public class PatronUpdateTests : PapiClientUnitTestBase
    {
        [TestMethod]
        public async Task PatronUpdate_RequestShape_IsStable()
        {
            var handler = new CapturingHttpMessageHandler(CreatePapiResponseJson());
            var client = CreateClient(handler);

            var response = await client.PatronUpdateAsync("AB C/+#?=", new PatronUpdateParams { EmailAddress = "patron@example.test" }, "1234", ignoresa: true, TestContext.CancellationToken);

            Assert.IsNotNull(response);
            Assert.AreEqual(HttpMethod.Put, handler.LastRequest!.Method);
            var encodedBarcode = WebUtility.UrlEncode("AB C/+#?=");
            Assert.Contains($"/public/v1/1033/100/1/patron/{encodedBarcode}", handler.LastRequest.RequestUri!.AbsolutePath);
            var query = ParseQuery(handler.LastRequest.RequestUri.Query);
            Assert.AreEqual("True", query["ignoresa"]);
            Assert.IsNotNull(handler.LastRequest.Content);
            Assert.IsTrue(handler.LastRequest.Headers.Contains("PolarisDate"));
            Assert.IsTrue(handler.LastRequest.Headers.Contains("Authorization"));
            Assert.IsNotNull(handler.LastRequestContent);
            Assert.Contains("patron@example.test", handler.LastRequestContent);
        }
    }
}
