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
    public class BibSearchTests
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
        public void BibKeywordSearch_WithDefaultParameters_UsesOrganizationIdAndDefaults()
        {
            var handler = new CaptureHttpMessageHandler();
            var client = CreateClient(handler);
            var keyword = "harry potter";

            client.BibKeywordSearch(keyword);

            Assert.IsNotNull(handler.LastRequest);
            var expectedPathAndQuery = $"/PAPIService/REST/public/v1/1033/100/1/search/bibs/keyword/KW?q={WebUtility.UrlEncode(keyword)}&sort=MP&page=1&bibsperpage=10&limit=";

            Assert.AreEqual(expectedPathAndQuery, handler.LastRequest!.RequestUri!.PathAndQuery);
        }

        [TestMethod]
        public void BibKeywordSearch_WithExplicitParameters_MapsCorrectly()
        {
            var handler = new CaptureHttpMessageHandler();
            var client = CreateClient(handler);
            var keyword = "dogs";
            var branchId = 7;
            var page = 3;
            var pageSize = 25;
            var sortBy = SearchSortOptions.PD;

            client.BibKeywordSearch(keyword, branchId, page, pageSize, sortBy);

            Assert.IsNotNull(handler.LastRequest);
            var expectedPathAndQuery = $"/PAPIService/REST/public/v1/1033/100/7/search/bibs/keyword/KW?q={WebUtility.UrlEncode(keyword)}&sort=PD&page=3&bibsperpage=25&limit=";

            Assert.AreEqual(expectedPathAndQuery, handler.LastRequest!.RequestUri!.PathAndQuery);
        }

        [TestMethod]
        public void BibKeywordSearch_WithSpecialCharacters_EncodesKeywordCorrectly()
        {
            var handler = new CaptureHttpMessageHandler();
            var client = CreateClient(handler);
            var keyword = "C# programming + & ?";

            client.BibKeywordSearch(keyword);

            Assert.IsNotNull(handler.LastRequest);
            var expectedPathAndQuery = $"/PAPIService/REST/public/v1/1033/100/1/search/bibs/keyword/KW?q={WebUtility.UrlEncode(keyword)}&sort=MP&page=1&bibsperpage=10&limit=";

            Assert.AreEqual(expectedPathAndQuery, handler.LastRequest!.RequestUri!.PathAndQuery);
        }
    }
}