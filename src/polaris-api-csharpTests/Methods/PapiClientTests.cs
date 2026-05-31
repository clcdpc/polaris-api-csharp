using Microsoft.VisualStudio.TestTools.UnitTesting;
using Clc.Polaris.Api;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Clc.Polaris.Api.Configuration;
using System.Security.Cryptography.X509Certificates;
using Clc.Polaris.Api.Models;
using System.Net;

#pragma warning disable CS8602 // Dereference of a possibly null reference.

namespace Clc.Polaris.Api.Tests
{

    [TestClass()]
    [TestCategory("Integration")]
    public class PapiClientTests
    {
        private const string MissingIntegrationConfigurationMessage = "Integration test configuration is missing. Provide appsettings.Test.json or environment variables to run integration tests.";

        TestSettings Settings = null!;

        protected static IConfiguration InitConfiguration()
        {
            var config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.Test.json", optional: true)
                .AddEnvironmentVariables()
                .Build();

            return config;
        }

        IPapiClient papi = null!;
        int bibId = 478907;

        [TestInitialize]
        public void TestInitialize()
        {
            InitializeIntegrationTest();
        }

        private void InitializeIntegrationTest()
        {
            var config = InitConfiguration();

            var papiSettings = config.GetSection(PapiSettings.SECTION_NAME).Get<PapiSettings>();
            var testSettings = config.Get<TestSettings>();

            if (!HasRequiredPapiSettings(papiSettings) || !HasRequiredTestSettings(testSettings))
            {
                Assert.Inconclusive(MissingIntegrationConfigurationMessage);
            }

            papi = new PapiClient(papiSettings!);
            Settings = testSettings!;
        }

        private static bool HasRequiredPapiSettings(PapiSettings? settings)
        {
            return settings != null
                && !string.IsNullOrWhiteSpace(settings.AccessId)
                && !string.IsNullOrWhiteSpace(settings.AccessKey)
                && !string.IsNullOrWhiteSpace(settings.Hostname);
        }

        private static bool HasRequiredTestSettings(TestSettings? settings)
        {
            return settings != null
                && settings.PatronId > 0
                && !string.IsNullOrWhiteSpace(settings.PatronBarcode)
                && !string.IsNullOrWhiteSpace(settings.PatronPin)
                && !string.IsNullOrWhiteSpace(settings.FreeTextBlock)
                && !string.IsNullOrWhiteSpace(settings.PatronListName)
                && !string.IsNullOrWhiteSpace(settings.OrgEmail);
        }

        [TestMethod()]
        public async Task ApiKeyValidateTest()
        {
            var response = await papi.ApiKeyValidateAsync();
            Assert.AreEqual(response.Data.PAPIErrorCode, 0);
        }

        [TestMethod()]
        public async Task ApiVersionGetTest()
        {
            var response = await papi.ApiVersionGetAsync();
            Assert.AreEqual(response.Data.PAPIErrorCode, 0);
            Assert.IsFalse(string.IsNullOrWhiteSpace(response.Data.ToString()));
        }

        [TestMethod()]
        public async Task AuthenticateStaffUserTest()
        {
            var staffOverrideAccount = papi.StaffOverrideAccount;
            if (staffOverrideAccount == null)
            {
                Assert.Inconclusive(MissingIntegrationConfigurationMessage);
            }

            var response = await papi.AuthenticateStaffUserAsync(staffOverrideAccount!);
            Assert.AreEqual(response.Data.PAPIErrorCode, 0);
            Assert.IsFalse(string.IsNullOrWhiteSpace(response.Data.AccessSecret));
            Assert.IsFalse(string.IsNullOrWhiteSpace(response.Data.AccessToken));
        }

        [TestMethod()]
        public async Task BibGetTest()
        {
            var response = await papi.BibGetAsync(478907);
            Assert.AreEqual(response.Data.PAPIErrorCode, 0);
            Assert.IsFalse(string.IsNullOrWhiteSpace(response.Data.Title));
            Assert.IsTrue(response.Response.RequestMessage.RequestUri.ToString().Contains("100/1/bib"));

        }

