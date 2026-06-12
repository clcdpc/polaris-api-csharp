using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Clc.Polaris.Api.Configuration;
using Clc.Polaris.Api.Models;
using Clc.Polaris.Api.Tests.TestInfrastructure;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Clc.Polaris.Api.Tests
{
    [TestClass]
    [UnitTest]
    public sealed class ItemUpdateBarcodeTests : PapiClientTestBase
    {
        [TestMethod]
        public async Task ItemUpdateBarcodeAsync_WhenUsingOldBarcode_EncodesBarcodeAndAddsIsBarcodeQueryParameter()
        {
            var handler = new RecordingHttpMessageHandler();
            var papi = CreateClient(handler);
            var oldBarcode = "OLD/BARCODE";

            await papi.ItemUpdateBarcodeAsync("NEW-BARCODE", oldBarcode: oldBarcode, cancellationToken: TestContext.CancellationToken);

            var requestUri = handler.Request?.RequestUri
                ?? throw new AssertFailedException("Expected an HTTP request to be sent.");

            Assert.Contains($"/cataloging/items/{WebUtility.UrlEncode(oldBarcode)}/barcode", requestUri.AbsoluteUri);
            Assert.Contains("wsid=9", requestUri.Query);
            Assert.Contains("isBarcode=1", requestUri.Query);

            Assert.IsFalse(
                requestUri.AbsoluteUri.Contains("/cataloging/items/OLD/BARCODE/barcode", StringComparison.Ordinal),
                "Old barcode should be URL encoded before being inserted into the route.");

            Assert.IsFalse(
                requestUri.AbsoluteUri.Contains(ProtectedToken.Placeholder, StringComparison.Ordinal),
                "ProtectedToken.Placeholder should be replaced before the request is sent.");
        }

        [TestMethod]
        public async Task ItemUpdateBarcodeAsync_WhenUsingItemRecordId_DoesNotAddIsBarcodeQueryParameter()
        {
            var handler = new RecordingHttpMessageHandler();
            var papi = CreateClient(handler);

            await papi.ItemUpdateBarcodeAsync("NEW-BARCODE", itemRecordId: 12345, oldBarcode: "OLD/BARCODE", cancellationToken: TestContext.CancellationToken);

            var requestUri = handler.Request?.RequestUri
                ?? throw new AssertFailedException("Expected an HTTP request to be sent.");

            Assert.Contains("/cataloging/items/12345/barcode", requestUri.AbsoluteUri);

            Assert.IsFalse(
                requestUri.Query.Contains("isBarcode=", StringComparison.Ordinal),
                "isBarcode should only be sent when the item identifier is an old barcode.");
        }

        [TestMethod]
        public async Task ItemUpdateBarcodeAsync_WhenItemRecordIdAndOldBarcodeAreMissing_ThrowsBeforeSendingRequest()
        {
            var handler = new RecordingHttpMessageHandler();
            var papi = CreateClient(handler);

            await Assert.ThrowsExactlyAsync<ArgumentException>(
                () => papi.ItemUpdateBarcodeAsync("NEW-BARCODE", cancellationToken: TestContext.CancellationToken));

            Assert.AreEqual(0, handler.CallCount);
        }

        [TestMethod]
        public async Task ItemUpdateBarcodeAsync_WhenNewBarcodeIsWhitespace_ThrowsBeforeSendingRequest()
        {
            var handler = new RecordingHttpMessageHandler();
            var papi = CreateClient(handler);

            await Assert.ThrowsExactlyAsync<ArgumentException>(
                () => papi.ItemUpdateBarcodeAsync(" ", itemRecordId: 12345, cancellationToken: TestContext.CancellationToken));

            Assert.AreEqual(0, handler.CallCount);
        }

        private static PapiClient CreateClient(RecordingHttpMessageHandler handler)
        {
            var settings = new PapiSettings
            {
                Hostname = "https://example.test",
                AccessId = "access-id",
                AccessKey = "access-key",
                OrganizationId = 7,
                UserId = 8,
                WorkstationId = 9
            };

            return new PapiClient(new HttpClient(handler), settings)
            {
                Token = new ProtectedToken
                {
                    AccessToken = "protected-token",
                    AccessSecret = "protected-secret",
                    ExpirationDate = ValidProtectedTokenExpirationDate
                },
                UseProtectedTokenCache = false
            };
        }

        private sealed class RecordingHttpMessageHandler : HttpMessageHandler
        {
            public HttpRequestMessage? Request { get; private set; }
            public int CallCount { get; private set; }

            protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            {
                Request = request;
                CallCount++;

                return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(CreateEmptyJsonObject())
                });
            }
        }

        public TestContext TestContext { get; set; }
    }
}