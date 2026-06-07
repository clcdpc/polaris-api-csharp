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
    public class RequestShapePatron_GetBarcodeFromIdTests : RequestShapeTestBase
    {
                [TestMethod]
                public async Task Patron_GetBarcodeFromId_FormatsUrlCorrectly()
                {
                    var handler = new CapturingHttpMessageHandler();
                    var client = CreateRequestShapeClient(handler);
                    var patronId = 12345;
                    client.Token = new ProtectedToken
                    {
                        AccessToken = "token-segment",
                        AccessSecret = "token-secret",
                        ExpirationDate = DateTime.Now.AddHours(1)
                    };
        
                    await client.Patron_GetBarcodeFromIdAsync(patronId);
        
                    var expectedPath = "/PAPIService/REST/protected/v1/1033/100/1/token-segment/patron/barcode";
                    var expectedQuery = $"?patronid={patronId}";
        
                    Assert.IsNotNull(handler.LastRequest);
                    Assert.AreEqual(expectedPath, handler.LastRequest!.RequestUri!.AbsolutePath);
                    Assert.AreEqual(expectedQuery, handler.LastRequest.RequestUri.Query);
                }
    }
}