        [TestMethod()]
        public async Task BibGetTest_PassBranchId()
        {
            var response = await papi.BibGetAsync(bibId, 7);
            Assert.AreEqual(response.Data.PAPIErrorCode, 0);
            Assert.IsFalse(string.IsNullOrWhiteSpace(response.Data.Title));
            Assert.IsTrue(response.Response.RequestMessage.RequestUri.ToString().Contains("100/7/bib"));
        }

        [TestMethod()]
        public async Task BibSearchTest()
        {
            var response = await papi.BibSearchAsync(new BibSearchOptions { Term = "dogs", PageSize = 10 });
            Assert.IsTrue(response.Data.PAPIErrorCode == 10);
            Assert.IsTrue(response.Data.WordList == "dogs ");
            Assert.IsTrue(response.Data.TotalRecordsFound > 10000);
        }

        [TestMethod()]
        public async Task CollectionsGetTest()
        {
            var response = await papi.CollectionsGetAsync();
            Assert.IsTrue(response.Data.PAPIErrorCode > 300);
            Assert.IsTrue(response.Data.CollectionsRows.Count > 300);
            Assert.AreEqual(response.Data.PAPIErrorCode, response.Data.CollectionsRows.Count);
        }

        [TestMethod()]
        public async Task CreatePatronBlocksTest_FreeTextBlock()
        {
            var response = await papi.CreatePatronBlocksAsync(Settings.PatronBarcode, BlockType.FreeText, Settings.FreeTextBlock);
            Assert.IsTrue(new[] { 0, -3507 }.Contains(response.Data.PAPIErrorCode));
        }

        [TestMethod()]
        public async Task CreatePatronBlocksTest_SystemBlock()
        {
            var response = await papi.CreatePatronBlocksAsync(Settings.PatronBarcode, BlockType.System, "128");
            Assert.IsTrue(new[] { 0, -3507 }.Contains(response.Data.PAPIErrorCode));
        }

        [TestMethod()]
        public async Task CreatePatronBlocksTest_LibraryAssignedBlock()
        {
            var response = await papi.CreatePatronBlocksAsync(Settings.PatronBarcode, BlockType.LibraryAssigned, "1");
            Assert.IsTrue(new[] { 0, -3507 }.Contains(response.Data.PAPIErrorCode));
        }

        [TestMethod()]
        public async Task DatesClosedGetTest()
        {
            var response = await papi.DatesClosedGetAsync(7);
            Assert.IsTrue(response.Data.DatesClosedRows.Any());
        }

        [TestMethod()]
        public void HeadingsSearchTest()
        {
            Assert.ThrowsException<NotImplementedException>(() => papi.HeadingsSearchAsync(bibId));
        }

        [TestMethod()]
        public async Task HoldingsGetTest()
        {
            var response = await papi.HoldingsGetAsync(bibId);
            Assert.IsTrue(response.Data.BibHoldingsGetRows.Any());
        }

        [TestMethod()]
        public async Task HoldRequestCancelTest()
        {
            var response = await papi.HoldRequestCancelAsync(Settings.PatronBarcode, 1234, Settings.PatronPin);
            Assert.IsTrue(response.Data.PAPIErrorCode == -4201);
        }

        [TestMethod()]
        public async Task HoldRequestCreateTest()
        {
            var response = await papi.HoldRequestCreateAsync(new HoldRequestCreateParams(Settings.PatronId, 1234, 7, 7));
            Assert.IsTrue(response.Data.PAPIErrorCode == -4006);
        }

        [TestMethod()]
        public async Task HoldRequestCreateTest2()
        {
            var response = await ((PapiClient)papi).HoldRequestCreateAsync(Settings.PatronId, 1234, 7);
            Assert.IsTrue(response.Data.PAPIErrorCode == -4006);
        }

        [TestMethod()]
        public async Task HoldRequestGetListTest()
        {
            var response = await papi.HoldRequestGetListAsync(7);
            Assert.IsTrue(response.Data.PAPIErrorCode == 0);
        }

        [TestMethod()]
        public async Task HoldRequestReactivateTest()
        {
            var response = await papi.HoldRequestReactivateAsync(Settings.PatronBarcode, Settings.PatronPin, 1234, DateTime.Now);
            Assert.IsTrue(response.Data.PAPIErrorCode == -4201);
        }

