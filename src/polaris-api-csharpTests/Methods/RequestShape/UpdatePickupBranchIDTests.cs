using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Clc.Polaris.Api;
using Clc.Polaris.Api.Models;
using Clc.Polaris.Api.Tests;
using Clc.Polaris.Api.Tests.TestInfrastructure;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Clc.Polaris.Api.Tests.Methods.RequestShape
{
    [TestClass]
    [UnitCategory]
    public class UpdatePickupBranchIDTests : PapiClientTestBase
    {
        [TestMethod]
        public async Task UpdatePickupBranchID_EncodesBarcodeAndConstructsQuery()
        {
            var handler = new CaptureHttpMessageHandler();
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
