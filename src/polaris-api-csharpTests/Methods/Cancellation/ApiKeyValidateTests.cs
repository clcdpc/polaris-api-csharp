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

namespace Clc.Polaris.Api.Tests.Methods.Cancellation
{
    [TestClass]
    [UnitCategory]
    public class ApiKeyValidateTests : PapiClientTestBase
    {
        [TestMethod]
        public async Task ApiKeyValidateAsync_PassesCancellationToken()
        {
            var handler = new CapturingHttpMessageHandler("{\"PAPIErrorCode\":0}");
            var client = CreateClient(handler);
            using var cts = new CancellationTokenSource();

            var response = await client.ApiKeyValidateAsync(cts.Token);

            Assert.IsNotNull(response);
            Assert.IsTrue(handler.LastCancellationToken.CanBeCanceled);
        }
    }
}
