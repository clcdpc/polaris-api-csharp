namespace Clc.Polaris.Api.Tests.Methods.Cancellation
{
    [TestClass]
    [UnitTest]
    public class ApiKeyValidateTests : PapiClientTestBase
    {
        [TestMethod]
        public async Task ApiKeyValidateAsync_PassesCancellationToken()
        {
            var handler = new CapturingHttpMessageHandler(CreatePapiResponseJson());
            var client = CreateClient(handler);
            using var cts = new CancellationTokenSource();

            var response = await client.ApiKeyValidateAsync(cts.Token);

            Assert.IsNotNull(response);
            Assert.IsTrue(handler.LastCancellationToken.CanBeCanceled);
        }
    }
}
