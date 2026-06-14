using System.Net;

namespace Clc.Polaris.Api.UnitTests.Methods.RequestShape
{
    [TestClass]
    [UnitTest]
    public class CreatePatronBlocksTests : PapiClientUnitTestBase
    {
        [TestMethod]
        public async Task CreatePatronBlocks_EncodesBarcodeInProtectedRoute_PreservesTokenPath()
        {
            var handler = new CapturingHttpMessageHandler();
            var client = CreateUrlEncodingClient(handler);
            var barcode = "AB C/+#?=";
            client.Token = new ProtectedToken
            {
                AccessToken = "token-segment",
                AccessSecret = "token-secret",
                ExpirationDate = ValidProtectedTokenExpirationDate
            };

            await client.CreatePatronBlocksAsync(barcode, BlockType.FreeText, "note", userId: 888, workstationId: 999, TestContext.CancellationToken);

            var expectedPath = $"/PAPIService/REST/protected/v1/1033/100/1/token-segment/patron/{WebUtility.UrlEncode(barcode)}/blocks";
            var expectedQuery = "?wsid=999&userid=888";

            Assert.IsNotNull(handler.LastRequest);
            Assert.AreEqual(expectedPath, handler.LastRequest!.RequestUri!.AbsolutePath);
            Assert.AreEqual(expectedQuery, handler.LastRequest.RequestUri.Query);
        }
    }
}
