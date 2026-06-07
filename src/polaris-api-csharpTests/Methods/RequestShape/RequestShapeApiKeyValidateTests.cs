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
    public class RequestShapeApiKeyValidateTests : RestClientMigrationTestBase
    {
                [TestMethod]
                public async Task ApiKeyValidate_RequestShape_IsStable()
                {
                    var handler = new CapturingHttpMessageHandler("{\"PAPIErrorCode\":0}");
                    var client = CreateClient(handler);
        
                    var response = await client.ApiKeyValidateAsync();
        
                    Assert.IsNotNull(response);
                    Assert.AreEqual(HttpMethod.Get, handler.LastRequest!.Method);
                    StringAssert.Contains(handler.LastRequest.RequestUri!.AbsolutePath, "/public/v1/1033/100/1/apikeyvalidate");
                    Assert.AreEqual(string.Empty, handler.LastRequest.RequestUri.Query);
                    Assert.IsNull(handler.LastRequest.Content);
                    Assert.IsTrue(handler.LastRequest.Headers.Contains("PolarisDate"));
                    Assert.IsTrue(handler.LastRequest.Headers.Contains("Authorization"));
                }
    }
}
