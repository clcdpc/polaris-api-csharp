using System.Net;

namespace Clc.Polaris.Api.Tests.Methods.RequestShape
{
    [TestClass]
    [UnitTest]
    public class PatronUpdateUserNameTests : PapiClientTestBase
    {

        [TestMethod]
        public async Task PatronUpdateUserName_EncodesBarcodeAndNewUsername()
        {
            var handler = new CapturingHttpMessageHandler();
            var client = CreateUrlEncodingClient(handler);
            var barcode = "AB C/+#?=";
            var newUsername = "new user+/name?=";

            await client.PatronUpdateUserNameAsync(barcode, newUsername, "pin", TestContext.CancellationToken);

            var expectedPath = $"/PAPIService/REST/public/v1/1033/100/1/patron/{WebUtility.UrlEncode(barcode)}/username/{WebUtility.UrlEncode(newUsername)}";
            Assert.IsNotNull(handler.LastRequest);
            Assert.AreEqual(expectedPath, handler.LastRequest!.RequestUri!.AbsolutePath);
        }
    }
}
