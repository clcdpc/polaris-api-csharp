namespace Clc.Polaris.Api.Tests
{
    [TestClass]
    [UnitTest]
    public sealed class ItemRenewAllForPatronTests : PapiClientTestBase
    {
        [TestMethod]
        public async Task ItemRenewAllForPatronAsync_SendsRenewRequestWithZeroItemId()
        {
            var handler = new CapturingHttpMessageHandler(CreateJson(new { PAPIErrorCode = 0 }));
            var client = CreateClient(handler);
            var barcode = "21945001234567";
            var password = "mypassword";

            var response = await client.ItemRenewAllForPatronAsync(barcode, password, cancellationToken: TestContext.CancellationToken);

            Assert.IsNotNull(response.Data);
            Assert.AreEqual(0, response.Data.PAPIErrorCode);
            Assert.AreEqual(HttpMethod.Put, GetLastRequest(handler).Method);
            Assert.AreEqual("/PAPIService/REST/public/v1/1033/100/1/patron/21945001234567/itemsout/0", GetLastRequestUri(handler).AbsolutePath);
            AssertLastRequestBodyJsonPropertyValue(handler, "LogonBranchID", client.OrganizationId);
            AssertLastRequestBodyJsonPropertyValue(handler, "LogonUserID", client.UserId);
            AssertLastRequestBodyJsonPropertyValue(handler, "LogonWorkstationID", client.WorkstationId);
        }
    }
}