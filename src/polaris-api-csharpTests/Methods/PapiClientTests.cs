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
    public class PapiClientTests
    {
        private const string TestArtifactPrefix = "PAPI_TEST_";

        TestSettings Settings = null!;
        PapiSettings PapiSettings = null!;

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

            if (!IntegrationTestRequirements.HasRequiredPapiSettings(papiSettings) || !IntegrationTestRequirements.HasRequiredTestSettings(testSettings))
            {
                Assert.Inconclusive(IntegrationTestRequirements.MissingIntegrationConfigurationMessage);
            }

            papi = new PapiClient(papiSettings!);
            PapiSettings = papiSettings!;
            Settings = testSettings!;
        }

        private static string CreateUniqueTestArtifactText(string? baseName = null, int maxLength = 80)
        {
            var sanitizedBaseName = SanitizeArtifactBaseName(baseName);
            var uniqueSuffix = $"{DateTime.UtcNow:yyyyMMddHHmmss}_{Guid.NewGuid():N}";
            var suffixWithPrefix = $"{TestArtifactPrefix}{uniqueSuffix}";

            if (string.IsNullOrWhiteSpace(sanitizedBaseName))
            {
                return suffixWithPrefix;
            }

            var baseNameBudget = maxLength - suffixWithPrefix.Length - 1;
            if (baseNameBudget <= 0)
            {
                return suffixWithPrefix;
            }

            var trimmedBaseName = sanitizedBaseName.Length <= baseNameBudget
                ? sanitizedBaseName
                : sanitizedBaseName[..baseNameBudget];

            return $"{TestArtifactPrefix}{trimmedBaseName}_{uniqueSuffix}";
        }

        private static string SanitizeArtifactBaseName(string? baseName)
        {
            if (string.IsNullOrWhiteSpace(baseName))
            {
                return string.Empty;
            }

            var sanitized = new string(baseName
                .Where(c => char.IsLetterOrDigit(c) || c == '-' || c == '_')
                .ToArray());

            return sanitized.Trim('_', '-');
        }

        [TestMethod()]
        [ReadOnlyIntegrationCategory]
        public async Task ApiKeyValidateTest()
        {
            var response = await papi.ApiKeyValidateAsync();
            Assert.AreEqual(response.Data.PAPIErrorCode, 0);
        }

        [TestMethod()]
        [ReadOnlyIntegrationCategory]
        public async Task ApiVersionGetTest()
        {
            var response = await papi.ApiVersionGetAsync();
            Assert.AreEqual(response.Data.PAPIErrorCode, 0);
            Assert.IsFalse(string.IsNullOrWhiteSpace(response.Data.ToString()));
        }

        [TestMethod()]
        [ProtectedReadOnlyIntegrationCategory]
        public async Task AuthenticateStaffUserTest()
        {
            var staffOverrideAccount = papi.StaffOverrideAccount;
            IntegrationTestRequirements.RequireStaffOverrideAccount(PapiSettings);

            var response = await papi.AuthenticateStaffUserAsync(staffOverrideAccount!);
            Assert.AreEqual(response.Data.PAPIErrorCode, 0);
            Assert.IsFalse(string.IsNullOrWhiteSpace(response.Data.AccessSecret));
            Assert.IsFalse(string.IsNullOrWhiteSpace(response.Data.AccessToken));
        }

        [TestMethod()]
        [ReadOnlyIntegrationCategory]
        public async Task BibGetTest()
        {
            var response = await papi.BibGetAsync(478907);
            Assert.AreEqual(response.Data.PAPIErrorCode, 0);
            Assert.IsFalse(string.IsNullOrWhiteSpace(response.Data.Title));
            Assert.IsTrue(response.Response.RequestMessage.RequestUri.ToString().Contains("100/1/bib"));

        }

        [TestMethod()]
        [ReadOnlyIntegrationCategory]
        public async Task BibGetTest_PassBranchId()
        {
            var response = await papi.BibGetAsync(bibId, 7);
            Assert.AreEqual(response.Data.PAPIErrorCode, 0);
            Assert.IsFalse(string.IsNullOrWhiteSpace(response.Data.Title));
            Assert.IsTrue(response.Response.RequestMessage.RequestUri.ToString().Contains("100/7/bib"));
        }

        [TestMethod()]
        [ReadOnlyIntegrationCategory]
        public async Task BibSearchTest()
        {
            var response = await papi.BibSearchAsync(new BibSearchOptions { Term = "dogs", PageSize = 10 });
            Assert.IsTrue(response.Data.PAPIErrorCode == 10);
            Assert.IsTrue(response.Data.WordList == "dogs ");
            Assert.IsTrue(response.Data.TotalRecordsFound > 10000);
        }

        [TestMethod()]
        [ReadOnlyIntegrationCategory]
        public async Task CollectionsGetTest()
        {
            var response = await papi.CollectionsGetAsync();
            Assert.IsTrue(response.Data.PAPIErrorCode > 300);
            Assert.IsTrue(response.Data.CollectionsRows.Count > 300);
            Assert.AreEqual(response.Data.PAPIErrorCode, response.Data.CollectionsRows.Count);
        }

        [TestMethod()]
        [ProtectedMutatingIntegrationCategory]
        [DoNotParallelize]
        public async Task CreatePatronBlocksTest_FreeTextBlock()
        {
            IntegrationTestRequirements.RequireStaffOverrideAccount(PapiSettings);

            var blockText = CreateUniqueTestArtifactText(Settings.FreeTextBlock, maxLength: 80);
            var response = await papi.CreatePatronBlocksAsync(Settings.PatronBarcode, BlockType.FreeText, blockText);
            Assert.IsTrue(new[] { 0, -3507 }.Contains(response.Data.PAPIErrorCode));
        }

        [TestMethod()]
        [ProtectedMutatingIntegrationCategory]
        [DoNotParallelize]
        public async Task CreatePatronBlocksTest_SystemBlock()
        {
            IntegrationTestRequirements.RequireStaffOverrideAccount(PapiSettings);

            var response = await papi.CreatePatronBlocksAsync(Settings.PatronBarcode, BlockType.System, "128");
            Assert.IsTrue(new[] { 0, -3507 }.Contains(response.Data.PAPIErrorCode));
        }

        [TestMethod()]
        [ProtectedMutatingIntegrationCategory]
        [DoNotParallelize]
        public async Task CreatePatronBlocksTest_LibraryAssignedBlock()
        {
            IntegrationTestRequirements.RequireStaffOverrideAccount(PapiSettings);

            var response = await papi.CreatePatronBlocksAsync(Settings.PatronBarcode, BlockType.LibraryAssigned, "1");
            Assert.IsTrue(new[] { 0, -3507 }.Contains(response.Data.PAPIErrorCode));
        }

        [TestMethod()]
        [ReadOnlyIntegrationCategory]
        public async Task DatesClosedGetTest()
        {
            var response = await papi.DatesClosedGetAsync(7);
            Assert.IsTrue(response.Data.DatesClosedRows.Any());
        }

        [TestMethod()]
        [ReadOnlyIntegrationCategory]
        public async Task HoldingsGetTest()
        {
            var response = await papi.HoldingsGetAsync(bibId);
            Assert.IsTrue(response.Data.BibHoldingsGetRows.Any());
        }

        [TestMethod()]
        [MutatingIntegrationCategory]
        [DoNotParallelize]
        public async Task HoldRequestCancelTest()
        {
            var response = await papi.HoldRequestCancelAsync(Settings.PatronBarcode, 1234, Settings.PatronPin);
            Assert.IsTrue(response.Data.PAPIErrorCode == -4201);
        }

        [TestMethod()]
        [MutatingIntegrationCategory]
        [DoNotParallelize]
        public async Task HoldRequestCreateTest()
        {
            var response = await papi.HoldRequestCreateAsync(new HoldRequestCreateParams(Settings.PatronId, 1234, 7, 7));
            Assert.IsTrue(response.Data.PAPIErrorCode == -4006);
        }

        [TestMethod()]
        [MutatingIntegrationCategory]
        [DoNotParallelize]
        public async Task HoldRequestCreateTest2()
        {
            var response = await ((PapiClient)papi).HoldRequestCreateAsync(Settings.PatronId, 1234, 7);
            Assert.IsTrue(response.Data.PAPIErrorCode == -4006);
        }

        [TestMethod()]
        [ProtectedReadOnlyIntegrationCategory]
        public async Task HoldRequestGetListTest()
        {
            IntegrationTestRequirements.RequireStaffOverrideAccount(PapiSettings);

            var response = await papi.HoldRequestGetListAsync(7);
            Assert.IsTrue(response.Data.PAPIErrorCode == 0);
        }

        [TestMethod()]
        [MutatingIntegrationCategory]
        [DoNotParallelize]
        public async Task HoldRequestReactivateTest()
        {
            var response = await papi.HoldRequestReactivateAsync(Settings.PatronBarcode, Settings.PatronPin, 1234, DateTime.Now);
            Assert.IsTrue(response.Data.PAPIErrorCode == -4201);
        }

        [TestMethod()]
        [MutatingIntegrationCategory]
        [DoNotParallelize]
        public async Task HoldRequestReplyTest()
        {
            var hold = new HoldRequestCreateResult
            {
                RequestGuid = new Guid(),
                TxnGroupQualifier = "test",
                TxnQualifier = "test"
            };
            var response = await papi.HoldRequestReplyAsync(hold, 7, HoldRequestReplyAnswer.Yes, HoldRequestReplyState.AcceptEvenWithExistingHolds);
            Assert.IsTrue(response.Data.PAPIErrorCode == -4101);
        }

        [TestMethod()]
        [MutatingIntegrationCategory]
        [DoNotParallelize]
        public async Task HoldRequestSuspendTest()
        {
            var response = await papi.HoldRequestSuspendAsync(Settings.PatronBarcode, 1234, DateTime.Now, Settings.PatronPin);
            Assert.IsTrue(response.Data.PAPIErrorCode == -4201);
        }

        [TestMethod()]
        [MutatingIntegrationCategory]
        [DoNotParallelize]
        public async Task ItemRenewTest()
        {
            var response = await papi.ItemRenewAsync(Settings.PatronBarcode, 1234, Settings.PatronPin);
            Assert.IsTrue(response.Data.PAPIErrorCode == -6001);
        }

        [TestMethod()]
        [ReadOnlyIntegrationCategory]
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
        [ReadOnlyIntegrationCategory]
        public async Task LimitFiltersGetTest()
        {
            var response = (await papi.LimitFiltersGetAsync()).Data;
            Assert.IsTrue(response.LimitFiltersRows.Count() == response.PAPIErrorCode);
        }

        [TestMethod()]
        [ReadOnlyIntegrationCategory]
        public async Task MARCTypeOfMaterialsGetAsyncTest()
        {
            var response = (await papi.MARCTypeOfMaterialsGetAsync()).Data;
            Assert.IsTrue(response.MARCTypeOfMaterialsRows.Count() == response.PAPIErrorCode);
        }

        [TestMethod()]
        [ProtectedMutatingIntegrationCategory]
        [DoNotParallelize]
        public async Task NotificationUpdateTest()
        {
            IntegrationTestRequirements.RequireStaffOverrideAccount(PapiSettings);

            var response = (await papi.NotificationUpdateAsync(new NotificationUpdateParams { PatronId = Settings.PatronId, DeliveryString = "test@test.test", ReportingOrgID = 7, NotificationDeliveryDate = DateTime.Now, DeliveryOptionId = 2, Details = CreateUniqueTestArtifactText(maxLength: 80), NotificationStatusId = NotificationStatus.EmailCompleted, NotificationTypeId = 1 })).Data;
            Assert.IsTrue(response.PAPIErrorCode == -1);
        }

        [TestMethod()]
        [ReadOnlyIntegrationCategory]
        public async Task OrganizationsGetTest()
        {
            var response = await papi.OrganizationsGetAsync();
            Assert.IsTrue(response.Data.OrganizationsGetRows.Count() == response.Data.PAPIErrorCode);
        }

        [TestMethod()]
        [ProtectedReadOnlyIntegrationCategory]
        public async Task Patron_GetBarcodeFromIdTest()
        {
            IntegrationTestRequirements.RequireStaffOverrideAccount(PapiSettings);

            var response = await papi.Patron_GetBarcodeFromIdAsync(Settings.PatronId);
            Assert.IsTrue(response.Data.Barcode == Settings.PatronBarcode);
        }

        [TestMethod()]
        [ProtectedMutatingIntegrationCategory]
        [DoNotParallelize]
        public async Task PatronAccountCreateCreditTest()
        {
            IntegrationTestRequirements.RequireStaffOverrideAccount(PapiSettings);

            var response = await papi.PatronAccountCreateCreditAsync(Settings.PatronBarcode, .01, PaymentMethod.Cash, note: CreateUniqueTestArtifactText(maxLength: 80));
            Assert.IsTrue(response.Data.PAPIErrorCode == 0);
        }

        [TestMethod()]
        [MutatingIntegrationCategory]
        [DoNotParallelize]
        public async Task TestTitleListCreate_Get_Delete()
        {
            var listName = CreateUniqueTestArtifactText(Settings.PatronListName);

            var createResponse = await papi.PatronAccountCreateTitleListAsync(Settings.PatronBarcode, listName, Settings.PatronPin);
            Assert.AreEqual(0, createResponse.Data.PAPIErrorCode);

            var getResponse = await papi.PatronAccountGetTitleListsAsync(Settings.PatronBarcode, Settings.PatronPin);
            var list = getResponse.Data.PatronAccountTitleListsRows.FirstOrDefault(l => l.RecordStoreName == listName);

            Assert.IsNotNull(list, $"Expected to find uniquely named title list '{listName}'.");

            var deleteResponse = await papi.PatronAccountDeleteTitleListAsync(Settings.PatronBarcode, list.RecordStoreId, Settings.PatronPin);
            Assert.IsTrue(deleteResponse.Data.PAPIErrorCode == 0);
        }

        [TestMethod()]
        [ProtectedMutatingIntegrationCategory]
        [DoNotParallelize]
        public async Task PatronAccountDepositCreditTest()
        {
            IntegrationTestRequirements.RequireStaffOverrideAccount(PapiSettings);

            var response = await papi.PatronAccountDepositCreditAsync(Settings.PatronBarcode, .01, note: CreateUniqueTestArtifactText(maxLength: 80));
            Assert.IsTrue(response.Data.PAPIErrorCode == 0);
        }

        [TestMethod()]
        [ReadOnlyIntegrationCategory]
        public async Task PatronAccountGetTest()
        {
            var response = await papi.PatronAccountGetAsync(Settings.PatronBarcode, Settings.PatronPin);
            Assert.IsTrue(response.Data.PAPIErrorCode == 0);
            Assert.IsTrue(response.Data.PatronAccountGetRows.Any());
        }

        [TestMethod()]
        [ProtectedMutatingIntegrationCategory]
        [DoNotParallelize]
        public async Task PatronAccountPayTest()
        {
            IntegrationTestRequirements.RequireStaffOverrideAccount(PapiSettings);

            var response = (await papi.PatronAccountPayAsync(Settings.PatronBarcode, 1234, .01, PaymentMethod.Cash, note: CreateUniqueTestArtifactText(maxLength: 80))).Data;
            Assert.IsTrue(response.PAPIErrorCode == -3600);
        }

        [TestMethod()]
        [ProtectedMutatingIntegrationCategory]
        [DoNotParallelize]
        public async Task PatronAccountPayAllTest()
        {
            IntegrationTestRequirements.RequireStaffOverrideAccount(PapiSettings);

            var response = await papi.PatronAccountPayAllAsync(Settings.PatronBarcode, 999999.99, PaymentMethod.Cash, note: CreateUniqueTestArtifactText(maxLength: 80));
            Assert.IsTrue(response.Data.PAPIErrorCode == -3610);
        }

        [TestMethod()]
        [ProtectedMutatingIntegrationCategory]
        [DoNotParallelize]
        public async Task PatronAccountRefundCreditTest()
        {
            IntegrationTestRequirements.RequireStaffOverrideAccount(PapiSettings);

            var response = await papi.PatronAccountRefundCreditAsync(Settings.PatronBarcode, 999999.99, note: CreateUniqueTestArtifactText(maxLength: 80));
            Assert.IsTrue(response.Data.PAPIErrorCode == -3606);
        }

        [TestMethod()]
        [ProtectedMutatingIntegrationCategory]
        [DoNotParallelize]
        public async Task PatronAccountVoidTest()
        {
            IntegrationTestRequirements.RequireStaffOverrideAccount(PapiSettings);

            var response = await papi.PatronAccountVoidAsync(Settings.PatronBarcode, 1234, note: CreateUniqueTestArtifactText(maxLength: 80));
            Assert.IsTrue(response.Data.PAPIErrorCode == -3606);
        }

        [TestMethod()]
        [ReadOnlyIntegrationCategory]
        public async Task PatronBasicDataGetTest()
        {
            var response = await papi.PatronBasicDataGetAsync(Settings.PatronBarcode, Settings.PatronPin, true);
            Assert.IsTrue(response.Data.PAPIErrorCode == 0);
            Assert.IsTrue(response.Data.PatronBasicData.PatronID == Settings.PatronId);
            Assert.IsTrue(response.Data.PatronBasicData.PatronAddresses.Any());
        }

        [TestMethod()]
        [ReadOnlyIntegrationCategory]
        public async Task PatronCirculateBlocksGetTest()
        {
            var response = await papi.PatronCirculateBlocksGetAsync(Settings.PatronBarcode, Settings.PatronPin);
            Assert.IsTrue(response.Data.PAPIErrorCode == 0);
        }

        [TestMethod()]
        [ReadOnlyIntegrationCategory]
        public async Task PatronCodesGetTest()
        {
            var response = await papi.PatronCodesGetAsync();
            Assert.IsTrue(response.Data.PAPIErrorCode == 0);
            Assert.IsTrue(response.Data.PatronCodesRows.Any());
        }

        [TestMethod()]
        [ReadOnlyIntegrationCategory]
        public async Task PatronHoldRequestsGetTest()
        {
            var response = await papi.PatronHoldRequestsGetAsync(Settings.PatronBarcode, PatronHoldStatus.all, Settings.PatronPin);
            Assert.IsTrue(response.Data.PAPIErrorCode == 0);
            //Assert.IsTrue(response.Data.PatronHoldRequestsGetRows.Any());
        }

        [TestMethod()]
        [ReadOnlyIntegrationCategory]
        public async Task PatronILLRequestsGetTest()
        {
            var response = await papi.PatronILLRequestsGetAsync(Settings.PatronBarcode, password: Settings.PatronPin);
            Assert.IsTrue(response.Data.PAPIErrorCode == 0);
            //Assert.IsTrue(response.Data.PatronILLRequestsGetRows.Any());
        }

        [TestMethod()]
        [ReadOnlyIntegrationCategory]
        public async Task PatronItemsOutGetTest()
        {
            var response = await papi.PatronItemsOutGetAsync(Settings.PatronBarcode, password: Settings.PatronPin);
            Assert.IsTrue(response.Data.PAPIErrorCode == 0);
            //Assert.IsTrue(response.Data.PatronItemsOutGetRows.Any());
        }

        [TestMethod()]
        [MutatingIntegrationCategory]
        [DoNotParallelize]
        public async Task PatronMessageDeleteTest()
        {
            var response = await papi.PatronMessageDeleteAsync(Settings.PatronBarcode, PatronMessageType.freetext, 1234, Settings.PatronPin);
            Assert.IsTrue(response.Data.PAPIErrorCode == -1);
        }

        [TestMethod()]
        [ReadOnlyIntegrationCategory]
        public async Task PatronMessagesGetTest()
        {
            var response = await papi.PatronMessagesGetAsync(Settings.PatronBarcode, password: Settings.PatronPin);
            Assert.IsTrue(response.Data.PAPIErrorCode == 0);
        }

        [TestMethod()]
        [MutatingIntegrationCategory]
        [DoNotParallelize]
        public async Task PatronMessageUpdateStatusTest()
        {
            var response = await papi.PatronMessageUpdateStatusAsync(Settings.PatronBarcode, PatronMessageType.freetext, 1234, Settings.PatronPin);
            Assert.IsTrue(response.Data.PAPIErrorCode == -1);
        }

        [TestMethod()]
        [ReadOnlyIntegrationCategory]
        public async Task PatronPreferencesGetTest()
        {
            var response = await papi.PatronPreferencesGetAsync(Settings.PatronBarcode, Settings.PatronPin);
            Assert.IsTrue(response.Data.PatronPreferences.PatronID == Settings.PatronId);
        }

        [TestMethod()]
        [MutatingIntegrationCategory]
        [DoNotParallelize]
        public async Task PatronReadingHistoryClearTest()
        {
            var response = await papi.PatronReadingHistoryClearAsync(Settings.PatronBarcode, Settings.PatronPin, new[] { 1234 });
            Assert.IsTrue(response.Data.PAPIErrorCode == -10);
        }

        [TestMethod()]
        [ReadOnlyIntegrationCategory]
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
        [ProtectedReadOnlyIntegrationCategory]
        public async Task PatronRenewBlocksGetTest()
        {
            IntegrationTestRequirements.RequireStaffOverrideAccount(PapiSettings);

            var response = await papi.PatronRenewBlocksGetAsync(Settings.PatronId);
            Assert.IsTrue(response.Data.PAPIErrorCode == 0);
        }

        [TestMethod()]
        [ReadOnlyIntegrationCategory]
        public async Task PatronSavedSearchesGetTest()
        {
            var response = await papi.PatronSavedSearchesGetAsync(Settings.PatronBarcode, Settings.PatronPin);
            Assert.IsTrue(response.Data.PAPIErrorCode == 0);
        }

        [TestMethod()]
        [ProtectedReadOnlyIntegrationCategory]
        public async Task PatronSearchTest()
        {
            IntegrationTestRequirements.RequireStaffOverrideAccount(PapiSettings);

            var response = await papi.PatronSearchAsync($"PRID={Settings.PatronId}");
            Assert.IsTrue(response.Data.PAPIErrorCode == response.Data.PatronSearchRows.Count);
        }

        [TestMethod()]
        [MutatingIntegrationCategory]
        [DoNotParallelize]
        public async Task PatronTitleListAddTitleTest()
        {
            var response = await papi.PatronTitleListAddTitleAsync(Settings.PatronBarcode, 1234, 1234, Settings.PatronPin);
            Assert.IsTrue(response.Data.PAPIErrorCode == -1);
        }

        [TestMethod()]
        [MutatingIntegrationCategory]
        [DoNotParallelize]
        public async Task PatronTitleListCopyAllTitlesTest()
        {
            var response = await papi.PatronTitleListCopyAllTitlesAsync(Settings.PatronBarcode, 1234, 1234, Settings.PatronPin);
            Assert.IsTrue(response.Data.PAPIErrorCode == -1);
        }

        [TestMethod()]
        [MutatingIntegrationCategory]
        [DoNotParallelize]
        public async Task PatronTitleListCopyTitleTest()
        {
            var response = await papi.PatronTitleListCopyTitleAsync(Settings.PatronBarcode, 1234, 1234, 1234, Settings.PatronPin);
            Assert.IsTrue(response.Data.PAPIErrorCode == -1);
        }

        [TestMethod()]
        [MutatingIntegrationCategory]
        [DoNotParallelize]
        public async Task PatronTitleListDeleteAllTitlesTest()
        {
            var response = await papi.PatronTitleListDeleteAllTitlesAsync(Settings.PatronBarcode, 1234, Settings.PatronPin);
            Assert.IsTrue(response.Data.PAPIErrorCode == -1);
        }

        [TestMethod()]
        [MutatingIntegrationCategory]
        [DoNotParallelize]
        public async Task PatronTitleListDeleteTitleTest()
        {
            var response = await papi.PatronTitleListDeleteTitleAsync(Settings.PatronBarcode, 1234, 1234, Settings.PatronPin);
            Assert.IsTrue(response.Data.PAPIErrorCode == -1);
        }

        [TestMethod()]
        [ReadOnlyIntegrationCategory]
        public async Task PatronTitleListGetTitlesTest()
        {
            var response = await papi.PatronTitleListGetTitlesAsync(Settings.PatronBarcode, 1234, password: Settings.PatronPin);
            Assert.IsTrue(response.Data.PAPIErrorCode == -1);
        }

        [TestMethod()]
        [MutatingIntegrationCategory]
        [DoNotParallelize]
        public async Task PatronTitleListMoveTitleTest()
        {
            var response = await papi.PatronTitleListMoveTitleAsync(Settings.PatronBarcode, 1234, 1234, 1234, Settings.PatronPin);
            Assert.IsTrue(response.Data.PAPIErrorCode == -1);
        }

        [TestMethod()]
        [MutatingIntegrationCategory]
        [DoNotParallelize]
        public async Task PatronUpdateTest()
        {
            var response = await papi.PatronUpdateAsync(Settings.PatronBarcode, new PatronUpdateParams(), Settings.PatronPin);
            Assert.IsTrue(response.Data.PAPIErrorCode == 0);
        }

        [TestMethod()]
        [MutatingIntegrationCategory]
        [DoNotParallelize]
        public async Task PatronUpdateUserNameTest()
        {
            var response = await papi.PatronUpdateUserNameAsync(Settings.PatronBarcode + "1234", Settings.PatronPin, Settings.PatronPin);
            Assert.IsTrue(response.Response.StatusCode == HttpStatusCode.Unauthorized);
        }

        [TestMethod()]
        [ReadOnlyIntegrationCategory]
        public async Task PatronValidateTest()
        {
            var response = await papi.PatronValidateAsync(Settings.PatronBarcode, Settings.PatronPin);
            Assert.IsTrue(response.Data.PatronID == Settings.PatronId);
        }

        [TestMethod()]
        [ReadOnlyIntegrationCategory]
        public async Task PickupBranchesGetTest()
        {
            var response = await papi.PickupBranchesGetAsync();
            Assert.IsTrue(response.Data.PickupBranchesRows.Any());
        }

        [TestMethod()]
        [ProtectedMutatingIntegrationCategory]
        [DoNotParallelize]
        public async Task RecordSetContentAddTest()
        {
            IntegrationTestRequirements.RequireStaffOverrideAccount(PapiSettings);

            var response = await papi.RecordSetContentAddAsync(1234, 1234);
            Assert.IsTrue(response.Data.PAPIErrorCode == -11001);
        }

        [TestMethod()]
        [ProtectedMutatingIntegrationCategory]
        [DoNotParallelize]
        public async Task RecordSetContentAddTest_List()
        {
            IntegrationTestRequirements.RequireStaffOverrideAccount(PapiSettings);

            var response = await papi.RecordSetContentAddAsync(1234, new[] { 1234 });
            Assert.IsTrue(response.Data.PAPIErrorCode == -11001);
        }

        [TestMethod()]
        [ProtectedMutatingIntegrationCategory]
        [DoNotParallelize]
        public async Task RecordSetContentRemoveTest()
        {
            IntegrationTestRequirements.RequireStaffOverrideAccount(PapiSettings);

            var response = await papi.RecordSetContentRemoveAsync(1234, 1234);
            Assert.IsTrue(response.Data.PAPIErrorCode == -11001);
        }

        [TestMethod()]
        [ProtectedMutatingIntegrationCategory]
        [DoNotParallelize]
        public async Task RecordSetContentRemoveTest_List()
        {
            IntegrationTestRequirements.RequireStaffOverrideAccount(PapiSettings);

            var response = await papi.RecordSetContentRemoveAsync(1234, new[] { 1234 });
            Assert.IsTrue(response.Data.PAPIErrorCode == -11001);
        }

        [TestMethod()]
        [ProtectedReadOnlyIntegrationCategory]
        public async Task RecordSetRecordsGetTest()
        {
            IntegrationTestRequirements.RequireStaffOverrideAccount(PapiSettings);

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
        [ProtectedReadOnlyIntegrationCategory]
        public async Task SA_GetValueByOrgTest()
        {
            IntegrationTestRequirements.RequireStaffOverrideAccount(PapiSettings);

            var response = await papi.SA_GetValueByOrgAsync("ORGEMAIL");
            Assert.IsTrue(response.Data.Value == Settings.OrgEmail);
        }

        [TestMethod()]
        [ReadOnlyIntegrationCategory]
        public async Task ShelfLocationsGetTest()
        {
            var response = await papi.ShelfLocationsGetAsync(7);
            Assert.IsTrue(response.Data.PAPIErrorCode == response.Data.ShelfLocationsRows.Count());
        }

        [TestMethod()]
        [ProtectedReadOnlyIntegrationCategory]
        public async Task Synch_BibsByIdGetTest()
        {
            IntegrationTestRequirements.RequireStaffOverrideAccount(PapiSettings);

            var response = await papi.Synch_BibsByIdGetAsync(bibId);
            Assert.IsTrue(response.Response.IsSuccessStatusCode);
        }
    }
}
