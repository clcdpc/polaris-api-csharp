using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Clc.Polaris.Api.Tests;
using Clc.Polaris.Api.Tests.TestInfrastructure;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Clc.Polaris.Api.Tests.PapiClientBehavior
{
    [TestClass]
    [DoNotParallelize]
    [UnitTest]
    public class PapiClientProtectedTokenCacheLockTests : PapiClientTestBase
    {
        [TestMethod]
        public async Task ProtectedTokenAcquisition_DifferentCacheKeys_DoNotBlockEachOther()
        {
            var handler = new PerUserBlockingProtectedTokenHttpMessageHandler();

            var blockedClient = CreateClientWithProtectedCache(handler, $"blocked-{Guid.NewGuid():N}");
            var independentClient = CreateClientWithProtectedCache(handler, $"independent-{Guid.NewGuid():N}");

            Task? blockedRequest = null;
            Task? independentRequest = null;

            try
            {
                blockedRequest = blockedClient.PatronSearchAsync("name=Blocked", orgId: 9, cancellationToken: TestContext.CancellationToken);
                await handler.BlockedAuthenticationStarted.Task.WaitAsync(TimeSpan.FromSeconds(5), TestContext.CancellationToken);

                independentRequest = independentClient.PatronSearchAsync("name=Independent", orgId: 9, cancellationToken: TestContext.CancellationToken);
                var completed = await Task.WhenAny(independentRequest, Task.Delay(TimeSpan.FromSeconds(1), TestContext.CancellationToken));

                Assert.AreSame(independentRequest, completed, "An authentication request for a different protected-token cache key should not wait behind the blocked cache key.");

                await independentRequest.ConfigureAwait(false);

                Assert.IsTrue(independentRequest.IsCompletedSuccessfully);
                Assert.AreEqual(2, handler.AuthenticationRequestCount);
                Assert.AreEqual(1, handler.NonAuthenticationRequestCount);
                Assert.Contains("/protected/v1/1033/100/9/independent-token/search/patrons/Boolean", handler.ProtectedRequests.Single().RequestUri!.AbsolutePath);

                handler.CompleteBlockedAuthentication.TrySetResult();

                await blockedRequest.ConfigureAwait(false);

                Assert.IsTrue(blockedRequest.IsCompletedSuccessfully);
                Assert.AreEqual(2, handler.NonAuthenticationRequestCount);
            }
            finally
            {
                handler.CompleteBlockedAuthentication.TrySetResult();

                await DrainTaskAsync(blockedRequest).ConfigureAwait(false);
                await DrainTaskAsync(independentRequest).ConfigureAwait(false);
            }
        }

        private static async Task DrainTaskAsync(Task? task)
        {
            if (task == null)
            {
                return;
            }

            try
            {
                await task.WaitAsync(TimeSpan.FromSeconds(5)).ConfigureAwait(false);
            }
            catch
            {
                // Cleanup path only. Do not mask the original test failure.
            }
        }

        private sealed class PerUserBlockingProtectedTokenHttpMessageHandler : HttpMessageHandler
        {
            private readonly object _syncRoot = new();
            private int _authenticationRequestCount;
            private int _nonAuthenticationRequestCount;

            public TaskCompletionSource BlockedAuthenticationStarted { get; } = new(TaskCreationOptions.RunContinuationsAsynchronously);
            public TaskCompletionSource CompleteBlockedAuthentication { get; } = new(TaskCreationOptions.RunContinuationsAsynchronously);

            public int AuthenticationRequestCount => _authenticationRequestCount;
            public int NonAuthenticationRequestCount => _nonAuthenticationRequestCount;
            public List<HttpRequestMessage> ProtectedRequests { get; } = [];

            protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            {
                if (request.RequestUri!.AbsolutePath.Contains("/authenticator/staff", StringComparison.Ordinal))
                {
                    Interlocked.Increment(ref _authenticationRequestCount);

                    var body = request.Content == null
                        ? string.Empty
                        : await request.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);

                    if (body.Contains("blocked-", StringComparison.Ordinal))
                    {
                        BlockedAuthenticationStarted.TrySetResult();
                        await CompleteBlockedAuthentication.Task.WaitAsync(cancellationToken).ConfigureAwait(false);

                        var responseJson = CreateProtectedTokenJson(accessToken: "blocked-token", accessSecret: "blocked-secret", expirationDate: ValidProtectedTokenExpirationDate);
                        return new HttpResponseMessage(HttpStatusCode.OK)
                        {
                            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
                        };
                    }

                    var independentResponseJson = CreateProtectedTokenJson(accessToken: "independent-token", accessSecret: "independent-secret", expirationDate: ValidProtectedTokenExpirationDate);
                    return new HttpResponseMessage(HttpStatusCode.OK)
                    {
                        Content = new StringContent(independentResponseJson, Encoding.UTF8, "application/json")
                    };
                }

                Interlocked.Increment(ref _nonAuthenticationRequestCount);

                lock (_syncRoot)
                {
                    ProtectedRequests.Add(request);
                }

                return new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(CreatePapiResponseJson(), Encoding.UTF8, "application/json")
                };
            }
        }

        public TestContext TestContext { get; set; } = null!;
    }
}
