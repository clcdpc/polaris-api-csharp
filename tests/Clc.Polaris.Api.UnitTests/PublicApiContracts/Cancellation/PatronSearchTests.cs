namespace Clc.Polaris.Api.UnitTests.PublicApiContracts.Cancellation
{
    [TestClass]
    [UnitTest]
    public class PatronSearchTests : PapiClientUnitTestBase
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
            Assert.HasCount(2, handler.Requests);
            Assert.Contains("/protected/v1/1033/100/1/authenticator/staff", handler.Requests[0].RequestUri!.AbsolutePath);
            Assert.AreEqual(HttpMethod.Post, handler.Requests[0].Method);
            Assert.IsTrue(handler.CancellationTokens[0].CanBeCanceled);
            Assert.Contains("/protected/v1/1033/100/1/protected-token/search/patrons/Boolean", handler.Requests[1].RequestUri!.AbsolutePath);
            Assert.IsTrue(handler.CancellationTokens[1].CanBeCanceled);
        }
    }
}
