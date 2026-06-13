using Clc.Polaris.Api.Configuration;
using Clc.Polaris.Api.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Clc.Polaris.Api.Tests
{
    [TestClass]
    [UnitTest]
    public class OrganizationIdRouteTests
    {
        private const int TestOrganizationId = 73;
        private const int TestUserId = 11;
        private const int TestWorkstationId = 22;
        private const string TestProtectedAccessToken = "protected-token";
        private const string TestProtectedAccessSecret = "protected-secret";
        private const string TestBarcode = "PAT123456";

        public TestContext TestContext { get; set; } = null!;

        [TestMethod]
        public async Task AuthenticatePatronAsync_UsesConfiguredOrganizationIdInRoute()
        {
            var handler = new CapturingHttpMessageHandler(CreateJson(new
            {
                PAPIErrorCode = 0,
                AccessToken = "patron-token",
                AccessSecret = "patron-secret",
                PatronID = 123
            }));
            var client = CreateClient(handler);

            await client.AuthenticatePatronAsync(TestBarcode, "1234", cancellationToken: TestContext.CancellationToken);

            AssertPathContainsConfiguredOrganization(handler, "/public/v1/1033/100/73/authenticator/patron");
        }

        [TestMethod]
        public async Task CreatePatronBlocksAsync_UsesConfiguredOrganizationIdInRoute()
        {
            var handler = new CapturingHttpMessageHandler(CreatePapiResponseJson());
            var client = CreateClient(handler);

            await client.CreatePatronBlocksAsync(
                TestBarcode,
                (BlockType)1,
                "test block",
                cancellationToken: TestContext.CancellationToken);

            AssertPathContainsConfiguredOrganization(
                handler,
                $"/protected/v1/1033/100/73/{TestProtectedAccessToken}/patron/{TestBarcode}/blocks");

            AssertQueryContains(handler, "userid=11");
            AssertQueryContains(handler, "wsid=22");
        }

        [TestMethod]
        public async Task HoldRequestCancelAsync_UsesConfiguredOrganizationIdInRoute()
        {
            var handler = new CapturingHttpMessageHandler(CreatePapiResponseJson());
            var client = CreateClient(handler);

            await client.HoldRequestCancelAsync(
                TestBarcode,
                requestId: 12345,
                password: "1234",
                cancellationToken: TestContext.CancellationToken);

            AssertPathContainsConfiguredOrganization(
                handler,
                $"/public/v1/1033/100/73/patron/{TestBarcode}/holdrequests/12345/cancelled");

            AssertQueryContains(handler, "userid=11");
            AssertQueryContains(handler, "wsid=22");
        }

        [TestMethod]
        public async Task ItemUpdateBarcodeAsync_WithItemRecordId_UsesConfiguredOrganizationIdInRoute()
        {
            var handler = new CapturingHttpMessageHandler(CreatePapiResponseJson());
            var client = CreateClient(handler);

            await client.ItemUpdateBarcodeAsync(
                newBarcode: "NEW123456",
                itemRecordId: 987654,
                cancellationToken: TestContext.CancellationToken);

            AssertPathContainsConfiguredOrganization(
                handler,
                $"/protected/v1/1033/100/73/{TestProtectedAccessToken}/cataloging/items/987654/barcode");

            AssertQueryContains(handler, "wsid=22");
            AssertRequestBodyContains(handler, "\"ItemBarcode\":\"NEW123456\"");
            AssertRequestBodyContains(handler, "\"TransactionBranchId\":73");
        }

        [TestMethod]
        public async Task ItemUpdateBarcodeAsync_WithOldBarcode_UsesConfiguredOrganizationIdInRouteAndIsBarcodeQuery()
        {
            var handler = new CapturingHttpMessageHandler(CreatePapiResponseJson());
            var client = CreateClient(handler);

            await client.ItemUpdateBarcodeAsync(
                newBarcode: "NEW123456",
                oldBarcode: "OLD123456",
                cancellationToken: TestContext.CancellationToken);

            AssertPathContainsConfiguredOrganization(
                handler,
                $"/protected/v1/1033/100/73/{TestProtectedAccessToken}/cataloging/items/OLD123456/barcode");

            AssertQueryContains(handler, "wsid=22");
            AssertQueryContains(handler, "isBarcode=1");
            AssertRequestBodyContains(handler, "\"ItemBarcode\":\"NEW123456\"");
            AssertRequestBodyContains(handler, "\"TransactionBranchId\":73");
        }

        [TestMethod]
        public async Task ItemUpdateBarcodeAsync_WithExplicitTransactionBranchId_UsesConfiguredOrganizationIdInRouteAndTransactionBranchInBody()
        {
            var handler = new CapturingHttpMessageHandler(CreatePapiResponseJson());
            var client = CreateClient(handler);

            await client.ItemUpdateBarcodeAsync(
                newBarcode: "NEW123456",
                itemRecordId: 987654,
                transactionBranchId: 55,
                cancellationToken: TestContext.CancellationToken);

            AssertPathContainsConfiguredOrganization(
                handler,
                $"/protected/v1/1033/100/73/{TestProtectedAccessToken}/cataloging/items/987654/barcode");

            AssertRequestBodyContains(handler, "\"ItemBarcode\":\"NEW123456\"");
            AssertRequestBodyContains(handler, "\"TransactionBranchId\":55");
        }

        [TestMethod]
        public async Task PatronAccountCreateCreditAsync_UsesConfiguredOrganizationIdInRoute()
        {
            var handler = new CapturingHttpMessageHandler(CreatePapiResponseJson());
            var client = CreateClient(handler);

            await client.PatronAccountCreateCreditAsync(
                TestBarcode,
                txnAmount: 1.25,
                paymentMethod: PaymentMethod.Cash,
                cancellationToken: TestContext.CancellationToken);

            AssertPathContainsConfiguredOrganization(
                handler,
                $"/protected/v1/1033/100/73/{TestProtectedAccessToken}/patron/{TestBarcode}/account/createcredit");

            AssertQueryContains(handler, "userid=11");
            AssertQueryContains(handler, "wsid=22");
        }

        [TestMethod]
        public async Task PatronAccountDepositCreditAsync_UsesConfiguredOrganizationIdInRoute()
        {
            var handler = new CapturingHttpMessageHandler(CreatePapiResponseJson());
            var client = CreateClient(handler);

            await client.PatronAccountDepositCreditAsync(
                TestBarcode,
                txnAmount: 1.25,
                cancellationToken: TestContext.CancellationToken);

            AssertPathContainsConfiguredOrganization(
                handler,
                $"/protected/v1/1033/100/73/{TestProtectedAccessToken}/patron/{TestBarcode}/account/lumpsumdepositcredit");

            AssertQueryContains(handler, "userid=11");
            AssertQueryContains(handler, "wsid=22");
        }

        [TestMethod]
        public async Task PatronAccountPayAllAsync_UsesConfiguredOrganizationIdInRoute()
        {
            var handler = new CapturingHttpMessageHandler(CreatePapiResponseJson());
            var client = CreateClient(handler);

            await client.PatronAccountPayAllAsync(
                TestBarcode,
                txnAmount: 1.25,
                paymentMethod: PaymentMethod.Cash,
                workstationId: null,
                userId: null,
                cancellationToken: TestContext.CancellationToken);

            AssertPathContainsConfiguredOrganization(
                handler,
                $"/protected/v1/1033/100/73/{TestProtectedAccessToken}/patron/{TestBarcode}/account/lumpsumpayment");

            AssertQueryContains(handler, "userid=11");
            AssertQueryContains(handler, "wsid=22");
        }

        [TestMethod]
        public async Task PatronAccountPayAsync_UsesConfiguredOrganizationIdInRoute()
        {
            var handler = new CapturingHttpMessageHandler(CreatePapiResponseJson());
            var client = CreateClient(handler);

            await client.PatronAccountPayAsync(
                TestBarcode,
                txnId: 12345,
                txnAmount: 1.25,
                paymentMethod: PaymentMethod.Cash,
                cancellationToken: TestContext.CancellationToken);

            AssertPathContainsConfiguredOrganization(
                handler,
                $"/protected/v1/1033/100/73/{TestProtectedAccessToken}/patron/{TestBarcode}/account/12345/pay");

            AssertQueryContains(handler, "userid=11");
            AssertQueryContains(handler, "wsid=22");
        }

        [TestMethod]
        public async Task PatronAccountRefundCreditAsync_UsesConfiguredOrganizationIdInRoute()
        {
            var handler = new CapturingHttpMessageHandler(CreatePapiResponseJson());
            var client = CreateClient(handler);

            await client.PatronAccountRefundCreditAsync(
                TestBarcode,
                txnAmount: 1.25,
                cancellationToken: TestContext.CancellationToken);

            AssertPathContainsConfiguredOrganization(
                handler,
                $"/protected/v1/1033/100/73/{TestProtectedAccessToken}/patron/{TestBarcode}/account/lumpsumrefundcredit");

            AssertQueryContains(handler, "userid=11");
            AssertQueryContains(handler, "wsid=22");
        }

        [TestMethod]
        public async Task PatronAccountVoidAsync_UsesConfiguredOrganizationIdInRoute()
        {
            var handler = new CapturingHttpMessageHandler(CreatePapiResponseJson());
            var client = CreateClient(handler);

            await client.PatronAccountVoidAsync(
                TestBarcode,
                paymentTxnId: 12345,
                cancellationToken: TestContext.CancellationToken);

            AssertPathContainsConfiguredOrganization(
                handler,
                $"/protected/v1/1033/100/73/{TestProtectedAccessToken}/patron/{TestBarcode}/account/12345/void/payment");

            AssertQueryContains(handler, "userid=11");
            AssertQueryContains(handler, "wsid=22");
        }

        [TestMethod]
        public async Task PatronCirculateBlocksGetAsync_UsesConfiguredOrganizationIdInRoute()
        {
            var handler = new CapturingHttpMessageHandler(CreatePapiResponseJson());
            var client = CreateClient(handler);

            await client.PatronCirculateBlocksGetAsync(
                TestBarcode,
                password: "1234",
                cancellationToken: TestContext.CancellationToken);

            AssertPathContainsConfiguredOrganization(
                handler,
                $"/public/v1/1033/100/73/patron/{TestBarcode}/circulationblocks");
        }

        [TestMethod]
        public async Task PatronItemsOutGetAsync_UsesConfiguredOrganizationIdInRoute()
        {
            var handler = new CapturingHttpMessageHandler(CreatePapiResponseJson());
            var client = CreateClient(handler);

            await client.PatronItemsOutGetAsync(
                TestBarcode,
                PatronItemsOutGetStatus.All,
                password: "1234",
                cancellationToken: TestContext.CancellationToken);

            AssertPathContainsConfiguredOrganization(
                handler,
                $"/public/v1/1033/100/73/patron/{TestBarcode}/itemsout/All");
        }

        [TestMethod]
        public async Task PatronValidateAsync_UsesConfiguredOrganizationIdInRoute()
        {
            var handler = new CapturingHttpMessageHandler(CreatePapiResponseJson());
            var client = CreateClient(handler);

            await client.PatronValidateAsync(
                TestBarcode,
                password: "1234",
                cancellationToken: TestContext.CancellationToken);

            AssertPathContainsConfiguredOrganization(
                handler,
                $"/public/v1/1033/100/73/patron/{TestBarcode}");
        }

        [TestMethod]
        public async Task RecordSetContentPutAsync_UsesConfiguredOrganizationIdInRoute()
        {
            var handler = new CapturingHttpMessageHandler(CreatePapiResponseJson());
            var client = CreateClient(handler);

            await client.RecordSetContentPutAsync(
                recordSetId: 12345,
                records: new[] { 111, 222 },
                action: (RecordSetContentPutActions)0,
                cancellationToken: TestContext.CancellationToken);

            AssertPathContainsConfiguredOrganization(
                handler,
                $"/protected/v1/1033/100/73/{TestProtectedAccessToken}/recordsets/12345");

            AssertQueryContains(handler, "userid=11");
            AssertQueryContains(handler, "wsid=22");
        }

        [TestMethod]
        public async Task RecordSetRecordsGetAsync_UsesConfiguredOrganizationIdInRoute()
        {
            var handler = new CapturingHttpMessageHandler(CreatePapiResponseJson());
            var client = CreateClient(handler);

            await client.RecordSetRecordsGetAsync(
                recordSetId: 12345,
                userId: TestUserId,
                workstationId: TestWorkstationId,
                startIndex: 0,
                numRecords: 1000,
                cancellationToken: TestContext.CancellationToken);

            AssertPathContainsConfiguredOrganization(
                handler,
                $"/protected/v1/1033/100/73/{TestProtectedAccessToken}/recordsets/12345/records");

            AssertQueryContains(handler, "startIndex=0");
            AssertQueryContains(handler, "numRecords=1000");
            AssertQueryContains(handler, "userid=11");
            AssertQueryContains(handler, "wsid=22");
        }

        [TestMethod]
        public async Task UpdatePatronNotesDataAsync_UsesConfiguredOrganizationIdInRoute()
        {
            var handler = new CapturingHttpMessageHandler(CreatePapiResponseJson());
            var client = CreateClient(handler);

            await client.UpdatePatronNotesDataAsync(
                TestBarcode,
                nonBlockingNote: "test note",
                cancellationToken: TestContext.CancellationToken);

            AssertPathContainsConfiguredOrganization(
                handler,
                $"/protected/v1/1033/100/73/{TestProtectedAccessToken}/patron/{TestBarcode}/notes");

            AssertQueryContains(handler, "wsid=22");
        }

        [TestMethod]
        public async Task UpdatePickupBranchIDAsync_UsesConfiguredOrganizationIdInRoute()
        {
            var handler = new CapturingHttpMessageHandler(CreatePapiResponseJson());
            var client = CreateClient(handler);

            await client.UpdatePickupBranchIDAsync(
                TestBarcode,
                requestId: 12345,
                pickupBranchId: 44,
                password: "1234",
                cancellationToken: TestContext.CancellationToken);

            AssertPathContainsConfiguredOrganization(
                handler,
                $"/public/v1/1033/100/73/patron/{TestBarcode}/holdrequests/12345/pickupbranch");

            AssertQueryContains(handler, "userid=11");
            AssertQueryContains(handler, "wsid=22");
            AssertQueryContains(handler, "pickupbranchid=44");
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
                var client = CreateClient(handler);

                await testCase.Execute(client);

                var actualPath = GetLastRequest(handler).RequestUri!.AbsolutePath;

                if (!actualPath.Contains($"/100/{TestOrganizationId}/", StringComparison.Ordinal))
                {
                    Assert.Fail($"{testCase.Name} did not use configured OrganizationId. Actual path: {actualPath}");
                }

                if (actualPath.Contains("/100/1/", StringComparison.Ordinal))
                {
                    Assert.Fail($"{testCase.Name} still used hardcoded organization 1. Actual path: {actualPath}");
                }
            }
        }

        private static PapiClient CreateClient(CapturingHttpMessageHandler handler)
        {
            var settings = new TestPapiSettings
            {
                OrganizationId = TestOrganizationId,
                UserId = TestUserId,
                WorkstationId = TestWorkstationId
            };

            var client = new PapiClient(new HttpClient(handler), settings)
            {
                AllowStaffOverrideRequests = false,
                UseProtectedTokenCache = false
            };

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
            var actualPath = GetLastRequest(handler).RequestUri!.AbsolutePath;

            Assert.Contains(expectedPath, actualPath);
            Assert.Contains($"/100/{TestOrganizationId}/", actualPath);

            if (actualPath.Contains("/100/1/", StringComparison.Ordinal))
            {
                Assert.Fail($"Request path still used hardcoded organization 1. Actual path: {actualPath}");
            }
        }

        private static void AssertQueryContains(CapturingHttpMessageHandler handler, string expectedQueryPart)
        {
            var actualQuery = GetLastRequest(handler).RequestUri!.Query;
            Assert.Contains(expectedQueryPart, actualQuery);
        }

        private static void AssertRequestBodyContains(CapturingHttpMessageHandler handler, string expectedContent)
        {
            Assert.IsNotNull(handler.LastRequestContent);
            Assert.Contains(expectedContent, handler.LastRequestContent);
        }

        private static HttpRequestMessage GetLastRequest(CapturingHttpMessageHandler handler)
        {
            Assert.IsNotNull(handler.LastRequest);
            return handler.LastRequest!;
        }

        private static string CreatePapiResponseJson()
        {
            return "{\"PAPIErrorCode\":0}";
        }

        private static string CreateJson(object value)
        {
            return JsonSerializer.Serialize(value);
        }

        private sealed class TestPapiSettings : IPapiSettings
        {
            public string AccessId { get; set; } = "access-id";
            public string AccessKey { get; set; } = "access-key";
            public string Hostname { get; set; } = "https://example.test";
            public int UserId { get; set; } = 1;
            public int WorkstationId { get; set; } = 1;
            public int OrganizationId { get; set; } = 1;
            public PolarisUser? PolarisOverrideAccount { get; set; }
        }

        private sealed class CapturingHttpMessageHandler : HttpMessageHandler
        {
            private readonly string _responseJson;

            public HttpRequestMessage? LastRequest { get; private set; }
            public string? LastRequestContent { get; private set; }

            public CapturingHttpMessageHandler(string responseJson)
            {
                _responseJson = responseJson;
            }

            protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            {
                LastRequest = request;
                LastRequestContent = request.Content == null
                    ? null
                    : await request.Content.ReadAsStringAsync(cancellationToken);

                return new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(_responseJson, Encoding.UTF8, "application/json")
                };
            }
        }
    }
}
