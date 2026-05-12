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
    public class AuthenticatePatronTests
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
                    Content = new StringContent("{\"PAPIErrorCode\":0, \"AccessToken\":\"mock-token\", \"AccessSecret\":\"mock-secret\", \"PatronID\":123}")
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
        public void AuthenticatePatron_SendsPostRequestWithJsonBody()
        {
            var handler = new CaptureHttpMessageHandler();
            var client = CreateClient(handler);
            var barcode = "21945001234567";
            var password = "mypassword";

            var response = client.AuthenticatePatron(barcode, password);

            Assert.IsNotNull(handler.LastRequest);
            Assert.AreEqual(HttpMethod.Post, handler.LastRequest.Method);
            var expectedPath = "/PAPIService/REST/public/v1/1033/100/1/authenticator/patron";
            Assert.AreEqual(expectedPath, handler.LastRequest.RequestUri!.AbsolutePath);

            Assert.IsNotNull(handler.RequestContent);
            Assert.IsTrue(handler.RequestContent.Contains($"\"Barcode\":\"{barcode}\"") || handler.RequestContent.Contains($"\"barcode\":\"{barcode}\""), "Body should contain barcode");
            Assert.IsTrue(handler.RequestContent.Contains($"\"Password\":\"{password}\"") || handler.RequestContent.Contains($"\"password\":\"{password}\""), "Body should contain password");

            Assert.AreEqual(0, response.Data.PAPIErrorCode);
            Assert.AreEqual("mock-token", response.Data.AccessToken);
            Assert.AreEqual("mock-secret", response.Data.AccessSecret);
            Assert.AreEqual(123, response.Data.PatronID);
        }
    }
}