        [TestMethod()]
        public async Task HoldRequestReplyTest()
        {
            var hold = new HoldRequestCreateResult { RequestGuid = new Guid() };
            var response = await papi.HoldRequestReplyAsync(hold, 7, HoldRequestReplyAnswer.Yes, HoldRequestReplyState.AcceptEvenWithExistingHolds);
            Assert.IsTrue(response.Data.PAPIErrorCode == -4101);
        }

        [TestMethod()]
        public async Task HoldRequestSuspendTest()
        {
            var response = await papi.HoldRequestSuspendAsync(Settings.PatronBarcode, 1234, DateTime.Now, Settings.PatronPin);
            Assert.IsTrue(response.Data.PAPIErrorCode == -4201);
        }

        [TestMethod()]
        public async Task ItemRenewTest()
        {
            var response = await papi.ItemRenewAsync(Settings.PatronBarcode, 1234, Settings.PatronPin);
            Assert.IsTrue(response.Data.PAPIErrorCode == -6001);
        }

        [TestMethod()]
        public async Task ItemStatusesGetAsyncTest()
        {
            var response = await papi.ItemStatusesGetAsync(7);
            Assert.IsTrue(response.Data.ItemStatusesRows.Count() == response.Data.PAPIErrorCode);
        }

        //[TestMethod()]
        //public async Task ItemUpdateBarcodeTest()
        //{
        //    var response = await papi.ItemUpdateBarcodeAsync("1234", 1234);
        //    Assert.IsTrue(response.Data.PAPIErrorCode == -2000);
        //}

        [TestMethod()]
        public async Task LimitFiltersGetTest()
        {
            var response = (await papi.LimitFiltersGetAsync()).Data;
            Assert.IsTrue(response.LimitFiltersRows.Count() == response.PAPIErrorCode);
        }

        [TestMethod()]
        public async Task MARCTypeOfMaterialsGetAsyncTest()
        {
            var response = (await papi.MARCTypeOfMaterialsGetAsync()).Data;
            Assert.IsTrue(response.MARCTypeOfMaterialsRows.Count() == response.PAPIErrorCode);
        }

        [TestMethod()]
        public async Task NotificationUpdateTest()
        {
            var response = (await papi.NotificationUpdateAsync(new NotificationUpdateParams { PatronId = Settings.PatronId, DeliveryString = "test@test.test", ReportingOrgID = 7, NotificationDeliveryDate = DateTime.Now, DeliveryOptionId = 2, Details = "test", NotificationStatusId = NotificationStatus.EmailCompleted, NotificationTypeId = 1 })).Data;
            Assert.IsTrue(response.PAPIErrorCode == -1);
        }

        [TestMethod()]
        public async Task OrganizationsGetTest()
        {
            var response = await papi.OrganizationsGetAsync();
            Assert.IsTrue(response.Data.OrganizationsGetRows.Count() == response.Data.PAPIErrorCode);
        }

        [TestMethod()]
        public async Task Patron_GetBarcodeFromIdTest()
        {
            var response = await papi.Patron_GetBarcodeFromIdAsync(Settings.PatronId);
            Assert.IsTrue(response.Data.Barcode == Settings.PatronBarcode);
        }

        [TestMethod()]
        public async Task PatronAccountCreateCreditTest()
        {
            var response = await papi.PatronAccountCreateCreditAsync(Settings.PatronBarcode, .01, PaymentMethod.Cash);
            Assert.IsTrue(response.Data.PAPIErrorCode == 0);
        }

        [TestMethod()]
        public async Task TestTitleListCreate_Get_Delete()
        {
            var createResponse = await papi.PatronAccountCreateTitleListAsync(Settings.PatronBarcode, Settings.PatronListName, Settings.PatronPin);
            Assert.IsTrue(createResponse.Data.PAPIErrorCode == 0 || createResponse.Data.PAPIErrorCode == -1);

            var getResponse = await papi.PatronAccountGetTitleListsAsync(Settings.PatronBarcode, Settings.PatronPin);
            var list = getResponse.Data.PatronAccountTitleListsRows.Single(l => l.RecordStoreName == Settings.PatronListName);
            var deleteResponse = await papi.PatronAccountDeleteTitleListAsync(Settings.PatronBarcode, list.RecordStoreId, Settings.PatronPin);
            Assert.IsTrue(deleteResponse.Data.PAPIErrorCode == 0);
        }

