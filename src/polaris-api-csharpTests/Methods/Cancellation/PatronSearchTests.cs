using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Clc.Polaris.Api.Models;
using Clc.Polaris.Api.Tests;
using Clc.Polaris.Api.Tests.TestInfrastructure;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Clc.Polaris.Api.Tests.Methods.Cancellation
{
    [TestClass]
    [UnitCategory]
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
    }
}
