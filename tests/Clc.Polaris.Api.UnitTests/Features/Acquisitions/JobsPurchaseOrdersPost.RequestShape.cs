namespace Clc.Polaris.Api.UnitTests.Features.Acquisitions
{
    [TestClass]
    [UnitTest]
    public sealed class JobsPurchaseOrdersPostRequestShape : PapiClientUnitTestBase
    {
        [TestMethod]
        public async Task JobsPurchaseOrdersPostAsync_SendsProtectedPostWithJsonBody()
        {
            var handler = new CapturingHttpMessageHandler(CreatePapiResponseJson());
            var client = CreateConfiguredClient(handler);
            client.Token = CreateToken();

            await client.JobsPurchaseOrdersPostAsync(CreateValidData(), cancellationToken: TestContext.CancellationToken);

            Assert.AreEqual(HttpMethod.Post, GetLastRequest(handler).Method);
            Assert.AreEqual("/PAPIService/REST/protected/v1/1033/100/101/protected-token/jobs/purchaseorders", GetLastRequestUri(handler).AbsolutePath);
            AssertLastRequestBodyContains(handler, "VendorName");
            AssertLastRequestBodyContains(handler, "PO-1");
            AssertLastRequestBodyContains(handler, "MARCLineItems");
            AssertLastRequestBodyContains(handler, "leader");
        }

        [TestMethod]
        public async Task JobsPurchaseOrdersPostAsync_WithNullRequest_Throws() =>
            await Assert.ThrowsExactlyAsync<ArgumentNullException>(async () => await CreateClient().JobsPurchaseOrdersPostAsync(null!, cancellationToken: TestContext.CancellationToken));

        [TestMethod]
        public async Task JobsPurchaseOrdersPostAsync_WithDefaultRequest_ThrowsBeforeSendingRequest()
        {
            var handler = new CapturingHttpMessageHandler(CreatePapiResponseJson());
            var client = CreateConfiguredClient(handler);

            await Assert.ThrowsAsync<ArgumentException>(async () => await client.JobsPurchaseOrdersPostAsync(new JobsPurchaseOrdersCreateData(), cancellationToken: TestContext.CancellationToken));

            Assert.AreEqual(0, handler.RequestCount);
        }

        [TestMethod]
        public async Task JobsPurchaseOrdersPostAsync_WithMissingRequiredString_ThrowsBeforeSendingRequest()
        {
            var handler = new CapturingHttpMessageHandler(CreatePapiResponseJson());
            var client = CreateConfiguredClient(handler);
            var data = CreateValidData();
            data.Vendor = " ";

            await Assert.ThrowsAsync<ArgumentException>(async () => await client.JobsPurchaseOrdersPostAsync(data, cancellationToken: TestContext.CancellationToken));

            Assert.AreEqual(0, handler.RequestCount);
        }

        [TestMethod]
        public async Task JobsPurchaseOrdersPostAsync_WithMissingMarcLineItems_ThrowsBeforeSendingRequest()
        {
            var handler = new CapturingHttpMessageHandler(CreatePapiResponseJson());
            var client = CreateConfiguredClient(handler);
            var data = CreateValidData();
            data.MARCLineItems = new List<JobsPurchaseOrdersMarcLineItem>();

            await Assert.ThrowsAsync<ArgumentException>(async () => await client.JobsPurchaseOrdersPostAsync(data, cancellationToken: TestContext.CancellationToken));

            Assert.AreEqual(0, handler.RequestCount);
        }

        [TestMethod]
        public async Task JobsPurchaseOrdersPostAsync_WithInvalidLineItemCopies_ThrowsBeforeSendingRequest()
        {
            var handler = new CapturingHttpMessageHandler(CreatePapiResponseJson());
            var client = CreateConfiguredClient(handler);
            var data = CreateValidData();
            data.MARCLineItems![0].Copies = 0;

            await Assert.ThrowsExactlyAsync<ArgumentOutOfRangeException>(async () => await client.JobsPurchaseOrdersPostAsync(data, cancellationToken: TestContext.CancellationToken));

            Assert.AreEqual(0, handler.RequestCount);
        }

        [TestMethod]
        public async Task JobsPurchaseOrdersPostAsync_WithMissingMarcRecord_ThrowsBeforeSendingRequest()
        {
            var handler = new CapturingHttpMessageHandler(CreatePapiResponseJson());
            var client = CreateConfiguredClient(handler);
            var data = CreateValidData();
            data.MARCLineItems![0].MarcRecord = null;

            await Assert.ThrowsAsync<ArgumentException>(async () => await client.JobsPurchaseOrdersPostAsync(data, cancellationToken: TestContext.CancellationToken));

            Assert.AreEqual(0, handler.RequestCount);
        }


        [TestMethod]
        [DataRow("Vendor")]
        [DataRow("OrderedAtLocation")]
        [DataRow("OrderType")]
        [DataRow("PaymentMethod")]
        [DataRow("PONumber")]
        public async Task JobsPurchaseOrdersPostAsync_WithWhitespaceRequiredString_ThrowsBeforeSendingRequest(string propertyName)
        {
            var handler = new CapturingHttpMessageHandler(CreatePapiResponseJson());
            var client = CreateConfiguredClient(handler);
            var data = CreateValidData();
            SetRequiredString(data, propertyName, " ");

            await Assert.ThrowsAsync<ArgumentException>(async () => await client.JobsPurchaseOrdersPostAsync(data, cancellationToken: TestContext.CancellationToken));

            Assert.AreEqual(0, handler.RequestCount);
        }

        [TestMethod]
        public async Task JobsPurchaseOrdersPostAsync_WithNullMarcLineItem_ThrowsBeforeSendingRequest()
        {
            var handler = new CapturingHttpMessageHandler(CreatePapiResponseJson());
            var client = CreateConfiguredClient(handler);
            var data = CreateValidData();
            data.MARCLineItems![0] = null!;

            await Assert.ThrowsAsync<ArgumentException>(async () => await client.JobsPurchaseOrdersPostAsync(data, cancellationToken: TestContext.CancellationToken));

            Assert.AreEqual(0, handler.RequestCount);
        }

        [TestMethod]
        public async Task JobsPurchaseOrdersPostAsync_WithEmptyMarcRecord_ThrowsBeforeSendingRequest()
        {
            var handler = new CapturingHttpMessageHandler(CreatePapiResponseJson());
            var client = CreateConfiguredClient(handler);
            var data = CreateValidData();
            data.MARCLineItems![0].MarcRecord = new MarcRecord();

            await Assert.ThrowsAsync<ArgumentException>(async () => await client.JobsPurchaseOrdersPostAsync(data, cancellationToken: TestContext.CancellationToken));

            Assert.AreEqual(0, handler.RequestCount);
        }
        private static JobsPurchaseOrdersCreateData CreateValidData() => new()
        {
            Vendor = "VendorName",
            OrderedAtLocation = "Main",
            OrderType = "Firm",
            PaymentMethod = "Account",
            PONumber = "PO-1",
            MARCLineItems = new List<JobsPurchaseOrdersMarcLineItem>
            {
                new()
                {
                    Copies = 1,
                    MarcRecord = new MarcRecord
                    {
                        Leader = "leader"
                    }
                }
            }
        };


        private static void SetRequiredString(JobsPurchaseOrdersCreateData data, string propertyName, string value)
        {
            switch (propertyName)
            {
                case "Vendor":
                    data.Vendor = value;
                    break;
                case "OrderedAtLocation":
                    data.OrderedAtLocation = value;
                    break;
                case "OrderType":
                    data.OrderType = value;
                    break;
                case "PaymentMethod":
                    data.PaymentMethod = value;
                    break;
                case "PONumber":
                    data.PONumber = value;
                    break;
            }
        }
        private static PapiClient CreateConfiguredClient(CapturingHttpMessageHandler handler)
        {
            var client = CreateClient(handler);
            client.OrganizationId = 101;
            return client;
        }

        private static ProtectedToken CreateToken() => new() { AccessToken = "protected-token", AccessSecret = "protected-secret", ExpirationDate = ValidProtectedTokenExpirationDate };
    }
}
