using System.Xml.Serialization;

namespace Clc.Polaris.Api.UnitTests.Methods.RequestShape
{
    [TestClass]
    [UnitTest]
    public sealed class FirstBatchPapiMethodsRequestShapeTests : PapiClientUnitTestBase
    {
        [TestMethod]
        public async Task BibGetByTypeV2Async_SendsExpectedRequest()
        {
            var handler = new CapturingHttpMessageHandler(CreatePapiResponseJson());
            var client = CreateConfiguredClient(handler);

            await client.BibGetByTypeV2Async("abc 123", branchId: 88, cancellationToken: TestContext.CancellationToken);

            Assert.AreEqual(HttpMethod.Get, GetLastRequest(handler).Method);
            Assert.AreEqual("/PAPIService/REST/public/v2/1033/100/88/bib/abc+123", GetLastRequestUri(handler).AbsolutePath);
            AssertLastRequestQueryParameter(handler, "type", "barcode");
        }

        [TestMethod]
        public async Task MultipartGetAsync_SendsExpectedRequestAndOptionalPickupLocation()
        {
            var handler = new CapturingHttpMessageHandler(CreatePapiResponseJson());
            var client = CreateConfiguredClient(handler);

            await client.MultipartGetAsync(123, 456, pickupLocationId: 789, cancellationToken: TestContext.CancellationToken);

            Assert.AreEqual(HttpMethod.Get, GetLastRequest(handler).Method);
            Assert.AreEqual("/PAPIService/REST/public/v1/1033/100/101/bib/123/multiparts", GetLastRequestUri(handler).AbsolutePath);
            AssertLastRequestQueryParameter(handler, "PatronID", "456");
            AssertLastRequestQueryParameter(handler, "PickupLocID", "789");
        }

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

        [TestMethod]
        [DataRow("sortoptions")]
        [DataRow("sysholdstatuses")]
        public async Task PublicLookupMethods_SendExpectedGetRoutes(string endpoint)
        {
            var handler = new CapturingHttpMessageHandler(CreatePapiResponseJson());
            var client = CreateConfiguredClient(handler);

            if (endpoint == "sortoptions")
            {
                await client.SortOptionsGetAsync(branchId: 88, cancellationToken: TestContext.CancellationToken);
            }
            else
            {
                await client.SysHoldStatusesGetAsync(branchId: 88, cancellationToken: TestContext.CancellationToken);
            }

            Assert.AreEqual(HttpMethod.Get, GetLastRequest(handler).Method);
            Assert.AreEqual($"/PAPIService/REST/public/v1/1033/100/88/{endpoint}", GetLastRequestUri(handler).AbsolutePath);
        }

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

        [TestMethod]
        public async Task RequestsUpdateStatusAsync_SendsExpectedProtectedPutRequest()
        {
            var handler = new CapturingHttpMessageHandler(CreatePapiResponseJson());
            var client = CreateConfiguredClient(handler);
            client.Token = CreateToken();

            await client.RequestsUpdateStatusAsync(123, RequestStatusAction.AskMeLater, itemId: 456, cancellationToken: TestContext.CancellationToken);

            Assert.AreEqual(HttpMethod.Put, GetLastRequest(handler).Method);
            Assert.AreEqual("/PAPIService/REST/protected/v1/1033/100/101/protected-token/circulation/requests/123/status", GetLastRequestUri(handler).AbsolutePath);
            AssertLastRequestQueryParameter(handler, "action", "askmelater");
            AssertLastRequestQueryParameter(handler, "itemid", "456");
        }

        [TestMethod]
        public async Task ILLRequestCancelAsync_SendsExpectedPutRequest()
        {
            var handler = new CapturingHttpMessageHandler(CreatePapiResponseJson());
            var client = CreateConfiguredClient(handler);

            await client.ILLRequestCancelAsync("abc 123", 0, cancellationToken: TestContext.CancellationToken);

            Assert.AreEqual(HttpMethod.Put, GetLastRequest(handler).Method);
            Assert.AreEqual("/PAPIService/REST/public/v1/1033/100/101/patron/abc+123/illrequests/0/cancelled", GetLastRequestUri(handler).AbsolutePath);
            AssertLastRequestQueryParameter(handler, "wsid", "22");
            AssertLastRequestQueryParameter(handler, "userid", "11");
        }

        [TestMethod]
        public async Task FirstBatchValidation_Throws()
        {
            var client = CreateClient();

            await Assert.ThrowsAsync<ArgumentException>(async () => await client.BibGetByTypeV2Async("", cancellationToken: TestContext.CancellationToken));
            await Assert.ThrowsExactlyAsync<ArgumentOutOfRangeException>(async () => await client.MultipartGetAsync(0, 1, cancellationToken: TestContext.CancellationToken));
            await Assert.ThrowsExactlyAsync<ArgumentOutOfRangeException>(async () => await client.ILLRequestCancelAsync("b", -1, cancellationToken: TestContext.CancellationToken));
            await Assert.ThrowsAsync<ArgumentException>(async () => await client.RequestsUpdateStatusAsync(1, RequestStatusAction.Deny, cancellationToken: TestContext.CancellationToken));
            await Assert.ThrowsAsync<ArgumentException>(async () => await client.RequestsUpdateStatusAsync(1, RequestStatusAction.Return, cancellationToken: TestContext.CancellationToken));
        }

        private static PapiClient CreateConfiguredClient(CapturingHttpMessageHandler handler)
        {
            var client = CreateClient(handler);
            client.OrganizationId = 101;
            client.UserId = 11;
            client.WorkstationId = 22;
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
