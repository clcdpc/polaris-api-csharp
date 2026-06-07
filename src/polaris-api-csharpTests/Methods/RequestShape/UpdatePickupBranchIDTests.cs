using Clc.Polaris.Api;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace Clc.Polaris.Api.Tests
{
    [TestClass]
    [UnitCategory]
    public class UpdatePickupBranchIDTests : PapiClientTestBase
    {
        [TestMethod]
        public async Task UpdatePickupBranchID_EncodesBarcodeAndConstructsQuery()
        {
            var handler = new CapturingHttpMessageHandler();
            var client = CreateClient(handler);
            var barcode = "AB C/+#?=";
            var requestId = 1234;
            var pickupBranchId = 5678;

            await client.UpdatePickupBranchIDAsync(barcode, requestId, pickupBranchId, "pin", userId: 888, workstationId: 999);

            var expectedPath = $"/PAPIService/REST/public/v1/1033/100/1/patron/{WebUtility.UrlEncode(barcode)}/holdrequests/{requestId}/pickupbranch";
            var expectedQuery = $"?userid=888&wsid=999&pickupbranchid={pickupBranchId}";

            Assert.IsNotNull(handler.LastRequest);
            Assert.AreEqual(HttpMethod.Put, handler.LastRequest!.Method);
            Assert.AreEqual(expectedPath, handler.LastRequest.RequestUri!.AbsolutePath);
            Assert.AreEqual(expectedQuery, handler.LastRequest.RequestUri.Query);
        }
    }
}
