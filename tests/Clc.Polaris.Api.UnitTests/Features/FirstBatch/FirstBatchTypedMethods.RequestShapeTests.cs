using System.Net;
using Clc.Polaris.Api.Models;

namespace Clc.Polaris.Api.UnitTests.Methods.RequestShape
{
    [TestClass]
    [UnitTest]
    public class FirstBatchTypedMethodsRequestShapeTests : PapiClientUnitTestBase
    {
        [TestMethod]
        public async Task PublicGetMethods_BuildExpectedPathsAndQueries()
        {
            var handler = new CapturingHttpMessageHandler(CreatePapiResponseJson());
            var client = CreateClient(handler);

            await client.SortOptionsGetAsync(branchId: 44, TestContext.CancellationToken);
            Assert.AreEqual(HttpMethod.Get, handler.LastRequest!.Method);
            Assert.AreEqual("/PAPIService/REST/public/v1/1033/100/44/sortoptions", handler.LastRequest.RequestUri!.AbsolutePath);

            await client.SysHoldStatusesGetAsync(branchId: 45, TestContext.CancellationToken);
            Assert.AreEqual("/PAPIService/REST/public/v1/1033/100/45/sysholdstatuses", handler.LastRequest!.RequestUri!.AbsolutePath);

            await client.HeadingsSearchAsync(SearchQualifiers.AU, 10, 2, startPoint: "twain", noTransaction: 1, branchId: 46, TestContext.CancellationToken);
            Assert.AreEqual("/PAPIService/REST/public/v1/1033/100/46/search/headings/AU", handler.LastRequest!.RequestUri!.AbsolutePath);
            AssertLastRequestQueryParameter(handler, "startpoint", "twain");
            AssertLastRequestQueryParameter(handler, "numterms", "10");
            AssertLastRequestQueryParameter(handler, "preferredpos", "2");
            AssertLastRequestQueryParameter(handler, "notran", "1");

            await client.MultipartGetAsync(123, 456, pickupLocationId: 789, branchId: 47, TestContext.CancellationToken);
            Assert.AreEqual("/PAPIService/REST/public/v1/1033/100/47/bib/123/multiparts", handler.LastRequest!.RequestUri!.AbsolutePath);
            AssertLastRequestQueryParameter(handler, "PatronID", "456");
            AssertLastRequestQueryParameter(handler, "PickupLocID", "789");

            await client.BibGetByTypeV2Async("abc", branchId: 48, cancellationToken: TestContext.CancellationToken);
            Assert.AreEqual("/PAPIService/REST/public/v2/1033/100/48/bib/abc", handler.LastRequest!.RequestUri!.AbsolutePath);
            AssertLastRequestQueryParameter(handler, "type", "barcode");
        }

        [TestMethod]
        public async Task ILLRequestCancel_EncodesBarcode_AllowsZeroAndUsesDefaultWsidUserid()
        {
            var handler = new CapturingHttpMessageHandler(CreatePapiResponseJson());
            var client = CreateUrlEncodingClient(handler);
            var barcode = "AB C/+#?=";

            await client.ILLRequestCancelAsync(barcode, 0, "pin", cancellationToken: TestContext.CancellationToken);

            Assert.AreEqual(HttpMethod.Put, handler.LastRequest!.Method);
            Assert.AreEqual($"/PAPIService/REST/public/v1/1033/100/1/patron/{WebUtility.UrlEncode(barcode)}/illrequests/0/cancelled", handler.LastRequest.RequestUri!.AbsolutePath);
            AssertLastRequestQueryParameter(handler, "wsid", "456");
            AssertLastRequestQueryParameter(handler, "userid", "123");
        }

        [TestMethod]
        public async Task ProtectedMethods_UseProtectedTokenPipelineAndExpectedQueries()
        {
            var handler = new ProtectedTokenHttpMessageHandler();
            var client = CreateProtectedClient(handler);

            await client.SAMobilePhoneCarriersGetAsync(cancellationToken: TestContext.CancellationToken);
            await client.RequestsUpdateStatusAsync(999, RequestStatusAction.Return, itemId: 888, cancellationToken: TestContext.CancellationToken);
            await client.RequestsUpdateStatusAsync(999, RequestStatusAction.Deny, denyReason: 7, cancellationToken: TestContext.CancellationToken);

            Assert.AreEqual(1, handler.AuthenticationRequestCount);
            Assert.IsTrue(handler.NonAuthenticationRequestCount >= 3);
            Assert.Contains("/protected/v1/1033/100/1/protected-token/sysadmin/mobilephonecarriers", handler.RequestPaths[1]);
            var returnRequest = handler.CapturedRequests.Single(request => request.AbsoluteUri.Contains("action=return", StringComparison.Ordinal));
            Assert.Contains("/protected/v1/1033/100/1/protected-token/circulation/requests/999/status", returnRequest.Path);
            Assert.Contains("itemid=888", returnRequest.AbsoluteUri);
            var denyRequest = handler.CapturedRequests.Single(request => request.AbsoluteUri.Contains("action=deny", StringComparison.Ordinal));
            Assert.Contains("denyreason=7", denyRequest.AbsoluteUri);
        }

        [TestMethod]
        public async Task Validation_RejectsInvalidInputs()
        {
            var client = CreateClient(new CapturingHttpMessageHandler(CreatePapiResponseJson()));

            await Assert.ThrowsExactlyAsync<ArgumentException>(() => client.BibGetByTypeV2Async(" ", cancellationToken: TestContext.CancellationToken));
            await Assert.ThrowsExactlyAsync<ArgumentOutOfRangeException>(() => client.MultipartGetAsync(0, 1, cancellationToken: TestContext.CancellationToken));
            await Assert.ThrowsExactlyAsync<ArgumentOutOfRangeException>(() => client.HeadingsSearchAsync(SearchQualifiers.KW, 1, 1, cancellationToken: TestContext.CancellationToken));
            await Assert.ThrowsExactlyAsync<ArgumentException>(() => client.RequestsUpdateStatusAsync(1, RequestStatusAction.Locate, cancellationToken: TestContext.CancellationToken));
            await Assert.ThrowsExactlyAsync<ArgumentException>(() => client.RequestsUpdateStatusAsync(1, RequestStatusAction.Deny, cancellationToken: TestContext.CancellationToken));
            await Assert.ThrowsExactlyAsync<ArgumentOutOfRangeException>(() => client.ILLRequestCancelAsync("b", -1, cancellationToken: TestContext.CancellationToken));
        }
    }
}
