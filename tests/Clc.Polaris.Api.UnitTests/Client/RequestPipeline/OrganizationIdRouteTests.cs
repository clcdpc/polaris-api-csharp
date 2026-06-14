namespace Clc.Polaris.Api.UnitTests.PapiClientBehavior
{
    [TestClass]
    [UnitTest]
    public sealed class OrganizationIdRouteTests : PapiClientUnitTestBase
    {
        private const int TestOrganizationId = 73;
        private const int TestUserId = 11;
        private const int TestWorkstationId = 22;
        private const string TestProtectedAccessToken = "protected-token";
        private const string TestProtectedAccessSecret = "protected-secret";
        private const string TestBarcode = "PAT123456";

        [TestMethod]
        public async Task AuthenticatePatronAsync_UsesConfiguredOrganizationIdInRoute()
        {
            var handler = new CapturingHttpMessageHandler(CreateJson(new { PAPIErrorCode = 0, AccessToken = "patron-token", AccessSecret = "patron-secret", PatronID = 123 }));
            var client = CreateOrganizationIdRouteClient(handler);

            await client.AuthenticatePatronAsync(TestBarcode, "1234", cancellationToken: TestContext.CancellationToken);

            AssertPathContainsConfiguredOrganization(handler, "/public/v1/1033/100/73/authenticator/patron");
        }

        [TestMethod]
        public async Task CreatePatronBlocksAsync_UsesConfiguredOrganizationIdInRoute()
        {
            var handler = new CapturingHttpMessageHandler(CreatePapiResponseJson());
            var client = CreateOrganizationIdRouteClient(handler);

            await client.CreatePatronBlocksAsync(TestBarcode, (BlockType)1, "test block", cancellationToken: TestContext.CancellationToken);

            AssertPathContainsConfiguredOrganization(handler, $"/protected/v1/1033/100/73/{TestProtectedAccessToken}/patron/{TestBarcode}/blocks");
            AssertLastRequestQueryParameter(handler, "userid", "11");
            AssertLastRequestQueryParameter(handler, "wsid", "22");
        }

        [TestMethod]
        public async Task HoldRequestCancelAsync_UsesConfiguredOrganizationIdInRoute()
        {
            var handler = new CapturingHttpMessageHandler(CreatePapiResponseJson());
            var client = CreateOrganizationIdRouteClient(handler);

            await client.HoldRequestCancelAsync(TestBarcode, requestId: 12345, password: "1234", cancellationToken: TestContext.CancellationToken);

            AssertPathContainsConfiguredOrganization(handler, $"/public/v1/1033/100/73/patron/{TestBarcode}/holdrequests/12345/cancelled");
            AssertLastRequestQueryParameter(handler, "userid", "11");
            AssertLastRequestQueryParameter(handler, "wsid", "22");
        }

        [TestMethod]
        public async Task ItemUpdateBarcodeAsync_WithItemRecordId_UsesConfiguredOrganizationIdInRoute()
        {
            var handler = new CapturingHttpMessageHandler(CreatePapiResponseJson());
            var client = CreateOrganizationIdRouteClient(handler);

            await client.ItemUpdateBarcodeAsync(newBarcode: "NEW123456", itemRecordId: 987654, cancellationToken: TestContext.CancellationToken);

            AssertPathContainsConfiguredOrganization(handler, $"/protected/v1/1033/100/73/{TestProtectedAccessToken}/cataloging/items/987654/barcode");
            AssertLastRequestQueryParameter(handler, "wsid", "22");
            AssertLastRequestBodyJsonPropertyValue(handler, "ItemBarcode", "NEW123456");
            AssertLastRequestBodyJsonPropertyValue(handler, "TransactionBranchId", 73);
        }

        [TestMethod]
        public async Task ItemUpdateBarcodeAsync_WithOldBarcode_UsesConfiguredOrganizationIdInRouteAndIsBarcodeQuery()
        {
            var handler = new CapturingHttpMessageHandler(CreatePapiResponseJson());
            var client = CreateOrganizationIdRouteClient(handler);

            await client.ItemUpdateBarcodeAsync(newBarcode: "NEW123456", oldBarcode: "OLD123456", cancellationToken: TestContext.CancellationToken);

            AssertPathContainsConfiguredOrganization(handler, $"/protected/v1/1033/100/73/{TestProtectedAccessToken}/cataloging/items/OLD123456/barcode");
            AssertLastRequestQueryParameter(handler, "wsid", "22");
            AssertLastRequestQueryParameter(handler, "isBarcode", "1");
            AssertLastRequestBodyJsonPropertyValue(handler, "ItemBarcode", "NEW123456");
            AssertLastRequestBodyJsonPropertyValue(handler, "TransactionBranchId", 73);
        }

        [TestMethod]
        public async Task ItemUpdateBarcodeAsync_WithExplicitTransactionBranchId_UsesConfiguredOrganizationIdInRouteAndTransactionBranchInBody()
        {
            var handler = new CapturingHttpMessageHandler(CreatePapiResponseJson());
            var client = CreateOrganizationIdRouteClient(handler);

            await client.ItemUpdateBarcodeAsync(newBarcode: "NEW123456", itemRecordId: 987654, transactionBranchId: 55, cancellationToken: TestContext.CancellationToken);

            AssertPathContainsConfiguredOrganization(handler, $"/protected/v1/1033/100/73/{TestProtectedAccessToken}/cataloging/items/987654/barcode");
            AssertLastRequestBodyJsonPropertyValue(handler, "ItemBarcode", "NEW123456");
            AssertLastRequestBodyJsonPropertyValue(handler, "TransactionBranchId", 55);
        }

        [TestMethod]
        public async Task PatronAccountCreateCreditAsync_UsesConfiguredOrganizationIdInRoute()
        {
            var handler = new CapturingHttpMessageHandler(CreatePapiResponseJson());
            var client = CreateOrganizationIdRouteClient(handler);

            await client.PatronAccountCreateCreditAsync(TestBarcode, txnAmount: 1.25, paymentMethod: PaymentMethod.Cash, cancellationToken: TestContext.CancellationToken);

            AssertPathContainsConfiguredOrganization(handler, $"/protected/v1/1033/100/73/{TestProtectedAccessToken}/patron/{TestBarcode}/account/createcredit");
            AssertLastRequestQueryParameter(handler, "userid", "11");
            AssertLastRequestQueryParameter(handler, "wsid", "22");
        }

        [TestMethod]
        public async Task PatronAccountDepositCreditAsync_UsesConfiguredOrganizationIdInRoute()
        {
            var handler = new CapturingHttpMessageHandler(CreatePapiResponseJson());
            var client = CreateOrganizationIdRouteClient(handler);

            await client.PatronAccountDepositCreditAsync(TestBarcode, txnAmount: 1.25, cancellationToken: TestContext.CancellationToken);

            AssertPathContainsConfiguredOrganization(handler, $"/protected/v1/1033/100/73/{TestProtectedAccessToken}/patron/{TestBarcode}/account/lumpsumdepositcredit");
            AssertLastRequestQueryParameter(handler, "userid", "11");
            AssertLastRequestQueryParameter(handler, "wsid", "22");
        }

        [TestMethod]
        public async Task PatronAccountPayAllAsync_UsesConfiguredOrganizationIdInRoute()
        {
            var handler = new CapturingHttpMessageHandler(CreatePapiResponseJson());
            var client = CreateOrganizationIdRouteClient(handler);

            await client.PatronAccountPayAllAsync(TestBarcode, txnAmount: 1.25, paymentMethod: PaymentMethod.Cash, workstationId: null, userId: null, cancellationToken: TestContext.CancellationToken);

            AssertPathContainsConfiguredOrganization(handler, $"/protected/v1/1033/100/73/{TestProtectedAccessToken}/patron/{TestBarcode}/account/lumpsumpayment");
            AssertLastRequestQueryParameter(handler, "userid", "11");
            AssertLastRequestQueryParameter(handler, "wsid", "22");
        }

        [TestMethod]
        public async Task PatronAccountPayAsync_UsesConfiguredOrganizationIdInRoute()
        {
            var handler = new CapturingHttpMessageHandler(CreatePapiResponseJson());
            var client = CreateOrganizationIdRouteClient(handler);

            await client.PatronAccountPayAsync(TestBarcode, txnId: 12345, txnAmount: 1.25, paymentMethod: PaymentMethod.Cash, cancellationToken: TestContext.CancellationToken);

            AssertPathContainsConfiguredOrganization(handler, $"/protected/v1/1033/100/73/{TestProtectedAccessToken}/patron/{TestBarcode}/account/12345/pay");
            AssertLastRequestQueryParameter(handler, "userid", "11");
            AssertLastRequestQueryParameter(handler, "wsid", "22");
        }

        [TestMethod]
        public async Task PatronAccountRefundCreditAsync_UsesConfiguredOrganizationIdInRoute()
        {
            var handler = new CapturingHttpMessageHandler(CreatePapiResponseJson());
            var client = CreateOrganizationIdRouteClient(handler);

            await client.PatronAccountRefundCreditAsync(TestBarcode, txnAmount: 1.25, cancellationToken: TestContext.CancellationToken);

            AssertPathContainsConfiguredOrganization(handler, $"/protected/v1/1033/100/73/{TestProtectedAccessToken}/patron/{TestBarcode}/account/lumpsumrefundcredit");
            AssertLastRequestQueryParameter(handler, "userid", "11");
            AssertLastRequestQueryParameter(handler, "wsid", "22");
        }

        [TestMethod]
        public async Task PatronAccountVoidAsync_UsesConfiguredOrganizationIdInRoute()
        {
            var handler = new CapturingHttpMessageHandler(CreatePapiResponseJson());
            var client = CreateOrganizationIdRouteClient(handler);

            await client.PatronAccountVoidAsync(TestBarcode, paymentTxnId: 12345, cancellationToken: TestContext.CancellationToken);

            AssertPathContainsConfiguredOrganization(handler, $"/protected/v1/1033/100/73/{TestProtectedAccessToken}/patron/{TestBarcode}/account/12345/void/payment");
            AssertLastRequestQueryParameter(handler, "userid", "11");
            AssertLastRequestQueryParameter(handler, "wsid", "22");
        }

        [TestMethod]
        public async Task PatronCirculateBlocksGetAsync_UsesConfiguredOrganizationIdInRoute()
        {
            var handler = new CapturingHttpMessageHandler(CreatePapiResponseJson());
            var client = CreateOrganizationIdRouteClient(handler);

            await client.PatronCirculateBlocksGetAsync(TestBarcode, password: "1234", cancellationToken: TestContext.CancellationToken);

            AssertPathContainsConfiguredOrganization(handler, $"/public/v1/1033/100/73/patron/{TestBarcode}/circulationblocks");
        }

        [TestMethod]
        public async Task PatronItemsOutGetAsync_UsesConfiguredOrganizationIdInRoute()
        {
            var handler = new CapturingHttpMessageHandler(CreatePapiResponseJson());
            var client = CreateOrganizationIdRouteClient(handler);

            await client.PatronItemsOutGetAsync(TestBarcode, PatronItemsOutGetStatus.All, password: "1234", cancellationToken: TestContext.CancellationToken);

            AssertPathContainsConfiguredOrganization(handler, $"/public/v1/1033/100/73/patron/{TestBarcode}/itemsout/All");
        }

        [TestMethod]
        public async Task PatronValidateAsync_UsesConfiguredOrganizationIdInRoute()
        {
            var handler = new CapturingHttpMessageHandler(CreatePapiResponseJson());
            var client = CreateOrganizationIdRouteClient(handler);

            await client.PatronValidateAsync(TestBarcode, password: "1234", cancellationToken: TestContext.CancellationToken);

            AssertPathContainsConfiguredOrganization(handler, $"/public/v1/1033/100/73/patron/{TestBarcode}");
        }

        [TestMethod]
        public async Task RecordSetContentPutAsync_UsesConfiguredOrganizationIdInRoute()
        {
            var handler = new CapturingHttpMessageHandler(CreatePapiResponseJson());
            var client = CreateOrganizationIdRouteClient(handler);

            await client.RecordSetContentPutAsync(recordSetId: 12345, records: new[] { 111, 222 }, action: (RecordSetContentPutActions)0, cancellationToken: TestContext.CancellationToken);

            AssertPathContainsConfiguredOrganization(handler, $"/protected/v1/1033/100/73/{TestProtectedAccessToken}/recordsets/12345");
            AssertLastRequestQueryParameter(handler, "userid", "11");
            AssertLastRequestQueryParameter(handler, "wsid", "22");
        }

        [TestMethod]
        public async Task RecordSetRecordsGetAsync_UsesConfiguredOrganizationIdInRoute()
        {
            var handler = new CapturingHttpMessageHandler(CreatePapiResponseJson());
            var client = CreateOrganizationIdRouteClient(handler);

            await client.RecordSetRecordsGetAsync(recordSetId: 12345, userId: TestUserId, workstationId: TestWorkstationId, startIndex: 0, numRecords: 1000, cancellationToken: TestContext.CancellationToken);

            AssertPathContainsConfiguredOrganization(handler, $"/protected/v1/1033/100/73/{TestProtectedAccessToken}/recordsets/12345/records");
            AssertLastRequestQueryParameter(handler, "startIndex", "0");
            AssertLastRequestQueryParameter(handler, "numRecords", "1000");
            AssertLastRequestQueryParameter(handler, "userid", "11");
            AssertLastRequestQueryParameter(handler, "wsid", "22");
        }

        [TestMethod]
        public async Task UpdatePatronNotesDataAsync_UsesConfiguredOrganizationIdInRoute()
        {
            var handler = new CapturingHttpMessageHandler(CreatePapiResponseJson());
            var client = CreateOrganizationIdRouteClient(handler);

            await client.UpdatePatronNotesDataAsync(TestBarcode, nonBlockingNote: "test note", cancellationToken: TestContext.CancellationToken);

            AssertPathContainsConfiguredOrganization(handler, $"/protected/v1/1033/100/73/{TestProtectedAccessToken}/patron/{TestBarcode}/notes");
            AssertLastRequestQueryParameter(handler, "wsid", "22");
        }

        [TestMethod]
        public async Task UpdatePickupBranchIDAsync_UsesConfiguredOrganizationIdInRoute()
        {
            var handler = new CapturingHttpMessageHandler(CreatePapiResponseJson());
            var client = CreateOrganizationIdRouteClient(handler);

            await client.UpdatePickupBranchIDAsync(TestBarcode, requestId: 12345, pickupBranchId: 44, password: "1234", cancellationToken: TestContext.CancellationToken);

            AssertPathContainsConfiguredOrganization(handler, $"/public/v1/1033/100/73/patron/{TestBarcode}/holdrequests/12345/pickupbranch");
            AssertLastRequestQueryParameter(handler, "userid", "11");
            AssertLastRequestQueryParameter(handler, "wsid", "22");
            AssertLastRequestQueryParameter(handler, "pickupbranchid", "44");
        }

        [TestMethod]
        public async Task ChangedOperationalMethods_DoNotUseHardcodedOrganizationOne_WhenConfiguredOrganizationDiffers()
        {
            var cases = new (string Name, Func<PapiClient, Task> Execute)[]
            {
                ("AuthenticatePatronAsync", client => client.AuthenticatePatronAsync(TestBarcode, "1234", cancellationToken: TestContext.CancellationToken)),
                ("CreatePatronBlocksAsync", client => client.CreatePatronBlocksAsync(TestBarcode, (BlockType)1, "test block", cancellationToken: TestContext.CancellationToken)),
                ("HoldRequestCancelAsync", client => client.HoldRequestCancelAsync(TestBarcode, 12345, "1234", cancellationToken: TestContext.CancellationToken)),
                ("ItemUpdateBarcodeAsync", client => client.ItemUpdateBarcodeAsync("NEW123456", itemRecordId: 987654, cancellationToken: TestContext.CancellationToken)),
                ("PatronAccountCreateCreditAsync", client => client.PatronAccountCreateCreditAsync(TestBarcode, 1.25, PaymentMethod.Cash, cancellationToken: TestContext.CancellationToken)),
                ("PatronAccountDepositCreditAsync", client => client.PatronAccountDepositCreditAsync(TestBarcode, 1.25, cancellationToken: TestContext.CancellationToken)),
                ("PatronAccountPayAllAsync", client => client.PatronAccountPayAllAsync(TestBarcode, 1.25, PaymentMethod.Cash, workstationId: null, userId: null, cancellationToken: TestContext.CancellationToken)),
                ("PatronAccountPayAsync", client => client.PatronAccountPayAsync(TestBarcode, 12345, 1.25, PaymentMethod.Cash, cancellationToken: TestContext.CancellationToken)),
                ("PatronAccountRefundCreditAsync", client => client.PatronAccountRefundCreditAsync(TestBarcode, 1.25, cancellationToken: TestContext.CancellationToken)),
                ("PatronAccountVoidAsync", client => client.PatronAccountVoidAsync(TestBarcode, 12345, cancellationToken: TestContext.CancellationToken)),
                ("PatronCirculateBlocksGetAsync", client => client.PatronCirculateBlocksGetAsync(TestBarcode, "1234", cancellationToken: TestContext.CancellationToken)),
                ("PatronItemsOutGetAsync", client => client.PatronItemsOutGetAsync(TestBarcode, PatronItemsOutGetStatus.All, "1234", cancellationToken: TestContext.CancellationToken)),
                ("PatronValidateAsync", client => client.PatronValidateAsync(TestBarcode, "1234", cancellationToken: TestContext.CancellationToken)),
                ("RecordSetContentPutAsync", client => client.RecordSetContentPutAsync(12345, new[] { 111, 222 }, (RecordSetContentPutActions)0, cancellationToken: TestContext.CancellationToken)),
                ("RecordSetRecordsGetAsync", client => client.RecordSetRecordsGetAsync(12345, TestUserId, TestWorkstationId, 0, 1000, cancellationToken: TestContext.CancellationToken)),
                ("UpdatePatronNotesDataAsync", client => client.UpdatePatronNotesDataAsync(TestBarcode, nonBlockingNote: "test note", cancellationToken: TestContext.CancellationToken)),
                ("UpdatePickupBranchIDAsync", client => client.UpdatePickupBranchIDAsync(TestBarcode, 12345, 44, "1234", cancellationToken: TestContext.CancellationToken)),
            };

            foreach (var testCase in cases)
            {
                var handler = new CapturingHttpMessageHandler(CreatePapiResponseJson());
                var client = CreateOrganizationIdRouteClient(handler);

                await testCase.Execute(client);

                AssertPathUsesConfiguredOrganization(handler, testCase.Name);
            }
        }

        private static PapiClient CreateOrganizationIdRouteClient(CapturingHttpMessageHandler handler)
        {
            var client = CreateClient(handler);
            client.OrganizationId = TestOrganizationId;
            client.UserId = TestUserId;
            client.WorkstationId = TestWorkstationId;
            client.Token = new ProtectedToken
            {
                AccessToken = TestProtectedAccessToken,
                AccessSecret = TestProtectedAccessSecret,
                ExpirationDate = DateTime.UtcNow.AddHours(1)
            };

            return client;
        }

        private static void AssertPathContainsConfiguredOrganization(CapturingHttpMessageHandler handler, string expectedPath)
        {
            AssertLastRequestPathContains(handler, expectedPath);

            var actualPath = GetLastRequestUri(handler).AbsolutePath;

            Assert.Contains($"/100/{TestOrganizationId}/", actualPath);
            Assert.IsFalse(actualPath.Contains("/100/1/", StringComparison.Ordinal), $"Request path still used hardcoded organization 1. Actual path: {actualPath}");
        }

        private static void AssertPathUsesConfiguredOrganization(CapturingHttpMessageHandler handler, string methodName)
        {
            var actualPath = GetLastRequestUri(handler).AbsolutePath;

            Assert.Contains($"/100/{TestOrganizationId}/", actualPath);
            Assert.IsFalse(actualPath.Contains("/100/1/", StringComparison.Ordinal), $"{methodName} still used hardcoded organization 1. Actual path: {actualPath}");
        }

        [TestMethod]
        public async Task ApiVersionGetAsync_UsesConfiguredOrganizationIdInRoute()
        {
            var handler = new CapturingHttpMessageHandler(CreateJson(new { PAPIErrorCode = 0, Version = "1.0" }));
            var client = CreateOrganizationIdRouteClient(handler);

            await client.ApiVersionGetAsync(cancellationToken: TestContext.CancellationToken);

            AssertPathContainsConfiguredOrganization(handler, "/public/v1/1033/100/73/api");
        }

        [TestMethod]
        public async Task ApiKeyValidateAsync_UsesConfiguredOrganizationIdInRoute()
        {
            var handler = new CapturingHttpMessageHandler(CreatePapiResponseJson());
            var client = CreateOrganizationIdRouteClient(handler);

            await client.ApiKeyValidateAsync(cancellationToken: TestContext.CancellationToken);

            AssertPathContainsConfiguredOrganization(handler, "/public/v1/1033/100/73/apikeyvalidate");
        }

        [TestMethod]
        public async Task AuthenticateStaffUserAsync_UsesConfiguredOrganizationIdInRoute()
        {
            var handler = new CapturingHttpMessageHandler(CreateProtectedTokenJson(TestProtectedAccessToken, TestProtectedAccessSecret, ValidProtectedTokenExpirationDate));
            var client = CreateOrganizationIdRouteClient(handler);

            await client.AuthenticateStaffUserAsync(CreateStaffUser(), cancellationToken: TestContext.CancellationToken);

            AssertPathContainsConfiguredOrganization(handler, "/protected/v1/1033/100/73/authenticator/staff");
        }

        [TestMethod]
        public async Task PatronRegistrationCreateAsync_UsesConfiguredOrganizationIdInRoute()
        {
            var handler = new CapturingHttpMessageHandler(CreatePapiResponseJson());
            var client = CreateOrganizationIdRouteClient(handler);

            await client.PatronRegistrationCreateAsync(new PatronRegistrationParams
            {
                LogonBranchID = 1,
                LogonUserID = 2,
                LogonWorkstationID = 3,
                PatronBranchID = 4,
                NameFirst = "Test",
                NameLast = "Patron"
            }, cancellationToken: TestContext.CancellationToken);

            AssertPathContainsConfiguredOrganization(handler, "/public/v1/1033/100/73/patron");
        }

        [TestMethod]
        public async Task PatronRegistrationCreateV2Async_UsesConfiguredOrganizationIdInRoute()
        {
            var handler = new CapturingHttpMessageHandler(CreatePapiResponseJson());
            var client = CreateOrganizationIdRouteClient(handler);

            await client.PatronRegistrationCreateV2Async(new PatronRegistrationData
            {
                LogonBranchID = 1,
                LogonUserID = 2,
                LogonWorkstationID = 3,
                PatronBranchID = 4,
                NameFirst = "Test",
                NameLast = "Patron"
            }, cancellationToken: TestContext.CancellationToken);

            AssertPathContainsConfiguredOrganization(handler, "/public/v2/1033/100/73/patron");
        }
    }
}
