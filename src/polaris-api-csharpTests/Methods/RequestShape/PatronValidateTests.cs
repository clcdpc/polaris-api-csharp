using System.Net;

namespace Clc.Polaris.Api.Tests.Methods.RequestShape
{
    [TestClass]
    [UnitTest]
    public class PatronValidateTests : PapiClientTestBase
    {

        [TestMethod]
        public async Task PatronValidate_NormalBarcode_PreservesBarcodePath()
        {
            var handler = new CapturingHttpMessageHandler();
            var client = CreateUrlEncodingClient(handler);
            var barcode = "21945001234567";

            await client.PatronValidateAsync(barcode, "pin", TestContext.CancellationToken);

            var expectedPath = $"/PAPIService/REST/public/v1/1033/100/1/patron/{WebUtility.UrlEncode(barcode)}";
            Assert.IsNotNull(handler.LastRequest);
            Assert.AreEqual(expectedPath, handler.LastRequest!.RequestUri!.AbsolutePath);
        }


        [TestMethod]
        public async Task PatronValidate_SpecialCharacters_EncodesBarcodePathSegment()
        {
            var handler = new CapturingHttpMessageHandler();
            var client = CreateUrlEncodingClient(handler);
            var barcode = "AB C/+#?=";

            await client.PatronValidateAsync(barcode, "pin", TestContext.CancellationToken);

            var expectedPath = $"/PAPIService/REST/public/v1/1033/100/1/patron/{WebUtility.UrlEncode(barcode)}";
            Assert.IsNotNull(handler.LastRequest);
            Assert.AreEqual(expectedPath, handler.LastRequest!.RequestUri!.AbsolutePath);
        }
    }
}
