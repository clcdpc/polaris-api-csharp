using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Clc.Polaris.Api.Models;
using Clc.Polaris.Api.Tests;
using Clc.Polaris.Api.Tests.TestInfrastructure;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Clc.Polaris.Api.Tests.Methods.Cancellation
{
    [TestClass]
    [DoNotParallelize]
    [UnitCategory]
    public class PatronSearchProtectedTokenTests : PapiClientTestBase
    {
        [TestInitialize]
        public void TestInitialize()
        {
            ClearProtectedTokenState();
        }

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

        private sealed class BlockingProtectedTokenHttpMessageHandler : HttpMessageHandler
        {
            private readonly object _syncRoot = new();
            private int _authenticationRequestCount;

            public TaskCompletionSource AuthenticationStarted { get; } = new(TaskCreationOptions.RunContinuationsAsynchronously);
            public TaskCompletionSource CompleteAuthentication { get; } = new(TaskCreationOptions.RunContinuationsAsynchronously);
            public int AuthenticationRequestCount => _authenticationRequestCount;
            public List<HttpRequestMessage> ProtectedRequests { get; } = new();

            protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            {
                if (request.RequestUri!.AbsolutePath.Contains("/authenticator/staff", StringComparison.Ordinal))
                {
                    Interlocked.Increment(ref _authenticationRequestCount);
                    AuthenticationStarted.TrySetResult();
                    await CompleteAuthentication.Task.WaitAsync(cancellationToken).ConfigureAwait(false);
                    return new HttpResponseMessage(HttpStatusCode.OK)
                    {
                        Content = new StringContent("{\"PAPIErrorCode\":0,\"AccessToken\":\"blocked-token\",\"AccessSecret\":\"blocked-secret\",\"AuthExpDate\":\"2030-01-01T00:00:00Z\"}", Encoding.UTF8, "application/json")
                    };
                }

                lock (_syncRoot)
                {
                    ProtectedRequests.Add(request);
                }

                return new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent("{\"PAPIErrorCode\":0}", Encoding.UTF8, "application/json")
                };
            }
        }
    }
}
