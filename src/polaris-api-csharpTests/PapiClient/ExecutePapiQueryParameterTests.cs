using Clc.Polaris.Api;
using Clc.Polaris.Api.Tests;
using Clc.Polaris.Api.Models;
using Clc.Rest.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace Clc.Polaris.Api.Tests.Client
{
    [TestClass]
    [UnitCategory]
    public class ExecutePapiQueryParameterTests : PapiClientTestBase
    {
        [TestMethod]
        public async Task ExecutePapiAsync_QueryParameterWithNullValue_OmitsParameterAndHashesSentUri()
        {
            var handler = new CapturingHttpMessageHandler("{\"PAPIErrorCode\":0}");
            var client = CreateClient(handler);
            var request = new PapiRestRequest(HttpMethod.Get, "/public/v1/1033/100/1/search/bibs/keyword/KW");
            request.QueryParameters.Add("q", "harry potter");
            request.QueryParameters.Add("limit", null!);

            await ExecuteRawPapiRequestAsync(client, request);

            Assert.IsNotNull(handler.LastRequest);
            Assert.AreEqual("https://example.test/PAPIService/REST/public/v1/1033/100/1/search/bibs/keyword/KW?q=harry%20potter", handler.LastRequest!.RequestUri!.AbsoluteUri);
            var query = ParseQuery(handler.LastRequest.RequestUri.Query);
            Assert.IsFalse(query.ContainsKey("limit"));
            AssertAuthorizationHashesSentUri(handler.LastRequest, string.Empty);
        }

        [TestMethod]
        public async Task ExecutePapiAsync_QueryParameterWithEmptyStringValue_OmitsParameterAndHashesSentUri()
        {
            var handler = new CapturingHttpMessageHandler("{\"PAPIErrorCode\":0}");
            var client = CreateClient(handler);
            var request = new PapiRestRequest(HttpMethod.Get, "/public/v1/1033/100/1/search/bibs/keyword/KW");
            request.QueryParameters.Add("q", "harry potter");
            request.QueryParameters.Add("limit", string.Empty);

            await ExecuteRawPapiRequestAsync(client, request);

            Assert.IsNotNull(handler.LastRequest);
            Assert.AreEqual("https://example.test/PAPIService/REST/public/v1/1033/100/1/search/bibs/keyword/KW?q=harry%20potter", handler.LastRequest!.RequestUri!.AbsoluteUri);
            var query = ParseQuery(handler.LastRequest.RequestUri.Query);
            Assert.IsFalse(query.ContainsKey("limit"));
            AssertAuthorizationHashesSentUri(handler.LastRequest, string.Empty);
        }

        [TestMethod]
        public async Task ExecutePapiAsync_QueryParameterWithWhitespaceValue_OmitsParameterAndHashesSentUri()
        {
            var handler = new CapturingHttpMessageHandler("{\"PAPIErrorCode\":0}");
            var client = CreateClient(handler);
            var request = new PapiRestRequest(HttpMethod.Get, "/public/v1/1033/100/1/search/bibs/keyword/KW");
            request.QueryParameters.Add("q", "harry potter");
            request.QueryParameters.Add("limit", "   ");

            await ExecuteRawPapiRequestAsync(client, request);

            Assert.IsNotNull(handler.LastRequest);
            Assert.AreEqual("https://example.test/PAPIService/REST/public/v1/1033/100/1/search/bibs/keyword/KW?q=harry%20potter", handler.LastRequest!.RequestUri!.AbsoluteUri);
            var query = ParseQuery(handler.LastRequest.RequestUri.Query);
            Assert.IsFalse(query.ContainsKey("limit"));
            AssertAuthorizationHashesSentUri(handler.LastRequest, string.Empty);
        }

        [TestMethod]
        public async Task ExecutePapiAsync_QueryParameterWithBlankKey_OmitsParameterAndHashesSentUri()
        {
            var handler = new CapturingHttpMessageHandler("{\"PAPIErrorCode\":0}");
            var client = CreateClient(handler);
            var request = new PapiRestRequest(HttpMethod.Get, "/public/v1/1033/100/1/search/bibs/keyword/KW");
            request.QueryParameters.Add("q", "harry potter");
            request.QueryParameters.Add("   ", "branch:1");

            await ExecuteRawPapiRequestAsync(client, request);

            Assert.IsNotNull(handler.LastRequest);
            Assert.AreEqual("https://example.test/PAPIService/REST/public/v1/1033/100/1/search/bibs/keyword/KW?q=harry%20potter", handler.LastRequest!.RequestUri!.AbsoluteUri);
            var query = ParseQuery(handler.LastRequest.RequestUri.Query);
            Assert.AreEqual(1, query.Count);
            Assert.IsTrue(query.ContainsKey("q"));
            AssertAuthorizationHashesSentUri(handler.LastRequest, string.Empty);
        }

        [TestMethod]
        public async Task ExecutePapiAsync_OnlyIneffectiveQueryParameters_OmitsQueryStringAndHashesSentUri()
        {
            var handler = new CapturingHttpMessageHandler("{\"PAPIErrorCode\":0}");
            var client = CreateClient(handler);
            var request = new PapiRestRequest(HttpMethod.Get, "/public/v1/1033/100/1/search/bibs/keyword/KW");
            request.QueryParameters.Add(" ", "blank key");
            request.QueryParameters.Add("empty", string.Empty);
            request.QueryParameters.Add("null", null!);

            await ExecuteRawPapiRequestAsync(client, request);

            Assert.IsNotNull(handler.LastRequest);
            Assert.AreEqual("https://example.test/PAPIService/REST/public/v1/1033/100/1/search/bibs/keyword/KW", handler.LastRequest!.RequestUri!.AbsoluteUri);
            Assert.AreEqual(string.Empty, handler.LastRequest.RequestUri.Query);
            AssertAuthorizationHashesSentUri(handler.LastRequest, string.Empty);
        }

        [TestMethod]
        public async Task ExecutePapiAsync_QueryParameters_UseInvariantCultureForSentUriAndHash()
        {
            var originalCulture = CultureInfo.CurrentCulture;
            var originalUiCulture = CultureInfo.CurrentUICulture;
            try
            {
                CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("fr-FR");
                CultureInfo.CurrentUICulture = CultureInfo.GetCultureInfo("fr-FR");
                var handler = new CapturingHttpMessageHandler("{\"PAPIErrorCode\":0}");
                var client = CreateClient(handler);
                var request = new PapiRestRequest(HttpMethod.Get, "/public/v1/1033/100/1/search/bibs/keyword/KW");
                request.QueryParameters.Add("amount", 12.34m);

                await ExecuteRawPapiRequestAsync(client, request);

                Assert.IsNotNull(handler.LastRequest);
                Assert.AreEqual("https://example.test/PAPIService/REST/public/v1/1033/100/1/search/bibs/keyword/KW?amount=12.34", handler.LastRequest!.RequestUri!.AbsoluteUri);
                AssertAuthorizationHashesSentUri(handler.LastRequest, string.Empty);
            }
            finally
            {
                CultureInfo.CurrentCulture = originalCulture;
                CultureInfo.CurrentUICulture = originalUiCulture;
            }
        }

        [TestMethod]
        public async Task ExecutePapiAsync_NonEmptyQueryParameters_HashesSentUri()
        {
            var handler = new CapturingHttpMessageHandler("{\"PAPIErrorCode\":0}");
            var client = CreateClient(handler);
            var request = new PapiRestRequest(HttpMethod.Get, "/public/v1/1033/100/1/search/bibs/keyword/KW");
            request.QueryParameters.Add("q", "harry potter & stone");
            request.QueryParameters.Add("limit", "branch:1");

            await ExecuteRawPapiRequestAsync(client, request);

            Assert.IsNotNull(handler.LastRequest);
            Assert.AreEqual("https://example.test/PAPIService/REST/public/v1/1033/100/1/search/bibs/keyword/KW?q=harry%20potter%20%26%20stone&limit=branch%3A1", handler.LastRequest!.RequestUri!.AbsoluteUri);
            AssertAuthorizationHashesSentUri(handler.LastRequest, string.Empty);
        }

        [TestMethod]
        public async Task ExecutePapiAsync_ExistingQueryString_AppendsQueryParametersAndHashesSentUri()
        {
            var handler = new CapturingHttpMessageHandler("{\"PAPIErrorCode\":0}");
            var client = CreateClient(handler);
            var request = new PapiRestRequest(HttpMethod.Get, "/public/v1/1033/100/1/search/bibs/keyword/KW?existing=1");
            request.QueryParameters.Add("q", "harry potter");

            await ExecuteRawPapiRequestAsync(client, request);

            Assert.IsNotNull(handler.LastRequest);
            Assert.AreEqual("https://example.test/PAPIService/REST/public/v1/1033/100/1/search/bibs/keyword/KW?existing=1&q=harry%20potter", handler.LastRequest!.RequestUri!.AbsoluteUri);
            var query = ParseQuery(handler.LastRequest.RequestUri.Query);
            Assert.AreEqual("1", query["existing"]);
            Assert.AreEqual("harry potter", query["q"]);
            AssertAuthorizationHashesSentUri(handler.LastRequest, string.Empty);
        }
    }
}
