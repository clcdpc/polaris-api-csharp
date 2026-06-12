using System.Net.Http;
using System.Threading.Tasks;
using Clc.Polaris.Api;
using Clc.Polaris.Api.Models;
using Clc.Polaris.Api.Tests;
using Clc.Polaris.Api.Tests.TestInfrastructure;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Clc.Polaris.Api.Tests.Methods.RequestShape
{
    [TestClass]
    [UnitTest]
    public class BibSearchTests : PapiClientTestBase
    {
        [TestMethod]
        public async Task BibSearch_RequestShape_IsStable()
        {
            var handler = new CapturingHttpMessageHandler(CreatePapiResponseJson());
            var client = CreateClient(handler);
            var options = new BibSearchOptions
            {
                Branch = 1,
                SearchType = BibSearchTypes.keyword,
                Qualifier = SearchQualifiers.KW,
                Term = "harry potter & stone",
                SortOption = SearchSortOptions.MP,
                Page = 2,
                PageSize = 15,
                Limit = "3"
            };

            var response = await client.BibSearchAsync(options);

            Assert.IsNotNull(response);
            Assert.AreEqual(HttpMethod.Get, handler.LastRequest!.Method);
            StringAssert.Contains(handler.LastRequest.RequestUri!.AbsolutePath, "/public/v1/1033/100/1/search/bibs/keyword/KW");
            var expectedUri = "https://example.test/PAPIService/REST/public/v1/1033/100/1/search/bibs/keyword/KW?q=harry%20potter%20%26%20stone&sort=MP&page=2&bibsperpage=15&limit=3";
            Assert.AreEqual(expectedUri, handler.LastRequest.RequestUri.AbsoluteUri);
            var query = ParseQuery(handler.LastRequest.RequestUri.Query);
            Assert.AreEqual("harry potter & stone", query["q"]);
            Assert.AreEqual("MP", query["sort"]);
            Assert.AreEqual("2", query["page"]);
            Assert.AreEqual("15", query["bibsperpage"]);
            Assert.AreEqual("3", query["limit"]);
            Assert.IsNull(handler.LastRequest.Content);
            Assert.IsTrue(handler.LastRequest.Headers.Contains("PolarisDate"));
            Assert.IsTrue(handler.LastRequest.Headers.Contains("Authorization"));
            AssertAuthorizationHashesSentUri(handler.LastRequest, string.Empty);
        }

        [TestMethod]
        public async Task BibKeywordSearchAsync_ThroughInterface_RequestShapeIsStable()
        {
            var handler = new CapturingHttpMessageHandler(CreatePapiResponseJson());
            IPapiClient client = CreateClient(handler);

            var response = await client.BibKeywordSearchAsync("harry potter & stone", branchId: 7, page: 2, pageSize: 15, sortBy: SearchSortOptions.MP);

            Assert.IsNotNull(response);
            Assert.IsNotNull(handler.LastRequest);
            Assert.AreEqual(HttpMethod.Get, handler.LastRequest!.Method);
            StringAssert.Contains(handler.LastRequest.RequestUri!.AbsolutePath, "/public/v1/1033/100/7/search/bibs/keyword/KW");
            var expectedUri = "https://example.test/PAPIService/REST/public/v1/1033/100/7/search/bibs/keyword/KW?q=harry%20potter%20%26%20stone&sort=MP&page=2&bibsperpage=15";
            Assert.AreEqual(expectedUri, handler.LastRequest.RequestUri.AbsoluteUri);
            var query = ParseQuery(handler.LastRequest.RequestUri.Query);
            Assert.AreEqual("harry potter & stone", query["q"]);
            Assert.AreEqual("MP", query["sort"]);
            Assert.AreEqual("2", query["page"]);
            Assert.AreEqual("15", query["bibsperpage"]);
            Assert.IsNull(handler.LastRequest.Content);
            Assert.IsTrue(handler.LastRequest.Headers.Contains("PolarisDate"));
            Assert.IsTrue(handler.LastRequest.Headers.Contains("Authorization"));
            AssertAuthorizationHashesSentUri(handler.LastRequest, string.Empty);
        }

        [TestMethod]
        public async Task BibKeywordSearchAsync_ThroughInterfaceWithoutBranchId_UsesOrganizationId()
        {
            var handler = new CapturingHttpMessageHandler(CreatePapiResponseJson());
            IPapiClient client = CreateClient(handler);
            client.OrganizationId = 42;

            var response = await client.BibKeywordSearchAsync("default branch search");

            Assert.IsNotNull(response);
            Assert.IsNotNull(handler.LastRequest);
            StringAssert.Contains(handler.LastRequest!.RequestUri!.AbsolutePath, "/public/v1/1033/100/42/search/bibs/keyword/KW");
            var expectedUri = "https://example.test/PAPIService/REST/public/v1/1033/100/42/search/bibs/keyword/KW?q=default%20branch%20search&sort=MP&page=1&bibsperpage=10";
            Assert.AreEqual(expectedUri, handler.LastRequest.RequestUri.AbsoluteUri);
            AssertAuthorizationHashesSentUri(handler.LastRequest, string.Empty);
        }

        [TestMethod]
        public async Task BibBooleanSearchAsync_ThroughInterface_RequestShapeIsStable()
        {
            var handler = new CapturingHttpMessageHandler(CreatePapiResponseJson());
            IPapiClient client = CreateClient(handler);

            var response = await client.BibBooleanSearchAsync("TI=Harry Potter", branchId: 8, page: 3, pageSize: 20, sortBy: SearchSortOptions.AU);

            Assert.IsNotNull(response);
            Assert.IsNotNull(handler.LastRequest);
            Assert.AreEqual(HttpMethod.Get, handler.LastRequest!.Method);
            StringAssert.Contains(handler.LastRequest.RequestUri!.AbsolutePath, "/public/v1/1033/100/8/search/bibs/boolean");
            var expectedUri = "https://example.test/PAPIService/REST/public/v1/1033/100/8/search/bibs/boolean?q=TI%3DHarry%20Potter&sort=AU&page=3&bibsperpage=20";
            Assert.AreEqual(expectedUri, handler.LastRequest.RequestUri.AbsoluteUri);
            var query = ParseQuery(handler.LastRequest.RequestUri.Query);
            Assert.AreEqual("TI=Harry Potter", query["q"]);
            Assert.AreEqual("AU", query["sort"]);
            Assert.AreEqual("3", query["page"]);
            Assert.AreEqual("20", query["bibsperpage"]);
            Assert.IsNull(handler.LastRequest.Content);
            Assert.IsTrue(handler.LastRequest.Headers.Contains("PolarisDate"));
            Assert.IsTrue(handler.LastRequest.Headers.Contains("Authorization"));
            AssertAuthorizationHashesSentUri(handler.LastRequest, string.Empty);
        }

        [TestMethod]
        public async Task BibBooleanSearchAsync_ThroughInterfaceWithoutBranchId_UsesOrganizationId()
        {
            var handler = new CapturingHttpMessageHandler(CreatePapiResponseJson());
            IPapiClient client = CreateClient(handler);
            client.OrganizationId = 42;

            var response = await client.BibBooleanSearchAsync("TI=Default Branch");

            Assert.IsNotNull(response);
            Assert.IsNotNull(handler.LastRequest);
            StringAssert.Contains(handler.LastRequest!.RequestUri!.AbsolutePath, "/public/v1/1033/100/42/search/bibs/boolean");
            var expectedUri = "https://example.test/PAPIService/REST/public/v1/1033/100/42/search/bibs/boolean?q=TI%3DDefault%20Branch&sort=MP&page=1&bibsperpage=10";
            Assert.AreEqual(expectedUri, handler.LastRequest.RequestUri.AbsoluteUri);
            AssertAuthorizationHashesSentUri(handler.LastRequest, string.Empty);
        }

        [TestMethod]
        public async Task BibSearchAsync_DefaultLimit_OmitsLimitAndHashesSentUri()
        {
            var handler = new CapturingHttpMessageHandler(CreatePapiResponseJson());
            var client = CreateClient(handler);
            var options = new BibSearchOptions
            {
                Branch = 1,
                SearchType = BibSearchTypes.keyword,
                Qualifier = SearchQualifiers.KW,
                Term = "harry potter & stone",
                SortOption = SearchSortOptions.MP,
                Page = 2,
                PageSize = 15
            };

            var response = await client.BibSearchAsync(options);

            Assert.IsNotNull(response);
            Assert.IsNotNull(handler.LastRequest);
            var expectedUri = "https://example.test/PAPIService/REST/public/v1/1033/100/1/search/bibs/keyword/KW?q=harry%20potter%20%26%20stone&sort=MP&page=2&bibsperpage=15";
            Assert.AreEqual(expectedUri, handler.LastRequest!.RequestUri!.AbsoluteUri);
            var query = ParseQuery(handler.LastRequest.RequestUri.Query);
            Assert.IsFalse(query.ContainsKey("limit"));
            AssertAuthorizationHashesSentUri(handler.LastRequest, string.Empty);
        }
    }
}
