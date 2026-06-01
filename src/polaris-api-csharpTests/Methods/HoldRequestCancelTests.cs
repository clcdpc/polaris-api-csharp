using Clc.Polaris.Api.Configuration;
using Clc.Polaris.Api.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Clc.Polaris.Api.Tests
{
    [TestClass]
    [TestCategory("Unit")]
    public class HoldRequestCancelTests
    {
        private sealed class CaptureHttpMessageHandler : HttpMessageHandler
        {
            public HttpRequestMessage? LastRequest { get; private set; }
            public string? RequestContent { get; private set; }

            protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            {
                LastRequest = request;
                if (request.Content != null)
                {
                    RequestContent = await request.Content.ReadAsStringAsync(cancellationToken);
                }
                var response = new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent("{\"PAPIErrorCode\":0}")
                };
                response.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
                return response;
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
        public void HoldRequestCancel_SendsPutRequestWithCorrectUrlAndQueryParameters()
        {
            var handler = new CaptureHttpMessageHandler();
            var client = CreateClient(handler);
            var barcode = "21945001234567";
            var requestId = 1234;
            var password = "mypassword";
            var userId = 888;
            var workstationId = 999;

            client.HoldRequestCancel(barcode, requestId, password, userId, workstationId);

            Assert.IsNotNull(handler.LastRequest);
            Assert.AreEqual(HttpMethod.Put, handler.LastRequest.Method);
            var expectedPath = $"/PAPIService/REST/public/v1/1033/100/1/patron/{barcode}/holdrequests/{requestId}/cancelled";
            Assert.AreEqual(expectedPath, handler.LastRequest.RequestUri!.AbsolutePath);
            var expectedQuery = $"?wsid={workstationId}&userid={userId}";
            Assert.AreEqual(expectedQuery, handler.LastRequest.RequestUri.Query);
        }

        [TestMethod]
        public void HoldRequestCancel_UsesDefaultWorkstationAndUserIdIfNull()
        {
            var handler = new CaptureHttpMessageHandler();
            var client = CreateClient(handler);
            var barcode = "21945001234567";
            var requestId = 1234;
            var password = "mypassword";

            client.HoldRequestCancel(barcode, requestId, password, null, null);

            Assert.IsNotNull(handler.LastRequest);
            Assert.AreEqual(HttpMethod.Put, handler.LastRequest.Method);
            var expectedPath = $"/PAPIService/REST/public/v1/1033/100/1/patron/{barcode}/holdrequests/{requestId}/cancelled";
            Assert.AreEqual(expectedPath, handler.LastRequest.RequestUri!.AbsolutePath);
            var expectedQuery = $"?wsid={client.WorkstationId}&userid={client.UserId}";
            Assert.AreEqual(expectedQuery, handler.LastRequest.RequestUri.Query);
        }

        [TestMethod]
        public void HoldRequestCancel_SendsPasswordInPapiRestRequest()
        {
            var handler = new CaptureHttpMessageHandler();
            var client = CreateClient(handler);
            var barcode = "21945001234567";
            var requestId = 1234;
            var password = "mypassword";

            client.HoldRequestCancel(barcode, requestId, password);

            Assert.IsNotNull(handler.LastRequest);
            Assert.AreEqual(HttpMethod.Put, handler.LastRequest.Method);
            Assert.IsTrue(handler.LastRequest.Headers.Contains("PolarisDate"));
            Assert.IsTrue(handler.LastRequest.Headers.Contains("Authorization"));
        }
    }
}
