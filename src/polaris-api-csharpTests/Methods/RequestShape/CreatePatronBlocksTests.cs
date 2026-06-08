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
    public class CreatePatronBlocksTests : PapiClientTestBase
    {
        [TestMethod]
        public async Task CreatePatronBlocks_EncodesBarcodeInProtectedRoute_PreservesTokenPath()
        {
            var handler = new CaptureHttpMessageHandler();
            var client = CreateClient(handler);
            var barcode = "AB C/+#?=";
            client.Token = new ProtectedToken
            {
                AccessToken = "token-segment",
                AccessSecret = "token-secret",
                ExpirationDate = DateTime.Now.AddHours(1)
            };

            await client.CreatePatronBlocksAsync(barcode, BlockType.FreeText, "note", userId: 888, workstationId: 999);

            var expectedPath = $"/PAPIService/REST/protected/v1/1033/100/1/token-segment/patron/{WebUtility.UrlEncode(barcode)}/blocks";
            var expectedQuery = "?wsid=999&userid=888";

            Assert.IsNotNull(handler.LastRequest);
            Assert.AreEqual(expectedPath, handler.LastRequest!.RequestUri!.AbsolutePath);
            Assert.AreEqual(expectedQuery, handler.LastRequest.RequestUri.Query);
        }
    }
}
