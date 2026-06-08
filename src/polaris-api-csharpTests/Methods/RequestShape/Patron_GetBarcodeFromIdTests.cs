using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Clc.Polaris.Api.Models;
using Clc.Polaris.Api.Tests;
using Clc.Polaris.Api.Tests.TestInfrastructure;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Clc.Polaris.Api.Tests.Methods.RequestShape
{
    [TestClass]
    [UnitCategory]
    public class Patron_GetBarcodeFromIdTests : PapiClientTestBase
    {

        [TestMethod]
        public async Task Patron_GetBarcodeFromId_FormatsUrlCorrectly()
        {
            var handler = new CaptureHttpMessageHandler();
            var client = CreateUrlEncodingClient(handler);
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
