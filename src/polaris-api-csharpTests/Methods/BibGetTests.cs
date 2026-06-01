using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Clc.Polaris.Api.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Clc.Polaris.Api.Tests.Methods
{
    [TestClass]
    [TestCategory("Unit")]
    public class BibGetTests
    {
        private sealed class CaptureHttpMessageHandler : HttpMessageHandler
        {
            public HttpRequestMessage? LastRequest { get; private set; }

            protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            {
                LastRequest = request;
                var response = new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent("{\"PAPIErrorCode\":0, \"BibGetRows\": []}")
                };
                response.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
                return Task.FromResult(response);
            }
        }

        private static PapiClient CreateClient(CaptureHttpMessageHandler handler, int organizationId = 1)
        {
            var settings = new PapiSettings
            {
                AccessId = "test-access-id",
                AccessKey = "test-access-key",
                Hostname = "https://example.test",
                OrganizationId = organizationId,
                UserId = 123,
                WorkstationId = 456
            };

            var httpClient = new HttpClient(handler);
            return new PapiClient(httpClient, settings);
        }

        [TestMethod]
        public void BibGet_WithoutBranchId_UsesOrganizationIdInUrl()
        {
            var handler = new CaptureHttpMessageHandler();
            var organizationId = 42;
            var client = CreateClient(handler, organizationId);
            var bibId = 12345;

            client.BibGet(bibId);

            var expectedPath = $"/PAPIService/REST/public/v1/1033/100/{organizationId}/bib/{bibId}";
            Assert.IsNotNull(handler.LastRequest);
            Assert.AreEqual(expectedPath, handler.LastRequest!.RequestUri!.AbsolutePath);
            Assert.AreEqual(HttpMethod.Get, handler.LastRequest.Method);
        }

        [TestMethod]
        public void BibGet_WithBranchId_UsesBranchIdInUrl()
        {
            var handler = new CaptureHttpMessageHandler();
            var organizationId = 42;
            var branchId = 99;
            var client = CreateClient(handler, organizationId);
            var bibId = 12345;

            client.BibGet(bibId, branchId);

            var expectedPath = $"/PAPIService/REST/public/v1/1033/100/{branchId}/bib/{bibId}";
            Assert.IsNotNull(handler.LastRequest);
            Assert.AreEqual(expectedPath, handler.LastRequest!.RequestUri!.AbsolutePath);
            Assert.AreEqual(HttpMethod.Get, handler.LastRequest.Method);
        }
    }
}
