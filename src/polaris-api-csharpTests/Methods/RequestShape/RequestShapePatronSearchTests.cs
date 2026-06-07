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
    public class RequestShapePatronSearchTests : RestClientMigrationTestBase
    {
                [TestMethod]
                public async Task PatronSearch_RequestShape_PreservesEncodedQuerySemantics()
                {
                    var handler = new CapturingHttpMessageHandler("{\"PAPIErrorCode\":0}");
                    var client = CreateClient(handler);
                    client.Token = new ProtectedToken { AccessToken = "token", AccessSecret = "secret", ExpirationDate = DateTime.Now.AddHours(1) };
        
                    var response = await client.PatronSearchAsync("name = Smith & status: active", page: 3, pageSize: 25, sortBy: PatronSortKeys.PATNL, orgId: 9);
        
                    Assert.IsNotNull(response);
                    Assert.AreEqual(HttpMethod.Get, handler.LastRequest!.Method);
                    StringAssert.Contains(handler.LastRequest.RequestUri!.AbsolutePath, "/protected/v1/1033/100/9/token/search/patrons/Boolean");
                    var query = ParseQuery(handler.LastRequest.RequestUri.Query);
                    Assert.AreEqual("name = Smith & status: active", query["q"]);
                    Assert.AreEqual("25", query["patronsperpage"]);
                    Assert.AreEqual("3", query["page"]);
                    Assert.AreEqual("PATNL", query["sort"]);
                    Assert.IsNull(handler.LastRequest.Content);
                    Assert.IsTrue(handler.LastRequest.Headers.Contains("PolarisDate"));
                    Assert.IsTrue(handler.LastRequest.Headers.Contains("Authorization"));
                }
    }
}
