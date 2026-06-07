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
    [DoNotParallelize]
    public class PatronSearchProtectedTokenTests : PapiClientTestBase
    {
        [TestMethod]
        public async Task PatronSearchAsync_CancellationDuringProtectedTokenAuthentication_PropagatesCancellation()
        {
            var handler = new ProtectedTokenHttpMessageHandler();
            var client = CreateProtectedClient(handler);
            using var cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromMilliseconds(10));

            try
            {
                await client.PatronSearchAsync("name=Smith", cancellationToken: cancellationTokenSource.Token);
                Assert.Fail("Expected cancellation to propagate.");
            }
            catch (OperationCanceledException)
            {
            }

            Assert.AreEqual(1, handler.AuthenticationRequestCount);
            Assert.AreEqual(0, handler.NonAuthenticationRequestCount);
        }
    }
}
