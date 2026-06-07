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
    public class PatronSearchTests : PapiClientTestBase
    {
        [TestMethod]
        public async Task PatronSearchAsync_PassesCancellationTokenToProtectedTokenAcquisition()
        {
            var handler = new ProtectedTokenCancellationHttpMessageHandler();
            var client = CreateClient(handler);
            client.AllowStaffOverrideRequests = true;
            client.StaffOverrideAccount = new PolarisUser
            {
                Domain = "main",
                Username = "staff",
                Password = "secret"
            };
            using var cts = new CancellationTokenSource();

            var response = await client.PatronSearchAsync("name=Smith", cancellationToken: cts.Token);

            Assert.IsNotNull(response);
            Assert.AreEqual(2, handler.Requests.Count);
            StringAssert.Contains(handler.Requests[0].RequestUri!.AbsolutePath, "/protected/v1/1033/100/1/authenticator/staff");
            Assert.AreEqual(HttpMethod.Post, handler.Requests[0].Method);
            Assert.IsTrue(handler.CancellationTokens[0].CanBeCanceled);
            StringAssert.Contains(handler.Requests[1].RequestUri!.AbsolutePath, "/protected/v1/1033/100/1/protected-token/search/patrons/Boolean");
            Assert.IsTrue(handler.CancellationTokens[1].CanBeCanceled);
        }

        [TestMethod]
        public async Task ProtectedTokenAcquisition_CanceledWaiterDoesNotHoldCacheLock()
        {
            var handler = new BlockingProtectedTokenHttpMessageHandler();
            var cacheUsername = $"cancel-waiter-{Guid.NewGuid():N}";
            var firstClient = CreateClientWithProtectedCache(handler, cacheUsername);
            var secondClient = CreateClientWithProtectedCache(handler, cacheUsername);
            var thirdClient = CreateClientWithProtectedCache(handler, cacheUsername);

            var firstRequest = firstClient.PatronSearchAsync("name=First", orgId: 9);
            await handler.AuthenticationStarted.Task.ConfigureAwait(false);
            using var secondCts = new CancellationTokenSource();
            var secondRequest = secondClient.PatronSearchAsync("name=Second", orgId: 9, cancellationToken: secondCts.Token);

            secondCts.Cancel();
            await Assert.ThrowsExceptionAsync<OperationCanceledException>(async () => await secondRequest.ConfigureAwait(false));

            handler.CompleteAuthentication.SetResult();
            await firstRequest.ConfigureAwait(false);

            var laterResponse = await thirdClient.PatronSearchAsync("name=Later", orgId: 9);

            Assert.IsNotNull(laterResponse);
            Assert.AreEqual(1, handler.AuthenticationRequestCount);
            Assert.AreEqual(2, handler.ProtectedRequests.Count);
            StringAssert.Contains(handler.ProtectedRequests[1].RequestUri!.AbsolutePath, "/protected/v1/1033/100/9/blocked-token/search/patrons/Boolean");
            Assert.IsFalse(handler.ProtectedRequests[1].RequestUri!.AbsoluteUri.Contains(ProtectedToken.Placeholder, StringComparison.Ordinal));
        }
    }
}
