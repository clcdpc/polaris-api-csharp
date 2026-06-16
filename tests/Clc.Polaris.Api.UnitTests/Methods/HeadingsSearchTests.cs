namespace Clc.Polaris.Api.UnitTests.Methods
{
    [TestClass]
    [UnitTest]
    public sealed class HeadingsSearchTests : PapiClientUnitTestBase
    {
        [TestMethod]
        public async Task HeadingsSearchAsync_SendsExpectedRequest()
        {
            var handler = new CapturingHttpMessageHandler(CreatePapiResponseJson());
            var client = CreateConfiguredClient(handler);

            await client.HeadingsSearchAsync(HeadingSearchQualifier.AU, 10, 1, startPoint: "twain", noTransaction: 1, cancellationToken: TestContext.CancellationToken);

            Assert.AreEqual(HttpMethod.Get, GetLastRequest(handler).Method);
            Assert.AreEqual("/PAPIService/REST/public/v1/1033/100/101/search/headings/AU", GetLastRequestUri(handler).AbsolutePath);
            AssertLastRequestQueryParameter(handler, "startpoint", "twain");
            AssertLastRequestQueryParameter(handler, "numterms", "10");
            AssertLastRequestQueryParameter(handler, "preferredpos", "1");
            AssertLastRequestQueryParameter(handler, "notran", "1");
        }

        private static PapiClient CreateConfiguredClient(CapturingHttpMessageHandler handler)
        {
            var client = CreateClient(handler);
            client.OrganizationId = 101;
            return client;
        }
    }
}