        [TestMethod()]
        public async Task PatronAccountDepositCreditTest()
        {
            var response = await papi.PatronAccountDepositCreditAsync(Settings.PatronBarcode, .01, note: "integration testing");
            Assert.IsTrue(response.Data.PAPIErrorCode == 0);
        }

        [TestMethod()]
        public async Task PatronAccountGetTest()
        {
            var response = await papi.PatronAccountGetAsync(Settings.PatronBarcode, Settings.PatronPin);
            Assert.IsTrue(response.Data.PAPIErrorCode == 0);
            Assert.IsTrue(response.Data.PatronAccountGetRows.Any());
        }

        [TestMethod()]
        public async Task PatronAccountPayTest()
        {
            var response = (await papi.PatronAccountPayAsync(Settings.PatronBarcode, 1234, .01, PaymentMethod.Cash, note: "integration testing")).Data;
            Assert.IsTrue(response.PAPIErrorCode == -3600);
        }

        [TestMethod()]
        public async Task PatronAccountPayAllTest()
        {
            var response = await papi.PatronAccountPayAllAsync(Settings.PatronBarcode, 999999.99, PaymentMethod.Cash, note: "integration testing");
            Assert.IsTrue(response.Data.PAPIErrorCode == -3610);
        }

        [TestMethod()]
        public async Task PatronAccountRefundCreditTest()
        {
            var response = await papi.PatronAccountRefundCreditAsync(Settings.PatronBarcode, 999999.99, note: "integration testing");
            Assert.IsTrue(response.Data.PAPIErrorCode == -3606);
        }

        [TestMethod()]
        public async Task PatronAccountVoidTest()
        {
            var response = await papi.PatronAccountVoidAsync(Settings.PatronBarcode, 1234, note: "integration testing");
            Assert.IsTrue(response.Data.PAPIErrorCode == -3606);
        }

        [TestMethod()]
        public async Task PatronBasicDataGetTest()
        {
            var response = await papi.PatronBasicDataGetAsync(Settings.PatronBarcode, Settings.PatronPin, true);
            Assert.IsTrue(response.Data.PAPIErrorCode == 0);
            Assert.IsTrue(response.Data.PatronBasicData.PatronID == Settings.PatronId);
            Assert.IsTrue(response.Data.PatronBasicData.PatronAddresses.Any());
        }

        [TestMethod()]
        public async Task PatronCirculateBlocksGetTest()
        {
            var response = await papi.PatronCirculateBlocksGetAsync(Settings.PatronBarcode, Settings.PatronPin);
            Assert.IsTrue(response.Data.PAPIErrorCode == 0);
        }

        [TestMethod()]
        public async Task PatronCodesGetTest()
        {
            var response = await papi.PatronCodesGetAsync();
            Assert.IsTrue(response.Data.PAPIErrorCode == 0);
            Assert.IsTrue(response.Data.PatronCodesRows.Any());
        }

        [TestMethod()]
        public async Task PatronHoldRequestsGetTest()
        {
            var response = await papi.PatronHoldRequestsGetAsync(Settings.PatronBarcode, PatronHoldStatus.all, Settings.PatronPin);
            Assert.IsTrue(response.Data.PAPIErrorCode == 0);
            Assert.IsTrue(response.Data.PatronHoldRequestsGetRows.Any());
        }

        [TestMethod()]
        public async Task PatronILLRequestsGetTest()
        {
            var response = await papi.PatronILLRequestsGetAsync(Settings.PatronBarcode, password: Settings.PatronPin);
            Assert.IsTrue(response.Data.PAPIErrorCode == 0);
            //Assert.IsTrue(response.Data.PatronILLRequestsGetRows.Any());
        }

        [TestMethod()]
        public async Task PatronItemsOutGetTest()
        {
            var response = await papi.PatronItemsOutGetAsync(Settings.PatronBarcode, password: Settings.PatronPin);
            Assert.IsTrue(response.Data.PAPIErrorCode == 0);
            //Assert.IsTrue(response.Data.PatronItemsOutGetRows.Any());
        }

