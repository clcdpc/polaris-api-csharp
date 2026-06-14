namespace Clc.Polaris.Api.UnitTests.Methods.RequestShape
{
    [TestClass]
    [UnitTest]
    public class Patron_GetBarcodeFromIdTests : PapiClientUnitTestBase
    {
        [TestMethod]
        public async Task Patron_GetBarcodeFromId_FormatsUrlCorrectly()
        {
            var handler = new CapturingHttpMessageHandler();
            var client = CreateUrlEncodingClient(handler);
            var patronId = 12345;
            client.Token = new ProtectedToken
            {
                AccessToken = "token-segment",
                AccessSecret = "token-secret",
                ExpirationDate = ValidProtectedTokenExpirationDate
            };

            await client.Patron_GetBarcodeFromIdAsync(patronId, TestContext.CancellationToken);

            var expectedPath = "/PAPIService/REST/protected/v1/1033/100/1/token-segment/patron/barcode";
            var expectedQuery = $"?patronid={patronId}";

            Assert.IsNotNull(handler.LastRequest);
            Assert.AreEqual(expectedPath, handler.LastRequest!.RequestUri!.AbsolutePath);
            Assert.AreEqual(expectedQuery, handler.LastRequest.RequestUri.Query);
        }
    }
}
