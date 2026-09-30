using System.Net;

namespace Clc.Polaris.Api.UnitTests.EndpointRequests.Patron
{
    [TestClass]
    [UnitTest]
    public class PatronUpdateTests : PapiClientUnitTestBase
    {
        [TestMethod]
        public async Task PatronUpdate_RequestShape_IsStable()
        {
            var handler = new CapturingHttpMessageHandler(CreatePapiResponseJson());
            var client = CreateClient(handler);

            var response = await client.PatronUpdateAsync("AB C/+#?=", new PatronUpdateParams { EmailAddress = "patron@example.test" }, "1234", ignoresa: true, TestContext.CancellationToken);

            Assert.IsNotNull(response);
            Assert.AreEqual(HttpMethod.Put, handler.LastRequest!.Method);
            var encodedBarcode = WebUtility.UrlEncode("AB C/+#?=");
            Assert.Contains($"/public/v1/1033/100/1/patron/{encodedBarcode}", handler.LastRequest.RequestUri!.AbsolutePath);
            var query = ParseQuery(handler.LastRequest.RequestUri.Query);
            Assert.AreEqual("True", query["ignoresa"]);
            Assert.IsNotNull(handler.LastRequest.Content);
            Assert.IsTrue(handler.LastRequest.Headers.Contains("PolarisDate"));
            Assert.IsTrue(handler.LastRequest.Headers.Contains("Authorization"));
            Assert.IsNotNull(handler.LastRequestContent);
            Assert.Contains("patron@example.test", handler.LastRequestContent);
        }

        [TestMethod]
        public async Task PatronUpdateAsync_DefaultLogonIds_UsesConfiguredClientIds()
        {
            var handler = new CapturingHttpMessageHandler(CreatePapiResponseJson());
            var client = CreateClient(handler);
            client.OrganizationId = 101;
            client.UserId = 202;
            client.WorkstationId = 303;

            await client.PatronUpdateAsync("123", new PatronUpdateParams(), cancellationToken: TestContext.CancellationToken);

            AssertLastRequestBodyJsonPropertyValue(handler, nameof(PatronUpdateParams.LogonBranchId), 101);
            AssertLastRequestBodyJsonPropertyValue(handler, nameof(PatronUpdateParams.LogonUserId), 202);
            AssertLastRequestBodyJsonPropertyValue(handler, nameof(PatronUpdateParams.LogonWorkstationId), 303);
        }

        [TestMethod]
        public async Task PatronUpdateAsync_ExplicitLogonIds_PreservesSuppliedValues()
        {
            var handler = new CapturingHttpMessageHandler(CreatePapiResponseJson());
            var client = CreateClient(handler);
            var updateParams = new PatronUpdateParams
            {
                LogonBranchId = 11,
                LogonUserId = 22,
                LogonWorkstationId = 33
            };

            await client.PatronUpdateAsync("123", updateParams, cancellationToken: TestContext.CancellationToken);

            AssertLastRequestBodyJsonPropertyValue(handler, nameof(PatronUpdateParams.LogonBranchId), 11);
            AssertLastRequestBodyJsonPropertyValue(handler, nameof(PatronUpdateParams.LogonUserId), 22);
            AssertLastRequestBodyJsonPropertyValue(handler, nameof(PatronUpdateParams.LogonWorkstationId), 33);
        }

        [TestMethod]
        [DataRow(nameof(PatronUpdateParams.LogonBranchId), 0)]
        [DataRow(nameof(PatronUpdateParams.LogonUserId), -1)]
        [DataRow(nameof(PatronUpdateParams.LogonWorkstationId), 0)]
        public async Task PatronUpdateAsync_InvalidLogonIds_ThrowsArgumentOutOfRangeException(string propertyName, int value)
        {
            var client = CreateClient();
            var updateParams = new PatronUpdateParams();
            switch (propertyName)
            {
                case nameof(PatronUpdateParams.LogonBranchId):
                    updateParams.LogonBranchId = value;
                    break;
                case nameof(PatronUpdateParams.LogonUserId):
                    updateParams.LogonUserId = value;
                    break;
                case nameof(PatronUpdateParams.LogonWorkstationId):
                    updateParams.LogonWorkstationId = value;
                    break;
            }

            await Assert.ThrowsExactlyAsync<ArgumentOutOfRangeException>(async () =>
                await client.PatronUpdateAsync("123", updateParams, cancellationToken: TestContext.CancellationToken));
        }

    }
}
