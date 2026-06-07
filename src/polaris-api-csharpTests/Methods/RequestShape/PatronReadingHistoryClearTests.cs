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

namespace Clc.Polaris.Api.Tests.Methods.RequestShape
{
    [TestClass]
    [UnitCategory]
    public class PatronReadingHistoryClearTests : PapiClientTestBase
    {
        [TestMethod]
        public async Task PatronReadingHistoryClearAsync_EnumeratesIdsOnceAndSendsCommaSeparatedIds()
        {
            var handler = new CapturingHttpMessageHandler("{\"PAPIErrorCode\":0}");
            var client = CreateClient(handler);
            var ids = new ThrowOnSecondEnumerationEnumerable(new[] { 101, 202, 303 });

            var response = await client.PatronReadingHistoryClearAsync("ABC123", "patron-password", ids);

            Assert.IsNotNull(response);
            Assert.AreEqual(1, ids.EnumerationCount);
            Assert.IsNotNull(handler.LastRequest);
            Assert.AreEqual(HttpMethod.Delete, handler.LastRequest!.Method);
            var query = ParseQuery(handler.LastRequest.RequestUri!.Query);
            Assert.AreEqual("101,202,303", query["ids"]);
            AssertAuthorizationHashesSentUri(handler.LastRequest, "patron-password");
        }
    }
}
