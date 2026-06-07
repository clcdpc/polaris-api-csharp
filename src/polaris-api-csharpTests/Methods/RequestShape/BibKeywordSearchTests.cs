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
    public class BibKeywordSearchTests : RestClientMigrationTestBase
    {
                [TestMethod]
                public async Task BibKeywordSearchAsync_ThroughInterface_RequestShapeIsStable()
                {
                    var handler = new CapturingHttpMessageHandler("{\"PAPIErrorCode\":0}");
                    IPapiClient client = CreateClient(handler);
        
                    var response = await client.BibKeywordSearchAsync(
                        "harry potter & stone",
                        branchId: 7,
                        page: 2,
                        pageSize: 15,
                        sortBy: SearchSortOptions.MP);
        
                    Assert.IsNotNull(response);
                    Assert.IsNotNull(handler.LastRequest);
                    Assert.AreEqual(HttpMethod.Get, handler.LastRequest!.Method);
                    StringAssert.Contains(handler.LastRequest.RequestUri!.AbsolutePath, "/public/v1/1033/100/7/search/bibs/keyword/KW");
                    Assert.AreEqual("https://example.test/PAPIService/REST/public/v1/1033/100/7/search/bibs/keyword/KW?q=harry%20potter%20%26%20stone&sort=MP&page=2&bibsperpage=15", handler.LastRequest.RequestUri.AbsoluteUri);
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
                    var handler = new CapturingHttpMessageHandler("{\"PAPIErrorCode\":0}");
                    IPapiClient client = CreateClient(handler);
                    client.OrganizationId = 42;
        
                    var response = await client.BibKeywordSearchAsync("default branch search");
        
                    Assert.IsNotNull(response);
                    Assert.IsNotNull(handler.LastRequest);
                    StringAssert.Contains(handler.LastRequest!.RequestUri!.AbsolutePath, "/public/v1/1033/100/42/search/bibs/keyword/KW");
                    Assert.AreEqual("https://example.test/PAPIService/REST/public/v1/1033/100/42/search/bibs/keyword/KW?q=default%20branch%20search&sort=MP&page=1&bibsperpage=10", handler.LastRequest.RequestUri.AbsoluteUri);
                    AssertAuthorizationHashesSentUri(handler.LastRequest, string.Empty);
                }
    }
}
