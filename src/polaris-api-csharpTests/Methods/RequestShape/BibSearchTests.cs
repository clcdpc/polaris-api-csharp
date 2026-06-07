using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Clc.Polaris.Api;
using Clc.Polaris.Api.Configuration;
using Clc.Polaris.Api.Models;
using Clc.Rest.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Clc.Polaris.Api.Tests
{
    [TestClass]
    [UnitCategory]
    public class RequestShapeBibSearchTests : RestClientMigrationTestBase
    {
                [TestMethod]
                public async Task BibSearch_RequestShape_IsStable()
                {
                    var handler = new CapturingHttpMessageHandler("{\"PAPIErrorCode\":0}");
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
                    Assert.AreEqual("https://example.test/PAPIService/REST/public/v1/1033/100/1/search/bibs/keyword/KW?q=harry%20potter%20%26%20stone&sort=MP&page=2&bibsperpage=15&limit=3", handler.LastRequest.RequestUri.AbsoluteUri);
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
                public async Task BibSearchAsync_DefaultLimit_OmitsLimitAndHashesSentUri()
                {
                    var handler = new CapturingHttpMessageHandler("{\"PAPIErrorCode\":0}");
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
                    Assert.AreEqual("https://example.test/PAPIService/REST/public/v1/1033/100/1/search/bibs/keyword/KW?q=harry%20potter%20%26%20stone&sort=MP&page=2&bibsperpage=15", handler.LastRequest!.RequestUri!.AbsoluteUri);
                    var query = ParseQuery(handler.LastRequest.RequestUri.Query);
                    Assert.IsFalse(query.ContainsKey("limit"));
                    AssertAuthorizationHashesSentUri(handler.LastRequest, string.Empty);
                }
    }
}
