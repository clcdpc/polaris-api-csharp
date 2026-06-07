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
    public class PapiRestRequestPathClassificationTests : RestClientMigrationTestBase
    {
                [TestMethod]
                public void PapiRestRequest_PathClassification_IsStable()
                {
                    var publicRequest = new PapiRestRequest("/public/foo");
                    Assert.IsTrue(publicRequest.IsPublicMethod);
                    Assert.IsFalse(publicRequest.IsProtectedMethod);
        
                    var protectedRequest = new PapiRestRequest("/protected/foo");
                    Assert.IsFalse(protectedRequest.IsPublicMethod);
                    Assert.IsTrue(protectedRequest.IsProtectedMethod);
                }
    }
}
