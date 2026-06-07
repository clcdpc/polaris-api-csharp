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
    public class PapiRestRequestConstructorTests : PapiClientTestBase
    {
        [TestMethod]
        public void PapiRestRequest_Constructors_PreserveCurrentBehavior()
        {
            var defaultGet = new PapiRestRequest("/public/foo");
            Assert.AreEqual(HttpMethod.Get, defaultGet.Method);
            Assert.AreEqual("/public/foo", defaultGet.Path);

            var body = new { Name = "Example" };
            var put = new PapiRestRequest(HttpMethod.Put, "/public/foo", "pin", body);
            Assert.AreEqual(HttpMethod.Put, put.Method);
            Assert.AreEqual("/public/foo", put.Path);
            Assert.AreEqual("pin", put.Password);
            Assert.AreSame(body, put.Body);

            var existing = new RestRequest
            {
                Method = HttpMethod.Post,
                Path = "/protected/foo",
                Body = new { Value = "v" },
            };
            existing.QueryParameters.Add("limit", "5");
            existing.Headers.Add("X-Test", "header");

            var copied = new PapiRestRequest(existing);
            Assert.AreEqual(existing.Method, copied.Method);
            Assert.AreEqual(existing.Path, copied.Path);
            Assert.AreSame(existing.Body, copied.Body);
            Assert.AreSame(existing.QueryParameters, copied.QueryParameters);
            Assert.AreSame(existing.Headers, copied.Headers);
        }
    }
}