        [TestMethod()]
        public async Task PatronMessageDeleteTest()
        {
            var response = await papi.PatronMessageDeleteAsync(Settings.PatronBarcode, PatronMessageType.freetext, 1234, Settings.PatronPin);
            Assert.IsTrue(response.Data.PAPIErrorCode == -1);
        }

        [TestMethod()]
        public async Task PatronMessagesGetTest()
        {
            var response = await papi.PatronMessagesGetAsync(Settings.PatronBarcode, password: Settings.PatronPin);
            Assert.IsTrue(response.Data.PAPIErrorCode == 0);
        }

        [TestMethod()]
        public async Task PatronMessageUpdateStatusTest()
        {
            var response = await papi.PatronMessageUpdateStatusAsync(Settings.PatronBarcode, PatronMessageType.freetext, 1234, Settings.PatronPin);
            Assert.IsTrue(response.Data.PAPIErrorCode == -1);
        }

        [TestMethod()]
        public async Task PatronPreferencesGetTest()
        {
            var response = await papi.PatronPreferencesGetAsync(Settings.PatronBarcode, Settings.PatronPin);
            Assert.IsTrue(response.Data.PatronPreferences.PatronID == Settings.PatronId);
        }

        [TestMethod()]
        public async Task PatronReadingHistoryClearTest()
        {
            var response = await papi.PatronReadingHistoryClearAsync(Settings.PatronBarcode, new[] { 1234 });
            Assert.IsTrue(response.Data.PAPIErrorCode == -10);
        }

        [TestMethod()]
        public async Task PatronReadingHistoryGetTest()
        {
            var response = await papi.PatronReadingHistoryGetAsync(Settings.PatronBarcode, password: Settings.PatronPin);
            Assert.IsTrue(response.Data.PAPIErrorCode == response.Data.PatronReadingHistoryGetRows.Count());
        }

        //[TestMethod()]
        //public async Task PatronRegistrationCreateTest()
        //{
        //    var response = await papi.PatronRegistrationCreateAsync(new PatronRegistrationParams());
        //    Assert.IsTrue(response.Data.PAPIErrorCode == -1);
        //}

        [TestMethod()]
        public async Task PatronRenewBlocksGetTest()
        {
            var response = await papi.PatronRenewBlocksGetAsync(Settings.PatronId);
            Assert.IsTrue(response.Data.PAPIErrorCode == 0);
        }

        [TestMethod()]
        public async Task PatronSavedSearchesGetTest()
        {
            var response = await papi.PatronSavedSearchesGetAsync(Settings.PatronBarcode, Settings.PatronPin);
            Assert.IsTrue(response.Data.PAPIErrorCode == 0);
        }

        [TestMethod()]
        public async Task PatronSearchTest()
        {
            var response = await papi.PatronSearchAsync($"PRID={Settings.PatronId}");
            Assert.IsTrue(response.Data.PAPIErrorCode == response.Data.PatronSearchRows.Count);
        }

        [TestMethod()]
        public async Task PatronTitleListAddTitleTest()
        {
            var response = await papi.PatronTitleListAddTitleAsync(Settings.PatronBarcode, 1234, 1234, Settings.PatronPin);
            Assert.IsTrue(response.Data.PAPIErrorCode == -1);
        }

        [TestMethod()]
        public async Task PatronTitleListCopyAllTitlesTest()
        {
            var response = await papi.PatronTitleListCopyAllTitlesAsync(Settings.PatronBarcode, 1234, 1234, Settings.PatronPin);
            Assert.IsTrue(response.Data.PAPIErrorCode == -1);
        }

        [TestMethod()]
        public async Task PatronTitleListCopyTitleTest()
        {
            var response = await papi.PatronTitleListCopyTitleAsync(Settings.PatronBarcode, 1234, 1234, 1234, Settings.PatronPin);
            Assert.IsTrue(response.Data.PAPIErrorCode == -1);
        }

        [TestMethod()]
        public async Task PatronTitleListDeleteAllTitlesTest()
        {
            var response = await papi.PatronTitleListDeleteAllTitlesAsync(Settings.PatronBarcode, 1234, Settings.PatronPin);
            Assert.IsTrue(response.Data.PAPIErrorCode == -1);
        }

