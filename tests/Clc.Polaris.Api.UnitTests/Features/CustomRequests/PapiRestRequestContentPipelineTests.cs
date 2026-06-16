namespace Clc.Polaris.Api.UnitTests.Features.CustomRequests
{
    [TestClass]
    [UnitTest]
    public sealed class PapiRestRequestContentPipelineTests : PapiClientUnitTestBase
    {
        [TestMethod]
        public async Task ExecutePapiAsync_WithRawContent_PreservesContentThroughPipeline()
        {
            var handler = new CapturingHttpMessageHandler(CreatePapiResponseJson());
            var client = CreateClient(handler);
            var request = PapiRestRequest.Post("/public/v1/1033/100/1/test");
            request.Content = new System.Net.Http.StringContent("<root />", System.Text.Encoding.UTF8, "application/xml");

            await client.ExecutePapiAsync<PAPIResult>(request, TestContext.CancellationToken);

            Assert.AreEqual("<root />", GetLastRequestBody(handler));
            Assert.AreEqual("application/xml", GetLastRequest(handler).Content!.Headers.ContentType!.MediaType);
            Assert.AreEqual("utf-8", GetLastRequest(handler).Content!.Headers.ContentType!.CharSet);
        }
    }
}
