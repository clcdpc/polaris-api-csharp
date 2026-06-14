using System.Net;

namespace Clc.Polaris.Api.UnitTests.Methods.RequestShape
{
    [TestClass]
    [UnitTest]
    public class HoldRequestCancelTests : PapiClientUnitTestBase
    {

        [TestMethod]
        public async Task HoldRequestCancel_EncodesBarcode_PreservesWsidAndUseridQueryStringValues()
        {
            var handler = new CapturingHttpMessageHandler();
            var client = CreateUrlEncodingClient(handler);
            var barcode = "AB C/+#?=";

            await client.HoldRequestCancelAsync(barcode, 9876, "pin", userId: 888, workstationId: 999, TestContext.CancellationToken);

            var expectedPath = $"/PAPIService/REST/public/v1/1033/100/1/patron/{WebUtility.UrlEncode(barcode)}/holdrequests/9876/cancelled";
            var expectedQuery = "?wsid=999&userid=888";

            Assert.IsNotNull(handler.LastRequest);
            Assert.AreEqual(expectedPath, handler.LastRequest!.RequestUri!.AbsolutePath);
            Assert.AreEqual(expectedQuery, handler.LastRequest.RequestUri.Query);
        }
    }
}
