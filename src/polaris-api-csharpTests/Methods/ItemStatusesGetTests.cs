using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Clc.Polaris.Api.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Clc.Polaris.Api.Tests
{
    [TestClass]
    [TestCategory("Unit")]
    public class ItemStatusesGetTests
    {
        private sealed class CaptureHttpMessageHandler : HttpMessageHandler
        {
            public HttpRequestMessage? LastRequest { get; private set; }

            protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            {
                LastRequest = request;
                var response = new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent("{\"PAPIErrorCode\":0, \"ItemStatusesRows\":[]}")
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
                OrganizationId = 5,
                UserId = 123,
                WorkstationId = 456
            };

            var httpClient = new HttpClient(handler);
            return new PapiClient(httpClient, settings);
        }

        [TestMethod]
        public void ItemStatusesGet_NoBranchId_UsesOrganizationId()
        {
            var handler = new CaptureHttpMessageHandler();
            var client = CreateClient(handler);

            client.ItemStatusesGet();

            var expectedPath = $"/PAPIService/REST/public/v1/1033/100/5/itemstatuses";
            Assert.IsNotNull(handler.LastRequest);
            Assert.AreEqual(HttpMethod.Get, handler.LastRequest.Method);
            Assert.AreEqual(expectedPath, handler.LastRequest.RequestUri!.AbsolutePath);
        }

        [TestMethod]
        public void ItemStatusesGet_WithBranchId_UsesProvidedBranchId()
        {
            var handler = new CaptureHttpMessageHandler();
            var client = CreateClient(handler);
            var branchId = 7;

            client.ItemStatusesGet(branchId);

            var expectedPath = $"/PAPIService/REST/public/v1/1033/100/{branchId}/itemstatuses";
            Assert.IsNotNull(handler.LastRequest);
            Assert.AreEqual(HttpMethod.Get, handler.LastRequest.Method);
            Assert.AreEqual(expectedPath, handler.LastRequest.RequestUri!.AbsolutePath);
        }
    }
}
