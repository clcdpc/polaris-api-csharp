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

namespace Clc.Polaris.Api.Tests.Models
{
    [TestClass]
    [UnitCategory]
    public class PapiRestRequestPathClassificationTests : PapiClientTestBase
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
