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
    public class PreformatRestRequestHeaderTests : RestClientMigrationTestBase
    {
                [TestMethod]
                public void PreformatRestRequest_AddsPapiHeaders_WhenAuthRequired()
                {
                    var client = CreateClient();
                    var request = new PapiRestRequest(HttpMethod.Get, "/public/v1/1033/100/1/apikeyvalidate");
        
                    var formatted = (PapiRestRequest)client.PreformatRestRequest(request);
        
                    Assert.AreEqual(HttpMethod.Get, formatted.Method);
                    Assert.AreEqual("/public/v1/1033/100/1/apikeyvalidate", formatted.Path);
                    Assert.IsTrue(formatted.Headers.ContainsKey("PolarisDate"));
                    Assert.IsTrue(formatted.Headers.ContainsKey("Authorization"));
                    StringAssert.StartsWith(formatted.Headers["Authorization"], "PWS access-id:");
                }

                [TestMethod]
                public void PreformatRestRequest_DoesNotAddPapiHeaders_WhenAuthNotRequired()
                {
                    var client = CreateClient();
                    var request = new PapiRestRequest(HttpMethod.Get, "/public/v1/1033/100/1/apikeyvalidate")
                    {
                        AuthRequired = false
                    };
        
                    var formatted = (PapiRestRequest)client.PreformatRestRequest(request);
        
                    Assert.IsFalse(formatted.Headers.ContainsKey("PolarisDate"));
                    Assert.IsFalse(formatted.Headers.ContainsKey("Authorization"));
                }
    }
}
