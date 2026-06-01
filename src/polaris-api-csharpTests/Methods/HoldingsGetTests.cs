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
    public class HoldingsGetTests
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
                    Content = new StringContent("{\"PAPIErrorCode\":0, \"BibHoldingsGetRows\": [{\"LocationID\":\"1\", \"LocationName\":\"Main Library\", \"ItemsIn\":\"5\", \"ItemsTotal\":\"10\"}]}")
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
        public void HoldingsGet_SendsGetRequestToCorrectUrl()
        {
            var handler = new CaptureHttpMessageHandler();
            var client = CreateClient(handler);
            var bibId = 12345;

            var response = client.HoldingsGet(bibId);

            Assert.IsNotNull(handler.LastRequest);
            Assert.AreEqual(HttpMethod.Get, handler.LastRequest.Method);
            var expectedPath = $"/PAPIService/REST/public/v1/1033/100/1/bib/{bibId}/holdings";
            Assert.AreEqual(expectedPath, handler.LastRequest.RequestUri!.AbsolutePath);

            Assert.AreEqual(0, response.Data.PAPIErrorCode);
            Assert.IsNotNull(response.Data.BibHoldingsGetRows);
            Assert.AreEqual(1, response.Data.BibHoldingsGetRows.Count);
            var row = response.Data.BibHoldingsGetRows[0];
            Assert.AreEqual("1", row.LocationID);
            Assert.AreEqual("Main Library", row.LocationName);
            Assert.AreEqual("5", row.ItemsIn);
            Assert.AreEqual("10", row.ItemsTotal);
        }
    }
}
