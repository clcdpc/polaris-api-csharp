using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Clc.Polaris.Api;
using Clc.Polaris.Api.Configuration;
using Clc.Polaris.Api.Models;
using Clc.Rest.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Clc.Polaris.Api.Tests
{
    [TestClass]
    [UnitCategory]
    public class RequestShapeHoldRequestCancelTests : RequestShapeTestBase
    {
                [TestMethod]
                public async Task HoldRequestCancel_EncodesBarcode_PreservesWsidAndUseridQueryStringValues()
                {
                    var handler = new CapturingHttpMessageHandler();
                    var client = CreateRequestShapeClient(handler);
                    var barcode = "AB C/+#?=";
        
                    await client.HoldRequestCancelAsync(barcode, 9876, "pin", userId: 888, workstationId: 999);
        
                    var expectedPath = $"/PAPIService/REST/public/v1/1033/100/1/patron/{WebUtility.UrlEncode(barcode)}/holdrequests/9876/cancelled";
                    var expectedQuery = "?wsid=999&userid=888";
        
                    Assert.IsNotNull(handler.LastRequest);
                    Assert.AreEqual(expectedPath, handler.LastRequest!.RequestUri!.AbsolutePath);
                    Assert.AreEqual(expectedQuery, handler.LastRequest.RequestUri.Query);
                }
    }
}
