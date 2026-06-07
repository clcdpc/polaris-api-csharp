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
    public class BibBooleanSearchTests : RestClientMigrationTestBase
    {
                [TestMethod]
                public async Task BibBooleanSearchAsync_ThroughInterface_RequestShapeIsStable()
                {
                    var handler = new CapturingHttpMessageHandler("{\"PAPIErrorCode\":0}");
                    IPapiClient client = CreateClient(handler);
        
                    var response = await client.BibBooleanSearchAsync(
                        "TI=Harry Potter",
                        branchId: 8,
                        page: 3,
                        pageSize: 20,
                        sortBy: SearchSortOptions.AU);
        
                    Assert.IsNotNull(response);
                    Assert.IsNotNull(handler.LastRequest);
                    Assert.AreEqual(HttpMethod.Get, handler.LastRequest!.Method);
                    StringAssert.Contains(handler.LastRequest.RequestUri!.AbsolutePath, "/public/v1/1033/100/8/search/bibs/boolean");
                    Assert.AreEqual("https://example.test/PAPIService/REST/public/v1/1033/100/8/search/bibs/boolean?q=TI%3DHarry%20Potter&sort=AU&page=3&bibsperpage=20", handler.LastRequest.RequestUri.AbsoluteUri);
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
                    var handler = new CapturingHttpMessageHandler("{\"PAPIErrorCode\":0}");
                    IPapiClient client = CreateClient(handler);
                    client.OrganizationId = 42;
        
                    var response = await client.BibBooleanSearchAsync("TI=Default Branch");
        
                    Assert.IsNotNull(response);
                    Assert.IsNotNull(handler.LastRequest);
                    StringAssert.Contains(handler.LastRequest!.RequestUri!.AbsolutePath, "/public/v1/1033/100/42/search/bibs/boolean");
                    Assert.AreEqual("https://example.test/PAPIService/REST/public/v1/1033/100/42/search/bibs/boolean?q=TI%3DDefault%20Branch&sort=MP&page=1&bibsperpage=10", handler.LastRequest.RequestUri.AbsoluteUri);
                    AssertAuthorizationHashesSentUri(handler.LastRequest, string.Empty);
                }
    }
}