        [TestMethod()]
        public async Task PatronTitleListDeleteTitleTest()
        {
            var response = await papi.PatronTitleListDeleteTitleAsync(Settings.PatronBarcode, 1234, 1234, Settings.PatronPin);
            Assert.IsTrue(response.Data.PAPIErrorCode == -1);
        }

        [TestMethod()]
        public async Task PatronTitleListGetTitlesTest()
        {
            var response = await papi.PatronTitleListGetTitlesAsync(Settings.PatronBarcode, 1234, password: Settings.PatronPin);
            Assert.IsTrue(response.Data.PAPIErrorCode == -1);
        }

        [TestMethod()]
        public async Task PatronTitleListMoveTitleTest()
        {
            var response = await papi.PatronTitleListMoveTitleAsync(Settings.PatronBarcode, 1234, 1234, 1234, Settings.PatronPin);
            Assert.IsTrue(response.Data.PAPIErrorCode == -1);
        }

        [TestMethod()]
        public async Task PatronUpdateTest()
        {
            var response = await papi.PatronUpdateAsync(Settings.PatronBarcode, new PatronUpdateParams(), Settings.PatronPin);
            Assert.IsTrue(response.Data.PAPIErrorCode == 0);
        }

        [TestMethod()]
        public async Task PatronUpdateUserNameTest()
        {
            var response = await papi.PatronUpdateUserNameAsync(Settings.PatronBarcode + "1234", Settings.PatronPin, Settings.PatronPin);
            Assert.IsTrue(response.Response.StatusCode == HttpStatusCode.Unauthorized);
        }

        [TestMethod()]
        public async Task PatronValidateTest()
        {
            var response = await papi.PatronValidateAsync(Settings.PatronBarcode, Settings.PatronPin);
            Assert.IsTrue(response.Data.PatronID == Settings.PatronId);
        }

        [TestMethod()]
        public async Task PickupBranchesGetTest()
        {
            var response = await papi.PickupBranchesGetAsync();
            Assert.IsTrue(response.Data.PickupBranchesRows.Any());
        }

        [TestMethod()]
        public async Task RecordSetContentAddTest()
        {
            var response = await papi.RecordSetContentAddAsync(1234, 1234);
            Assert.IsTrue(response.Data.PAPIErrorCode == -11001);
        }

        [TestMethod()]
        public async Task RecordSetContentAddTest_List()
        {
            var response = await papi.RecordSetContentAddAsync(1234, new[] { 1234 });
            Assert.IsTrue(response.Data.PAPIErrorCode == -11001);
        }

        [TestMethod()]
        public async Task RecordSetContentRemoveTest()
        {
            var response = await papi.RecordSetContentRemoveAsync(1234, 1234);
            Assert.IsTrue(response.Data.PAPIErrorCode == -11001);
        }

        [TestMethod()]
        public async Task RecordSetContentRemoveTest_List()
        {
            var response = await papi.RecordSetContentRemoveAsync(1234, new[] { 1234 });
            Assert.IsTrue(response.Data.PAPIErrorCode == -11001);
        }

        [TestMethod()]
        public async Task RecordSetRecordsGetTest()
        {
            var response = await papi.RecordSetRecordsGetAsync(1234);
            Assert.IsTrue(response.Data.PAPIErrorCode == -11001);
        }

        //[TestMethod()]
        //public async Task RemoteStorageItemsGetTest()
        //{
        //    var response = await papi.RemoteStorageItemsGetAsync(7, "asdf", "asdf", 1, 1);
        //    Assert.IsTrue(response.Data.PAPIErrorCode == -1);
        //}

        [TestMethod()]
        public async Task SA_GetValueByOrgTest()
        {
            var response = await papi.SA_GetValueByOrgAsync("ORGEMAIL");
            Assert.IsTrue(response.Data.Value == Settings.OrgEmail);
        }

        [TestMethod()]
        public async Task ShelfLocationsGetTest()
        {
            var response = await papi.ShelfLocationsGetAsync(7);
            Assert.IsTrue(response.Data.PAPIErrorCode == response.Data.ShelfLocationsRows.Count());
        }

        [TestMethod()]
        public async Task Synch_BibsByIdGetTest()
        {
            var response = await papi.Synch_BibsByIdGetAsync(bibId);
            Assert.IsTrue(response.Response.IsSuccessStatusCode);
        }
    }
}