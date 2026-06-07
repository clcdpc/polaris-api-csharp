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
    public class RequestShapePatronValidateTests : RequestShapeTestBase
    {
                [TestMethod]
                public async Task PatronValidate_NormalBarcode_PreservesBarcodePath()
                {
                    var handler = new CapturingHttpMessageHandler();
                    var client = CreateRequestShapeClient(handler);
                    var barcode = "21945001234567";
        
                    await client.PatronValidateAsync(barcode, "pin");
        
                    var expectedPath = $"/PAPIService/REST/public/v1/1033/100/1/patron/{WebUtility.UrlEncode(barcode)}";
                    Assert.IsNotNull(handler.LastRequest);
                    Assert.AreEqual(expectedPath, handler.LastRequest!.RequestUri!.AbsolutePath);
                }

                [TestMethod]
                public async Task PatronValidate_SpecialCharacters_EncodesBarcodePathSegment()
                {
                    var handler = new CapturingHttpMessageHandler();
                    var client = CreateRequestShapeClient(handler);
                    var barcode = "AB C/+#?=";
        
                    await client.PatronValidateAsync(barcode, "pin");
        
                    var expectedPath = $"/PAPIService/REST/public/v1/1033/100/1/patron/{WebUtility.UrlEncode(barcode)}";
                    Assert.IsNotNull(handler.LastRequest);
                    Assert.AreEqual(expectedPath, handler.LastRequest!.RequestUri!.AbsolutePath);
                }
    }
}
