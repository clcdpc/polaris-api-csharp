namespace Clc.Polaris.Api.UnitTests.Features.Bibliographic
{
    [TestClass]
    [UnitTest]
    public sealed class BibsPostRequestShape : PapiClientUnitTestBase
    {
        [TestMethod]
        public async Task BibsPostAsync_SendsProtectedPostWithXmlAndOptionalParameters()
        {
            var handler = new CapturingHttpMessageHandler(CreatePapiResponseJson());
            var client = CreateConfiguredClient(handler);
            client.Token = CreateToken();
            var marcXml = "<collection xmlns=\"http://www.loc.gov/MARC21/slim\" />";

            await client.BibsPostAsync(marcXml, importProfileName: "profile", workstationId: 77, cancellationToken: TestContext.CancellationToken);

            Assert.AreEqual(HttpMethod.Post, GetLastRequest(handler).Method);
            Assert.AreEqual("/PAPIService/REST/protected/v1/1033/100/101/protected-token/bibs", GetLastRequestUri(handler).AbsolutePath);
            AssertLastRequestQueryParameter(handler, "ImportProfileName", "profile");
            AssertLastRequestQueryParameter(handler, "WorkstationID", "77");
            Assert.AreEqual(marcXml, GetLastRequestBody(handler));
            Assert.AreEqual("application/xml", GetLastRequest(handler).Content!.Headers.ContentType!.MediaType);
            Assert.AreEqual("utf-8", GetLastRequest(handler).Content!.Headers.ContentType!.CharSet);
        }

        [TestMethod]
        public async Task BibsPostAsync_OmitsOptionalParametersWhenNotProvided()
        {
            var handler = new CapturingHttpMessageHandler(CreatePapiResponseJson());
            var client = CreateConfiguredClient(handler);
            client.Token = CreateToken();

            await client.BibsPostAsync("<collection />", cancellationToken: TestContext.CancellationToken);

            Assert.AreEqual(string.Empty, GetLastRequestUri(handler).Query);
        }

        [TestMethod]
        public async Task BibsPostAsync_WithEmptyMarcXml_Throws()
        {
            var client = CreateClient();
            await Assert.ThrowsAsync<ArgumentException>(async () => await client.BibsPostAsync(" ", cancellationToken: TestContext.CancellationToken));
        }

        [TestMethod]
        public async Task BibsPostAsync_WithInvalidWorkstationId_Throws()
        {
            var client = CreateClient();
            await Assert.ThrowsExactlyAsync<ArgumentOutOfRangeException>(async () => await client.BibsPostAsync("<collection />", workstationId: 0, cancellationToken: TestContext.CancellationToken));
        }

        private static PapiClient CreateConfiguredClient(CapturingHttpMessageHandler handler) { var client = CreateClient(handler); client.OrganizationId = 101; return client; }
        private static ProtectedToken CreateToken() => new() { AccessToken = "protected-token", AccessSecret = "protected-secret", ExpirationDate = ValidProtectedTokenExpirationDate };
    }
}
