using Clc.Polaris.Api;
using Clc.Polaris.Api.Tests;
using Clc.Polaris.Api.Models;
using Clc.Rest.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace Clc.Polaris.Api.Tests.Methods.RequestShape
{
    [TestClass]
    [UnitCategory]
    public class PatronUpdateUserNameTests : PapiClientTestBase
    {
        [TestMethod]
        public async Task PatronUpdateUserName_EncodesBarcodeAndNewUsername()
        {
            var handler = new CapturingHttpMessageHandler();
            var client = CreateClient(handler);
            var barcode = "AB C/+#?=";
            var newUsername = "new user+/name?=";

            await client.PatronUpdateUserNameAsync(barcode, newUsername, "pin");

            var expectedPath = $"/PAPIService/REST/public/v1/1033/100/1/patron/{WebUtility.UrlEncode(barcode)}/username/{WebUtility.UrlEncode(newUsername)}";
            Assert.IsNotNull(handler.LastRequest);
            Assert.AreEqual(expectedPath, handler.LastRequest!.RequestUri!.AbsolutePath);
        }
    }
}
