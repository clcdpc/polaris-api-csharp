namespace Clc.Polaris.Api.UnitTests.EndpointRequests.Acquisitions
{
    [TestClass]
    [UnitTest]
    public sealed class JobsPurchaseOrdersPutTests : PapiClientUnitTestBase
    {
        [TestMethod]
        public async Task JobsPurchaseOrdersPutAsync_SendsProtectedPutWithJsonBody()
        {
            var handler = new CapturingHttpMessageHandler(CreatePapiResponseJson());
            var client = CreateConfiguredClient(handler);
            client.Token = CreateToken();

            await client.JobsPurchaseOrdersPutAsync(CreateValidData(), cancellationToken: TestContext.CancellationToken);

            Assert.AreEqual(HttpMethod.Put, GetLastRequest(handler).Method);
            Assert.AreEqual("/PAPIService/REST/protected/v1/1033/100/101/protected-token/jobs/purchaseorders", GetLastRequestUri(handler).AbsolutePath);
            AssertLastRequestQueryParameter(handler, "preordervalidation", "1");
            AssertLastRequestBodyContains(handler, "VendorName");
            AssertLastRequestBodyContains(handler, "LineItems");
            AssertLastRequestBodyContains(handler, "Segments");
            AssertLastRequestBodyContains(handler, "General");
            AssertLastRequestBodyContains(handler, "Materials");
        }

        [TestMethod]
        public async Task JobsPurchaseOrdersPutAsync_WithNullRequest_Throws() =>
            await Assert.ThrowsExactlyAsync<ArgumentNullException>(async () => await CreateClient().JobsPurchaseOrdersPutAsync(null!, cancellationToken: TestContext.CancellationToken));

        [TestMethod]
        public async Task JobsPurchaseOrdersPutAsync_WithInvalidPreorderValidation_Throws() =>
            await Assert.ThrowsExactlyAsync<ArgumentOutOfRangeException>(async () => await CreateClient().JobsPurchaseOrdersPutAsync(CreateValidData(), 2, cancellationToken: TestContext.CancellationToken));

        [TestMethod]
        public async Task JobsPurchaseOrdersPutAsync_WithDefaultRequest_ThrowsBeforeSendingRequest()
        {
            var handler = new CapturingHttpMessageHandler(CreatePapiResponseJson());
            var client = CreateConfiguredClient(handler);

            await Assert.ThrowsAsync<ArgumentException>(async () => await client.JobsPurchaseOrdersPutAsync(new JobsPurchaseOrdersPreorderValidationData(), cancellationToken: TestContext.CancellationToken));

            Assert.AreEqual(0, handler.RequestCount);
        }

        [TestMethod]
        public async Task JobsPurchaseOrdersPutAsync_WithMissingLineItems_ThrowsBeforeSendingRequest()
        {
            var handler = new CapturingHttpMessageHandler(CreatePapiResponseJson());
            var client = CreateConfiguredClient(handler);
            var data = CreateValidData();
            data.LineItems = new List<JobsPurchaseOrdersPreorderValidationLineItem>();

            await Assert.ThrowsAsync<ArgumentException>(async () => await client.JobsPurchaseOrdersPutAsync(data, cancellationToken: TestContext.CancellationToken));

            Assert.AreEqual(0, handler.RequestCount);
        }

        [TestMethod]
        public async Task JobsPurchaseOrdersPutAsync_WithInvalidLineItemValues_ThrowsBeforeSendingRequest()
        {
            var handler = new CapturingHttpMessageHandler(CreatePapiResponseJson());
            var client = CreateConfiguredClient(handler);
            var data = CreateValidData();
            data.LineItems![0].Title = " ";
            data.LineItems[0].ISBN = " ";

            await Assert.ThrowsAsync<ArgumentException>(async () => await client.JobsPurchaseOrdersPutAsync(data, cancellationToken: TestContext.CancellationToken));

            Assert.AreEqual(0, handler.RequestCount);
        }

        [TestMethod]
        public async Task JobsPurchaseOrdersPutAsync_WithInvalidSegmentValues_ThrowsBeforeSendingRequest()
        {
            var handler = new CapturingHttpMessageHandler(CreatePapiResponseJson());
            var client = CreateConfiguredClient(handler);
            var data = CreateValidData();
            data.LineItems![0].Segments![0].Copies = 0;

            await Assert.ThrowsExactlyAsync<ArgumentOutOfRangeException>(async () => await client.JobsPurchaseOrdersPutAsync(data, cancellationToken: TestContext.CancellationToken));

            Assert.AreEqual(0, handler.RequestCount);
        }

        [TestMethod]
        public async Task JobsPurchaseOrdersPutAsync_WithNegativeSegmentUnitPrice_ThrowsBeforeSendingRequest()
        {
            var handler = new CapturingHttpMessageHandler(CreatePapiResponseJson());
            var client = CreateConfiguredClient(handler);
            var data = CreateValidData();
            data.LineItems![0].Segments![0].UnitPrice = -1;

            await Assert.ThrowsExactlyAsync<ArgumentOutOfRangeException>(async () => await client.JobsPurchaseOrdersPutAsync(data, cancellationToken: TestContext.CancellationToken));

            Assert.AreEqual(0, handler.RequestCount);
        }

        [TestMethod]
        [DataRow("Vendor")]
        [DataRow("OrderedAtLocation")]
        [DataRow("OrderType")]
        [DataRow("PaymentMethod")]
        public async Task JobsPurchaseOrdersPutAsync_WithWhitespaceRequiredString_ThrowsBeforeSendingRequest(string propertyName)
        {
            var handler = new CapturingHttpMessageHandler(CreatePapiResponseJson());
            var client = CreateConfiguredClient(handler);
            var data = CreateValidData();
            SetRequiredString(data, propertyName, " ");

            await Assert.ThrowsAsync<ArgumentException>(async () => await client.JobsPurchaseOrdersPutAsync(data, cancellationToken: TestContext.CancellationToken));

            Assert.AreEqual(0, handler.RequestCount);
        }

        [TestMethod]
        public async Task JobsPurchaseOrdersPutAsync_WithNullLineItem_ThrowsBeforeSendingRequest()
        {
            var handler = new CapturingHttpMessageHandler(CreatePapiResponseJson());
            var client = CreateConfiguredClient(handler);
            var data = CreateValidData();
            data.LineItems![0] = null!;

            await Assert.ThrowsAsync<ArgumentException>(async () => await client.JobsPurchaseOrdersPutAsync(data, cancellationToken: TestContext.CancellationToken));

            Assert.AreEqual(0, handler.RequestCount);
        }

        [TestMethod]
        public async Task JobsPurchaseOrdersPutAsync_WithNullSegments_ThrowsBeforeSendingRequest()
        {
            var handler = new CapturingHttpMessageHandler(CreatePapiResponseJson());
            var client = CreateConfiguredClient(handler);
            var data = CreateValidData();
            data.LineItems![0].Segments = null;

            await Assert.ThrowsExactlyAsync<ArgumentNullException>(async () => await client.JobsPurchaseOrdersPutAsync(data, cancellationToken: TestContext.CancellationToken));

            Assert.AreEqual(0, handler.RequestCount);
        }

        [TestMethod]
        public async Task JobsPurchaseOrdersPutAsync_WithEmptySegments_ThrowsBeforeSendingRequest()
        {
            var handler = new CapturingHttpMessageHandler(CreatePapiResponseJson());
            var client = CreateConfiguredClient(handler);
            var data = CreateValidData();
            data.LineItems![0].Segments = new List<JobsPurchaseOrdersPreorderValidationLineItemSegment>();

            await Assert.ThrowsAsync<ArgumentException>(async () => await client.JobsPurchaseOrdersPutAsync(data, cancellationToken: TestContext.CancellationToken));

            Assert.AreEqual(0, handler.RequestCount);
        }

        [TestMethod]
        public async Task JobsPurchaseOrdersPutAsync_WithNullSegment_ThrowsBeforeSendingRequest()
        {
            var handler = new CapturingHttpMessageHandler(CreatePapiResponseJson());
            var client = CreateConfiguredClient(handler);
            var data = CreateValidData();
            data.LineItems![0].Segments![0] = null!;

            await Assert.ThrowsAsync<ArgumentException>(async () => await client.JobsPurchaseOrdersPutAsync(data, cancellationToken: TestContext.CancellationToken));

            Assert.AreEqual(0, handler.RequestCount);
        }

        [TestMethod]
        public async Task JobsPurchaseOrdersPutAsync_WithMissingCollection_ThrowsBeforeSendingRequest()
        {
            var handler = new CapturingHttpMessageHandler(CreatePapiResponseJson());
            var client = CreateConfiguredClient(handler);
            var data = CreateValidData();
            data.LineItems![0].Segments![0].Collection = " ";

            await Assert.ThrowsAsync<ArgumentException>(async () => await client.JobsPurchaseOrdersPutAsync(data, cancellationToken: TestContext.CancellationToken));

            Assert.AreEqual(0, handler.RequestCount);
        }

        [TestMethod]
        public async Task JobsPurchaseOrdersPutAsync_WithMissingFund_ThrowsBeforeSendingRequest()
        {
            var handler = new CapturingHttpMessageHandler(CreatePapiResponseJson());
            var client = CreateConfiguredClient(handler);
            var data = CreateValidData();
            data.LineItems![0].Segments![0].Fund = " ";

            await Assert.ThrowsAsync<ArgumentException>(async () => await client.JobsPurchaseOrdersPutAsync(data, cancellationToken: TestContext.CancellationToken));

            Assert.AreEqual(0, handler.RequestCount);
        }

        [TestMethod]
        public async Task JobsPurchaseOrdersPutAsync_WithInvalidLineItemCopies_ThrowsBeforeSendingRequest()
        {
            var handler = new CapturingHttpMessageHandler(CreatePapiResponseJson());
            var client = CreateConfiguredClient(handler);
            var data = CreateValidData();
            data.LineItems![0].Copies = 0;

            await Assert.ThrowsExactlyAsync<ArgumentOutOfRangeException>(async () => await client.JobsPurchaseOrdersPutAsync(data, cancellationToken: TestContext.CancellationToken));

            Assert.AreEqual(0, handler.RequestCount);
        }

        private static JobsPurchaseOrdersPreorderValidationData CreateValidData() => new()
        {
            Vendor = "VendorName",
            OrderedAtLocation = "Main",
            OrderType = "Firm",
            PaymentMethod = "Account",
            LineItems = new List<JobsPurchaseOrdersPreorderValidationLineItem>
            {
                new()
                {
                    Title = "Title",
                    ISBN = "9780000000000",
                    Copies = 1,
                    Segments = new List<JobsPurchaseOrdersPreorderValidationLineItemSegment>
                    {
                        new()
                        {
                            Collection = "General",
                            Fund = "Materials",
                            Copies = 1,
                            UnitPrice = 12.99m
                        }
                    }
                }
            }
        };

        private static void SetRequiredString(JobsPurchaseOrdersPreorderValidationData data, string propertyName, string value)
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
                default:
                    throw new ArgumentOutOfRangeException(nameof(propertyName), propertyName, "Unknown required string property.");
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
