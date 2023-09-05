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
        TestSettings Settings;

        protected static IConfiguration InitConfiguration()
        {
            var config = new ConfigurationBuilder()
               .AddJsonFile("appsettings.Test.json")
                .AddEnvironmentVariables()
                .Build();
            return config;
        }

        IPapiClient papi;
        int bibId = 478907;

        public PapiClientTests()
        {
            var config = InitConfiguration();
            papi = new PapiClient(config.GetSection(PapiSettings.SECTION_NAME).Get<PapiSettings>());
            Settings = config.Get<TestSettings>();
            var foo = "";
            
        }

        [TestMethod()]
        public void ApiKeyValidateTest()
        {
            var response = papi.ApiKeyValidate();
            Assert.AreEqual(response.Data.PAPIErrorCode, 0);
        }

        [TestMethod()]
        public void ApiVersionGetTest()
        {
            var response = papi.ApiVersionGet();
            Assert.AreEqual(response.Data.PAPIErrorCode, 0);
            Assert.IsFalse(string.IsNullOrWhiteSpace(response.Data.ToString()));
        }

        [TestMethod()]
        public void AuthenticateStaffUserTest()
        {
            var response = papi.AuthenticateStaffUser(papi.StaffOverrideAccount);
            Assert.AreEqual(response.Data.PAPIErrorCode, 0);
            Assert.IsFalse(string.IsNullOrWhiteSpace(response.Data.AccessSecret));
            Assert.IsFalse(string.IsNullOrWhiteSpace(response.Data.AccessToken));
        }

        [TestMethod()]
        public void BibGetTest()
        {
            var response = papi.BibGet(478907);
            Assert.AreEqual(response.Data.PAPIErrorCode, 0);
            Assert.IsFalse(string.IsNullOrWhiteSpace(response.Data.Title));
            Assert.IsTrue(response.Response.RequestMessage.RequestUri.ToString().Contains("100/1/bib"));

        }

        [TestMethod()]
        public void BibGetTest_PassBranchId()
        {
            var response = papi.BibGet(bibId, 7);
            Assert.AreEqual(response.Data.PAPIErrorCode, 0);
            Assert.IsFalse(string.IsNullOrWhiteSpace(response.Data.Title));
            Assert.IsTrue(response.Response.RequestMessage.RequestUri.ToString().Contains("100/7/bib"));
        }

        [TestMethod()]
        public void BibSearchTest()
        {
            var response = papi.BibSearch(new BibSearchOptions { Term = "dogs", PageSize = 10 });
            Assert.IsTrue(response.Data.PAPIErrorCode == 10);
            Assert.IsTrue(response.Data.WordList == "dogs ");
            Assert.IsTrue(response.Data.TotalRecordsFound > 10000);
        }

        [TestMethod()]
        public void CollectionsGetTest()
        {
            var response = papi.CollectionsGet();
            Assert.IsTrue(response.Data.PAPIErrorCode > 300);
            Assert.IsTrue(response.Data.CollectionsRows.Count > 300);
            Assert.AreEqual(response.Data.PAPIErrorCode, response.Data.CollectionsRows.Count);
        }

        [TestMethod()]
        public void CreatePatronBlocksTest_FreeTextBlock()
        {
            var response = papi.CreatePatronBlocks(Settings.PatronBarcode, BlockType.FreeText, Settings.FreeTextBlock);
            Assert.IsTrue(new[] { 0, -3507 }.Contains(response.Data.PAPIErrorCode));
        }

        [TestMethod()]
        public void CreatePatronBlocksTest_SystemBlock()
        {
            var response = papi.CreatePatronBlocks(Settings.PatronBarcode, BlockType.System, "128");
            Assert.IsTrue(new[] { 0, -3507 }.Contains(response.Data.PAPIErrorCode));
        }

        [TestMethod()]
        public void CreatePatronBlocksTest_LibraryAssignedBlock()
        {
            var response = papi.CreatePatronBlocks(Settings.PatronBarcode, BlockType.LibraryAssigned, "1");
            Assert.IsTrue(new[] { 0, -3507 }.Contains(response.Data.PAPIErrorCode));
        }

        [TestMethod()]
        public void DatesClosedGetTest()
        {
            var response = papi.DatesClosedGet(7);
            Assert.IsTrue(response.Data.DatesClosedRows.Any());
        }

        [TestMethod()]
        public void HeadingsSearchTest()
        {
            Assert.ThrowsException<NotImplementedException>(() => papi.HeadingsSearch(bibId));
        }

        [TestMethod()]
        public void HoldingsGetTest()
        {
            var response = papi.HoldingsGet(bibId);
            Assert.IsTrue(response.Data.BibHoldingsGetRows.Any());
        }

        [TestMethod()]
        public void HoldRequestCancelTest()
        {
            var response = papi.HoldRequestCancel(Settings.PatronBarcode, 1234, Settings.PatronPin);
            Assert.IsTrue(response.Data.PAPIErrorCode == -4201);
        }

        [TestMethod()]
        public void HoldRequestCreateTest()
        {
            var response = papi.HoldRequestCreate(new HoldRequestCreateParams(Settings.PatronId, 1234, 7, 7));
            Assert.IsTrue(response.Data.PAPIErrorCode == -4006);
        }

        [TestMethod()]
        public void HoldRequestCreateTest2()
        {
            var response = ((PapiClient)papi).HoldRequestCreate(Settings.PatronId, 1234, 7);
            Assert.IsTrue(response.Data.PAPIErrorCode == -4006);
        }

        [TestMethod()]
        public void HoldRequestGetListTest()
        {
            var response = papi.HoldRequestGetList(7);
            Assert.IsTrue(response.Data.PAPIErrorCode == 0);
        }

        [TestMethod()]
        public void HoldRequestReactivateTest()
        {
            var response = papi.HoldRequestReactivate(Settings.PatronBarcode, Settings.PatronPin, 1234, DateTime.Now);
            Assert.IsTrue(response.Data.PAPIErrorCode == -4201);
        }

        [TestMethod()]
        public void HoldRequestReplyTest()
        {
            var hold = new HoldRequestCreateResult { RequestGuid = new Guid() };
            var response = papi.HoldRequestReply(hold, 7, HoldRequestReplyAnswer.Yes, HoldRequestReplyState.AcceptEvenWithExistingHolds);
            Assert.IsTrue(response.Data.PAPIErrorCode == -4101);
        }

        [TestMethod()]
        public void HoldRequestSuspendTest()
        {
            var response = papi.HoldRequestSuspend(Settings.PatronBarcode, 1234, DateTime.Now, Settings.PatronPin);
            Assert.IsTrue(response.Data.PAPIErrorCode == -4201);
        }

        [TestMethod()]
        public void ItemRenewTest()
        {
            var response = papi.ItemRenew(Settings.PatronBarcode, 1234, Settings.PatronPin);
            Assert.IsTrue(response.Data.PAPIErrorCode == -6001);
        }

        [TestMethod()]
        public void ItemStatusesGetAsyncTest()
        {
            var response = papi.ItemStatusesGet(7);
            Assert.IsTrue(response.Data.ItemStatusesRows.Count() == response.Data.PAPIErrorCode); ;
        }

        [TestMethod()]
        public void ItemUpdateBarcodeTest()
        {
            var response = papi.ItemUpdateBarcode("1234", 1234);
            Assert.IsTrue(response.Data.PAPIErrorCode == -2000);
        }

        [TestMethod()]
        public void LimitFiltersGetTest()
        {
            var response = papi.LimitFiltersGet().Data;
            Assert.IsTrue(response.LimitFiltersRows.Count() == response.PAPIErrorCode);
        }

        [TestMethod()]
        public void MARCTypeOfMaterialsGetAsyncTest()
        {
            var response = papi.MARCTypeOfMaterialsGet().Data;
            Assert.IsTrue(response.MARCTypeOfMaterialsRows.Count() == response.PAPIErrorCode);
        }

        [TestMethod()]
        public void NotificationUpdateTest()
        {
            var response = papi.NotificationUpdate(new NotificationUpdateParams { PatronId = Settings.PatronId, DeliveryString = "test@test.test", ReportingOrgID = 7, NotificationDeliveryDate = DateTime.Now, DeliveryOptionId = 2, Details = "test", NotificationStatusId = NotificationStatus.EmailCompleted, NotificationTypeId = 1 }).Data;
            Assert.IsTrue(response.PAPIErrorCode == -1);
        }

        [TestMethod()]
        public void OrganizationsGetTest()
        {
            var response = papi.OrganizationsGet();
            Assert.IsTrue(response.Data.OrganizationsGetRows.Count() == response.Data.PAPIErrorCode);
        }

        [TestMethod()]
        public void Patron_GetBarcodeFromIdTest()
        {
            var response = papi.Patron_GetBarcodeFromId(Settings.PatronId);
            Assert.IsTrue(response.Data.Barcode == Settings.PatronBarcode);
        }

        [TestMethod()]
        public void PatronAccountCreateCreditTest()
        {
            var response = papi.PatronAccountCreateCredit(Settings.PatronBarcode, .01, PaymentMethod.Cash);
            Assert.IsTrue(response.Data.PAPIErrorCode == 0);
        }

        [TestMethod()]
        public void TestTitleListCreate_Get_Delete()
        {
            var createResponse = papi.PatronAccountCreateTitleList(Settings.PatronBarcode, Settings.PatronListName, Settings.PatronPin);
            Assert.IsTrue(createResponse.Data.PAPIErrorCode == 0 || createResponse.Data.PAPIErrorCode == -1);

            var getResponse = papi.PatronAccountGetTitleLists(Settings.PatronBarcode, Settings.PatronPin);
            var list = getResponse.Data.PatronAccountTitleListsRows.Single(l => l.RecordStoreName == Settings.PatronListName);
            var deleteResponse = papi.PatronAccountDeleteTitleList(Settings.PatronBarcode, list.RecordStoreId, Settings.PatronPin);
            Assert.IsTrue(deleteResponse.Data.PAPIErrorCode == 0);
        }

        [TestMethod()]
        public void PatronAccountDepositCreditTest()
        {
            var response = papi.PatronAccountDepositCredit(Settings.PatronBarcode, .01, note: "integration testing");
            Assert.IsTrue(response.Data.PAPIErrorCode == 0);
        }

        [TestMethod()]
        public void PatronAccountGetTest()
        {
            var response = papi.PatronAccountGet(Settings.PatronBarcode, Settings.PatronPin);
            Assert.IsTrue(response.Data.PAPIErrorCode == 0);
            Assert.IsTrue(response.Data.PatronAccountGetRows.Any());
        }

        [TestMethod()]
        public void PatronAccountPayTest()
        {
            var response = papi.PatronAccountPay(Settings.PatronBarcode, 1234, .01, PaymentMethod.Cash, note: "integration testing").Data;
            Assert.IsTrue(response.PAPIErrorCode == -3600);
        }

        [TestMethod()]
        public void PatronAccountPayAllTest()
        {
            var response = papi.PatronAccountPayAll(Settings.PatronBarcode, 999999.99, PaymentMethod.Cash, note: "integration testing");
            Assert.IsTrue(response.Data.PAPIErrorCode == -3610);
        }

        [TestMethod()]
        public void PatronAccountRefundCreditTest()
        {
            var response = papi.PatronAccountRefundCredit(Settings.PatronBarcode, 999999.99, note: "integration testing");
            Assert.IsTrue(response.Data.PAPIErrorCode == -3606);
        }

        [TestMethod()]
        public void PatronAccountVoidTest()
        {
            var response = papi.PatronAccountVoid(Settings.PatronBarcode, 1234, note: "integration testing");
            Assert.IsTrue(response.Data.PAPIErrorCode == -3606);
        }

        [TestMethod()]
        public void PatronBasicDataGetTest()
        {
            var response = papi.PatronBasicDataGet(Settings.PatronBarcode, Settings.PatronPin, true);
            Assert.IsTrue(response.Data.PAPIErrorCode == 0);
            Assert.IsTrue(response.Data.PatronBasicData.PatronID == Settings.PatronId);
            Assert.IsTrue(response.Data.PatronBasicData.PatronAddresses.Any());
        }

        [TestMethod()]
        public void PatronCirculateBlocksGetTest()
        {
            var response = papi.PatronCirculateBlocksGet(Settings.PatronBarcode, Settings.PatronPin);
            Assert.IsTrue(response.Data.PAPIErrorCode == 0);
        }

        [TestMethod()]
        public void PatronCodesGetTest()
        {
            var response = papi.PatronCodesGet();
            Assert.IsTrue(response.Data.PAPIErrorCode == 0);
            Assert.IsTrue(response.Data.PatronCodesRows.Any());
        }

        [TestMethod()]
        public void PatronHoldRequestsGetTest()
        {
            var response = papi.PatronHoldRequestsGet(Settings.PatronBarcode, PatronHoldStatus.all, Settings.PatronPin);
            Assert.IsTrue(response.Data.PAPIErrorCode == 0);
            Assert.IsTrue(response.Data.PatronHoldRequestsGetRows.Any());
        }

        [TestMethod()]
        public void PatronILLRequestsGetTest()
        {
            var response = papi.PatronILLRequestsGet(Settings.PatronBarcode, password: Settings.PatronPin);
            Assert.IsTrue(response.Data.PAPIErrorCode == 0);
            //Assert.IsTrue(response.Data.PatronILLRequestsGetRows.Any());
        }

        [TestMethod()]
        public void PatronItemsOutGetTest()
        {
            var response = papi.PatronItemsOutGet(Settings.PatronBarcode, password: Settings.PatronPin);
            Assert.IsTrue(response.Data.PAPIErrorCode == 0);
            //Assert.IsTrue(response.Data.PatronItemsOutGetRows.Any());
        }

        [TestMethod()]
        public void PatronMessageDeleteTest()
        {
            var response = papi.PatronMessageDelete(Settings.PatronBarcode, PatronMessageType.freetext, 1234, Settings.PatronPin);
            Assert.IsTrue(response.Data.PAPIErrorCode == -1);
        }

        [TestMethod()]
        public void PatronMessagesGetTest()
        {
            var response = papi.PatronMessagesGet(Settings.PatronBarcode, password: Settings.PatronPin);
            Assert.IsTrue(response.Data.PAPIErrorCode == 0);
        }

        [TestMethod()]
        public void PatronMessageUpdateStatusTest()
        {
            var response = papi.PatronMessageUpdateStatus(Settings.PatronBarcode, PatronMessageType.freetext, 1234, Settings.PatronPin);
            Assert.IsTrue(response.Data.PAPIErrorCode == -1);
        }

        [TestMethod()]
        public void PatronPreferencesGetTest()
        {
            var response = papi.PatronPreferencesGet(Settings.PatronBarcode, Settings.PatronPin);
            Assert.IsTrue(response.Data.PatronPreferences.PatronID == Settings.PatronId);
        }

        [TestMethod()]
        public void PatronReadingHistoryClearTest()
        {
            var response = papi.PatronReadingHistoryClear(Settings.PatronBarcode, 1234);
            Assert.IsTrue(response.Data.PAPIErrorCode == -10);
        }

        [TestMethod()]
        public void PatronReadingHistoryGetTest()
        {
            var response = papi.PatronReadingHistoryGet(Settings.PatronBarcode, password: Settings.PatronPin);
            Assert.IsTrue(response.Data.PAPIErrorCode == response.Data.PatronReadingHistoryGetRows.Count());
        }

        [TestMethod()]
        public void PatronRegistrationCreateTest()
        {
            var response = papi.PatronRegistrationCreate(new PatronRegistrationParams());
            Assert.IsTrue(response.Data.PAPIErrorCode == -1);
        }

        [TestMethod()]
        public void PatronRenewBlocksGetTest()
        {
            var response = papi.PatronRenewBlocksGet(Settings.PatronId);
            Assert.IsTrue(response.Data.PAPIErrorCode == 0);
        }

        [TestMethod()]
        public void PatronSavedSearchesGetTest()
        {
            var response = papi.PatronSavedSearchesGet(Settings.PatronBarcode, Settings.PatronPin);
            Assert.IsTrue(response.Data.PAPIErrorCode == 0);
        }

        [TestMethod()]
        public void PatronSearchTest()
        {
            var response = papi.PatronSearch($"PRID={Settings.PatronId}");
            Assert.IsTrue(response.Data.PAPIErrorCode == response.Data.PatronSearchRows.Count);
        }

        [TestMethod()]
        public void PatronTitleListAddTitleTest()
        {
            var response = papi.PatronTitleListAddTitle(Settings.PatronBarcode, 1234, 1234, Settings.PatronPin);
            Assert.IsTrue(response.Data.PAPIErrorCode == -1);
        }

        [TestMethod()]
        public void PatronTitleListCopyAllTitlesTest()
        {
            var response = papi.PatronTitleListCopyAllTitles(Settings.PatronBarcode, 1234, 1234, Settings.PatronPin);
            Assert.IsTrue(response.Data.PAPIErrorCode == -1);
        }

        [TestMethod()]
        public void PatronTitleListCopyTitleTest()
        {
            var response = papi.PatronTitleListCopyTitle(Settings.PatronBarcode, 1234, 1234, 1234, Settings.PatronPin);
            Assert.IsTrue(response.Data.PAPIErrorCode == -1);
        }

        [TestMethod()]
        public void PatronTitleListDeleteAllTitlesTest()
        {
            var response = papi.PatronTitleListDeleteAllTitles(Settings.PatronBarcode, 1234, Settings.PatronPin);
            Assert.IsTrue(response.Data.PAPIErrorCode == -1);
        }

        [TestMethod()]
        public void PatronTitleListDeleteTitleTest()
        {
            var response = papi.PatronTitleListDeleteTitle(Settings.PatronBarcode, 1234, 1234, Settings.PatronPin);
            Assert.IsTrue(response.Data.PAPIErrorCode == -1);
        }

        [TestMethod()]
        public void PatronTitleListGetTitlesTest()
        {
            var response = papi.PatronTitleListGetTitles(Settings.PatronBarcode, 1234, password: Settings.PatronPin);
            Assert.IsTrue(response.Data.PAPIErrorCode == -1);
        }

        [TestMethod()]
        public void PatronTitleListMoveTitleTest()
        {
            var response = papi.PatronTitleListMoveTitle(Settings.PatronBarcode, 1234, 1234, 1234, Settings.PatronPin);
            Assert.IsTrue(response.Data.PAPIErrorCode == -1);
        }

        [TestMethod()]
        public void PatronUpdateTest()
        {
            var response = papi.PatronUpdate(Settings.PatronBarcode, new PatronUpdateParams(), Settings.PatronPin);
            Assert.IsTrue(response.Data.PAPIErrorCode == 0);
        }

        [TestMethod()]
        public void PatronUpdateUserNameTest()
        {
            var response = papi.PatronUpdateUserName(Settings.PatronBarcode + "1234", Settings.PatronPin, Settings.PatronPin);
            Assert.IsTrue(response.Response.StatusCode == HttpStatusCode.Unauthorized);
        }

        [TestMethod()]
        public void PatronValidateTest()
        {
            var response = papi.PatronValidate(Settings.PatronBarcode, Settings.PatronPin);
            Assert.IsTrue(response.Data.PatronID == Settings.PatronId);
        }

        [TestMethod()]
        public void PickupBranchesGetTest()
        {
            var response = papi.PickupBranchesGet();
            Assert.IsTrue(response.Data.PickupBranchesRows.Any());
        }

        [TestMethod()]
        public void RecordSetContentAddTest()
        {
            var response = papi.RecordSetContentAdd(1234, 1234);
            Assert.IsTrue(response.Data.PAPIErrorCode == -11001);
        }

        [TestMethod()]
        public void RecordSetContentAddTest_List()
        {
            var response = papi.RecordSetContentAdd(1234, new[] { 1234 });
            Assert.IsTrue(response.Data.PAPIErrorCode == -11001);
        }

        [TestMethod()]
        public void RecordSetContentRemoveTest()
        {
            var response = papi.RecordSetContentRemove(1234, 1234);
            Assert.IsTrue(response.Data.PAPIErrorCode == -11001);
        }

        [TestMethod()]
        public void RecordSetContentRemoveTest_List()
        {
            var response = papi.RecordSetContentRemove(1234, new[] { 1234 });
            Assert.IsTrue(response.Data.PAPIErrorCode == -11001);
        }

        [TestMethod()]
        public void RecordSetRecordsGetTest()
        {
            var response = papi.RecordSetRecordsGet(1234);
            Assert.IsTrue(response.Data.PAPIErrorCode == -11001);
        }

        [TestMethod()]
        public void RemoteStorageItemsGetTest()
        {
            var response = papi.RemoteStorageItemsGet(7, "asdf", "asdf", 1, 1);
            Assert.IsTrue(response.Data.PAPIErrorCode == -1);
        }

        [TestMethod()]
        public void SA_GetValueByOrgTest()
        {
            var response = papi.SA_GetValueByOrg("ORGEMAIL");
            Assert.IsTrue(response.Data.Value == Settings.OrgEmail);
        }

        [TestMethod()]
        public void ShelfLocationsGetTest()
        {
            var response = papi.ShelfLocationsGet(7);
            Assert.IsTrue(response.Data.PAPIErrorCode == response.Data.ShelfLocationsRows.Count());
        }

        [TestMethod()]
        public void Synch_BibsByIdGetTest()
        {
            var response = papi.Synch_BibsByIdGet(bibId);
            Assert.IsTrue(response.Response.IsSuccessStatusCode);
        }
    }
}