using Clc.Polaris.Api;
using Clc.Polaris.Api.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Clc.Polaris.Api.Tests
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
            using var cancellationTokenSource = new CancellationTokenSource();
            var searchTask = client.PatronSearchAsync("name=Smith", cancellationToken: cancellationTokenSource.Token);

            await WaitForAuthenticationRequestAsync(handler);
            cancellationTokenSource.Cancel();

            await Assert.ThrowsExceptionAsync<OperationCanceledException>(async () => await searchTask.ConfigureAwait(false));

            Assert.AreEqual(1, handler.AuthenticationRequestCount);
            Assert.AreEqual(0, handler.NonAuthenticationRequestCount);
        }

        private static async Task WaitForAuthenticationRequestAsync(ProtectedTokenHttpMessageHandler handler)
        {
            using var timeout = new CancellationTokenSource(TimeSpan.FromSeconds(1));
            while (handler.AuthenticationRequestCount == 0 && !timeout.IsCancellationRequested)
            {
                await Task.Delay(10, timeout.Token).ConfigureAwait(false);
            }

            Assert.AreEqual(1, handler.AuthenticationRequestCount);
        }
    }
}
