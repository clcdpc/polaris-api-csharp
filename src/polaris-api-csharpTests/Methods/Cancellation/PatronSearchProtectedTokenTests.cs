using System.Net;
using System.Text;

namespace Clc.Polaris.Api.Tests.Methods.Cancellation
{
    [TestClass]
    [DoNotParallelize]
    [UnitTest]
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
            var handler = new CancelableProtectedTokenAuthenticationHttpMessageHandler();
            var client = CreateClient(handler);
            client.AllowStaffOverrideRequests = true;
            client.UseProtectedTokenCache = true;
            client.StaffOverrideAccount = CreateStaffUser();

            using var cancellationTokenSource = new CancellationTokenSource();

            var request = client.PatronSearchAsync("name=Smith", cancellationToken: cancellationTokenSource.Token);

            await handler.AuthenticationStarted.Task.WaitAsync(TimeSpan.FromSeconds(5), TestContext.CancellationToken);

            cancellationTokenSource.Cancel();

            await Assert.ThrowsAsync<OperationCanceledException>(async () => await request.ConfigureAwait(false));

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

            var firstRequest = firstClient.PatronSearchAsync("name=First", orgId: 9, cancellationToken: TestContext.CancellationToken);
            await handler.AuthenticationStarted.Task.ConfigureAwait(false);
            using var secondCts = new CancellationTokenSource();
            var secondRequest = secondClient.PatronSearchAsync("name=Second", orgId: 9, cancellationToken: secondCts.Token);

            secondCts.Cancel();
            await Assert.ThrowsAsync<OperationCanceledException>(async () => await secondRequest.ConfigureAwait(false));

            handler.CompleteAuthentication.SetResult();
            await firstRequest.ConfigureAwait(false);

            var laterResponse = await thirdClient.PatronSearchAsync("name=Later", orgId: 9, cancellationToken: TestContext.CancellationToken);

            Assert.IsNotNull(laterResponse);
            Assert.AreEqual(1, handler.AuthenticationRequestCount);
            Assert.HasCount(2, handler.ProtectedRequests);
            Assert.Contains("/protected/v1/1033/100/9/blocked-token/search/patrons/Boolean", handler.ProtectedRequests[1].RequestUri!.AbsolutePath);
            Assert.IsFalse(handler.ProtectedRequests[1].RequestUri!.AbsoluteUri.Contains(ProtectedToken.Placeholder, StringComparison.Ordinal));
        }

        private sealed class BlockingProtectedTokenHttpMessageHandler : HttpMessageHandler
        {
            private readonly object _syncRoot = new();
            private int _authenticationRequestCount;

            public TaskCompletionSource AuthenticationStarted { get; } = new(TaskCreationOptions.RunContinuationsAsynchronously);
            public TaskCompletionSource CompleteAuthentication { get; } = new(TaskCreationOptions.RunContinuationsAsynchronously);
            public int AuthenticationRequestCount => _authenticationRequestCount;
            public List<HttpRequestMessage> ProtectedRequests { get; } = [];

            protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            {
                if (request.RequestUri!.AbsolutePath.Contains("/authenticator/staff", StringComparison.Ordinal))
                {
                    Interlocked.Increment(ref _authenticationRequestCount);
                    AuthenticationStarted.TrySetResult();
                    await CompleteAuthentication.Task.WaitAsync(cancellationToken).ConfigureAwait(false);
                    var responseJson = CreateProtectedTokenJson(accessToken: "blocked-token", accessSecret: "blocked-secret", expirationDate: ValidProtectedTokenExpirationDate);
                    return new HttpResponseMessage(HttpStatusCode.OK)
                    {
                        Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
                    };
                }

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

        private sealed class CancelableProtectedTokenAuthenticationHttpMessageHandler : HttpMessageHandler
        {
            private int _authenticationRequestCount;
            private int _nonAuthenticationRequestCount;

            public TaskCompletionSource AuthenticationStarted { get; } =
                new(TaskCreationOptions.RunContinuationsAsynchronously);

            public int AuthenticationRequestCount => _authenticationRequestCount;

            public int NonAuthenticationRequestCount => _nonAuthenticationRequestCount;

            protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            {
                if (request.RequestUri!.AbsolutePath.Contains("/authenticator/staff", StringComparison.Ordinal))
                {
                    Interlocked.Increment(ref _authenticationRequestCount);
                    AuthenticationStarted.TrySetResult();

                    await Task.Delay(Timeout.InfiniteTimeSpan, cancellationToken).ConfigureAwait(false);

                    Assert.Fail("Expected staff authentication to be canceled before returning a response.");
                }

                Interlocked.Increment(ref _nonAuthenticationRequestCount);

                return new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(CreatePapiResponseJson(), Encoding.UTF8, "application/json")
                };
            }
        }
    }
}
