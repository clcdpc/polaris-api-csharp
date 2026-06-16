using System.Net;

namespace Clc.Polaris.Api.UnitTests.EndpointRequests.Holds
{
    [TestClass]
    [UnitTest]
    public class HoldRequestSuspendTests : PapiClientUnitTestBase
    {

        [TestMethod]
        public async Task UpdatePickupBranchID_EncodesBarcodeAndConstructsQuery()
        {
            var handler = new CapturingHttpMessageHandler();
            var client = CreateUrlEncodingClient(handler);
            var barcode = "AB C/+#?=";
            var requestId = 1234;
            var pickupBranchId = 5678;

            await client.UpdatePickupBranchIDAsync(barcode, requestId, pickupBranchId, "pin", userId: 888, workstationId: 999, TestContext.CancellationToken);

            var expectedPath = $"/PAPIService/REST/public/v1/1033/100/1/patron/{WebUtility.UrlEncode(barcode)}/holdrequests/{requestId}/pickupbranch";
            var expectedQuery = $"?userid=888&wsid=999&pickupbranchid={pickupBranchId}";

            Assert.IsNotNull(handler.LastRequest);
            Assert.AreEqual(HttpMethod.Put, handler.LastRequest!.Method);
            Assert.AreEqual(expectedPath, handler.LastRequest.RequestUri!.AbsolutePath);
            Assert.AreEqual(expectedQuery, handler.LastRequest.RequestUri.Query);
        }
    }
}
