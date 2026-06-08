using System;
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
    [UnitCategory]
    public class PatronMessagesGetTests : PapiClientTestBase
    {
        [TestMethod]
        public async Task PatronMessages_RequestShape_PreservesBooleanLikeQueryValue()
        {
            var handler = new CapturingHttpMessageHandler("{\"PAPIErrorCode\":0}");
            var client = CreateClient(handler);

            var response = await client.PatronMessagesGetAsync("ABC 123", unreadOnly: true, password: "1234");

            Assert.IsNotNull(response);
            Assert.AreEqual(HttpMethod.Get, handler.LastRequest!.Method);
            StringAssert.Contains(handler.LastRequest.RequestUri!.AbsolutePath, "/public/v1/1033/100/1/patron/ABC+123/messages");
            var query = ParseQuery(handler.LastRequest.RequestUri.Query);
            Assert.AreEqual("1", query["unreadonly"]);
            Assert.IsNull(handler.LastRequest.Content);
            Assert.IsTrue(handler.LastRequest.Headers.Contains("PolarisDate"));
            Assert.IsTrue(handler.LastRequest.Headers.Contains("Authorization"));
        }
    }
}
