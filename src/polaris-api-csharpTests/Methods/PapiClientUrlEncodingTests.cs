using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Clc.Polaris.Api;
using Clc.Polaris.Api.Configuration;
using Clc.Polaris.Api.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Clc.Polaris.Api.Tests
{
    [TestClass]
    [TestCategory("Unit")]
    public class PapiClientUrlEncodingTests
    {
        private sealed class CaptureHttpMessageHandler : HttpMessageHandler
        {
            public HttpRequestMessage? LastRequest { get; private set; }

            protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            {
                LastRequest = request;
                var response = new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent("{\"PAPIErrorCode\":0}")
                };
                response.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
                return Task.FromResult(response);
            }
        }

        private static PapiClient CreateClient(CaptureHttpMessageHandler handler)
        {
            var settings = new PapiSettings
            {
                AccessId = "test-access-id",
                AccessKey = "test-access-key",
                Hostname = "https://example.test",
                OrganizationId = 1,
                UserId = 123,
                WorkstationId = 456
            };

            var httpClient = new HttpClient(handler);
            return new PapiClient(httpClient, settings);
        }

        [TestMethod]
        public void PatronValidate_NormalBarcode_PreservesBarcodePath()
        {
            var handler = new CaptureHttpMessageHandler();
            var client = CreateClient(handler);
            var barcode = "21945001234567";

            client.PatronValidate(barcode, "pin");

            var expectedPath = $"/PAPIService/REST/public/v1/1033/100/1/patron/{WebUtility.UrlEncode(barcode)}";
            Assert.IsNotNull(handler.LastRequest);
            Assert.AreEqual(expectedPath, handler.LastRequest!.RequestUri!.AbsolutePath);
        }

        [TestMethod]
        public void PatronValidate_SpecialCharacters_EncodesBarcodePathSegment()
        {
            var handler = new CaptureHttpMessageHandler();
            var client = CreateClient(handler);
            var barcode = "AB C/+#?=";

            client.PatronValidate(barcode, "pin");

            var expectedPath = $"/PAPIService/REST/public/v1/1033/100/1/patron/{WebUtility.UrlEncode(barcode)}";
            Assert.IsNotNull(handler.LastRequest);
            Assert.AreEqual(expectedPath, handler.LastRequest!.RequestUri!.AbsolutePath);
        }

        [TestMethod]
        public void PatronUpdateUserName_EncodesBarcodeAndNewUsername()
        {
            var handler = new CaptureHttpMessageHandler();
            var client = CreateClient(handler);
            var barcode = "AB C/+#?=";
            var newUsername = "new user+/name?=";

            client.PatronUpdateUserName(barcode, newUsername, "pin");

            var expectedPath = $"/PAPIService/REST/public/v1/1033/100/1/patron/{WebUtility.UrlEncode(barcode)}/username/{WebUtility.UrlEncode(newUsername)}";
            Assert.IsNotNull(handler.LastRequest);
            Assert.AreEqual(expectedPath, handler.LastRequest!.RequestUri!.AbsolutePath);
        }

        [TestMethod]
        public void HoldRequestCancel_EncodesBarcode_PreservesWsidAndUseridQueryStringValues()
        {
            var handler = new CaptureHttpMessageHandler();
            var client = CreateClient(handler);
            var barcode = "AB C/+#?=";

            client.HoldRequestCancel(barcode, 9876, "pin", userId: 888, workstationId: 999);

            var expectedPath = $"/PAPIService/REST/public/v1/1033/100/1/patron/{WebUtility.UrlEncode(barcode)}/holdrequests/9876/cancelled";
            var expectedQuery = "?wsid=999&userid=888";

            Assert.IsNotNull(handler.LastRequest);
            Assert.AreEqual(expectedPath, handler.LastRequest!.RequestUri!.AbsolutePath);
            Assert.AreEqual(expectedQuery, handler.LastRequest.RequestUri.Query);
        }

        [TestMethod]
        public void CreatePatronBlocks_EncodesBarcodeInProtectedRoute_PreservesTokenPath()
        {
            var handler = new CaptureHttpMessageHandler();
            var client = CreateClient(handler);
            var barcode = "AB C/+#?=";
            client.Token = new ProtectedToken
            {
                AccessToken = "token-segment",
                AccessSecret = "token-secret",
                ExpirationDate = DateTime.UtcNow.AddHours(1)
            };

            client.CreatePatronBlocks(barcode, BlockType.FreeText, "note", userId: 888, workstationId: 999);

            var expectedPath = $"/PAPIService/REST/protected/v1/1033/100/1/token-segment/patron/{WebUtility.UrlEncode(barcode)}/blocks";
            var expectedQuery = "?wsid=999&userid=888";

            Assert.IsNotNull(handler.LastRequest);
            Assert.AreEqual(expectedPath, handler.LastRequest!.RequestUri!.AbsolutePath);
            Assert.AreEqual(expectedQuery, handler.LastRequest.RequestUri.Query);
        }

        [TestMethod]
        public void NotificationQueueGet_RequestsCorrectUrl()
        {
            var handler = new CaptureHttpMessageHandler();
            var client = CreateClient(handler);
            client.Token = new ProtectedToken
            {
                AccessToken = "token-segment",
                AccessSecret = "token-secret",
                ExpirationDate = DateTime.UtcNow.AddHours(1)
            };

            client.NotificationQueueGet(1);

            var expectedPath = $"/PAPIService/REST/protected/v1/1033/24/1/token-segment/notification/";
            Assert.IsNotNull(handler.LastRequest);
            Assert.AreEqual(expectedPath, handler.LastRequest!.RequestUri!.AbsolutePath);
        }
    }
}
