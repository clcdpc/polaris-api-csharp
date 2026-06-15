using System.Net;

namespace Clc.Polaris.Api.UnitTests.Methods.RequestShape
{
    [TestClass]
    [UnitTest]
    public sealed class PatronNotesRequestShapeTests : PapiClientUnitTestBase
    {
        [TestMethod]
        public async Task PatronNotesGetAsync_UsesEncodedBarcodePathAndPasswordHash()
        {
            var handler = new CapturingHttpMessageHandler(CreateJson(new
            {
                PAPIErrorCode = 0,
                PatronNotes = new
                {
                    BlockingStatusNotes = "blocked",
                    NonBlockingStatusNotes = "note"
                }
            }));
            var client = CreateClient(handler);
            var barcode = "AB C/+#?=";

            var response = await client.PatronNotesGetAsync(barcode, "pin", TestContext.CancellationToken);

            Assert.AreEqual(HttpMethod.Get, GetLastRequest(handler).Method);
            Assert.AreEqual($"/PAPIService/REST/public/v1/1033/100/1/patron/{WebUtility.UrlEncode(barcode)}/notes", GetLastRequestUri(handler).AbsolutePath);
            AssertAuthorizationHashesSentUri(GetLastRequest(handler), "pin");
            Assert.AreEqual("blocked", response.Data?.PatronNotes?.BlockingStatusNotes);
            Assert.AreEqual("note", response.Data?.PatronNotes?.NonBlockingStatusNotes);
        }
    }
}
