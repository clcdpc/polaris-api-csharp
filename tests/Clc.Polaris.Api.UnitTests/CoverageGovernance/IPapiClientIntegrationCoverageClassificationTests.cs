using System.Reflection;

namespace Clc.Polaris.Api.UnitTests
{
    [TestClass]
    [UnitTest]
    public sealed class IPapiClientIntegrationCoverageClassificationTests
    {
        private enum LiveCoverageClassification
        {
            UnitOnly,
            ReadOnly,
            StaffReadOnly,
            Mutating,
            StaffMutating,
            NotLiveTestedWithReason,
        }

        private sealed record MethodCoverage(LiveCoverageClassification Classification, string? Reason = null);

        private static readonly Dictionary<string, MethodCoverage> Classifications = new()
        {
            [nameof(IPapiClient.ApiKeyValidateAsync)] = new(LiveCoverageClassification.ReadOnly),
            [nameof(IPapiClient.ApiVersionGetAsync)] = new(LiveCoverageClassification.ReadOnly),
            [nameof(IPapiClient.AuthenticatePatronAsync)] = new(LiveCoverageClassification.ReadOnly),
            [nameof(IPapiClient.AuthenticateStaffUserAsync)] = new(LiveCoverageClassification.StaffReadOnly),
            [nameof(IPapiClient.BibGetAsync)] = new(LiveCoverageClassification.ReadOnly),
            [nameof(IPapiClient.BibGetByTypeV2Async)] = new(LiveCoverageClassification.ReadOnly),
            [nameof(IPapiClient.BibSearchAsync)] = new(LiveCoverageClassification.ReadOnly),
            [nameof(IPapiClient.CollectionsGetAsync)] = new(LiveCoverageClassification.ReadOnly),
            [nameof(IPapiClient.CreatePatronBlocksAsync)] = new(LiveCoverageClassification.StaffMutating),
            [nameof(IPapiClient.DatesClosedGetAsync)] = new(LiveCoverageClassification.ReadOnly),
            [nameof(IPapiClient.HeadingsSearchAsync)] = new(LiveCoverageClassification.ReadOnly),
            [nameof(IPapiClient.HoldingsGetAsync)] = new(LiveCoverageClassification.ReadOnly),
            [nameof(IPapiClient.HoldRequestCancelAsync)] = new(LiveCoverageClassification.Mutating),
            [nameof(IPapiClient.HoldRequestCreateAsync)] = new(LiveCoverageClassification.Mutating),
            [nameof(IPapiClient.HoldRequestGetListAsync)] = new(LiveCoverageClassification.StaffReadOnly),
            [nameof(IPapiClient.HoldRequestReactivateAsync)] = new(LiveCoverageClassification.Mutating),
            [nameof(IPapiClient.HoldRequestReplyAsync)] = new(LiveCoverageClassification.Mutating),
            [nameof(IPapiClient.HoldRequestSuspendAsync)] = new(LiveCoverageClassification.Mutating),
            [nameof(IPapiClient.ILLRequestCancelAsync)] = new(LiveCoverageClassification.Mutating),
            [nameof(IPapiClient.ItemCheckInPostAsync)] = new(LiveCoverageClassification.StaffMutating),
            [nameof(IPapiClient.ItemCheckOutPostAsync)] = new(LiveCoverageClassification.StaffMutating),
            [nameof(IPapiClient.ItemRenewAsync)] = new(LiveCoverageClassification.Mutating),
            [nameof(IPapiClient.ItemStatusesGetAsync)] = new(LiveCoverageClassification.ReadOnly),
            [nameof(IPapiClient.ItemUpdateBarcodeAsync)] = new(LiveCoverageClassification.Mutating),
            [nameof(IPapiClient.LimitFiltersGetAsync)] = new(LiveCoverageClassification.ReadOnly),
            [nameof(IPapiClient.MARCTypeOfMaterialsGetAsync)] = new(LiveCoverageClassification.ReadOnly),
            [nameof(IPapiClient.MaterialTypesGetAsync)] = new(LiveCoverageClassification.ReadOnly),
            [nameof(IPapiClient.MultipartGetAsync)] = new(LiveCoverageClassification.ReadOnly),
            [nameof(IPapiClient.NotificationUpdateAsync)] = new(LiveCoverageClassification.StaffMutating),
            [nameof(IPapiClient.NotificationQueueGetAsync)] = new(LiveCoverageClassification.UnitOnly, "Request-shape coverage only; live queue contents depend on environment-specific notification setup."),
            [nameof(IPapiClient.OrganizationsGetAsync)] = new(LiveCoverageClassification.ReadOnly),
            [nameof(IPapiClient.PatronAccountCreateCreditAsync)] = new(LiveCoverageClassification.StaffMutating),
            [nameof(IPapiClient.PatronAccountCreateTitleListAsync)] = new(LiveCoverageClassification.Mutating),
            [nameof(IPapiClient.PatronAccountDeleteTitleListAsync)] = new(LiveCoverageClassification.Mutating),
            [nameof(IPapiClient.PatronAccountDepositCreditAsync)] = new(LiveCoverageClassification.StaffMutating),
            [nameof(IPapiClient.PatronAccountGetAsync)] = new(LiveCoverageClassification.ReadOnly),
            [nameof(IPapiClient.PatronAccountGetTitleListsAsync)] = new(LiveCoverageClassification.ReadOnly),
            [nameof(IPapiClient.PatronAccountPayAsync)] = new(LiveCoverageClassification.StaffMutating),
            [nameof(IPapiClient.PatronAccountPayAllAsync)] = new(LiveCoverageClassification.StaffMutating),
            [nameof(IPapiClient.PatronAccountRefundCreditAsync)] = new(LiveCoverageClassification.StaffMutating),
            [nameof(IPapiClient.PatronAccountVoidAsync)] = new(LiveCoverageClassification.StaffMutating),
            [nameof(IPapiClient.PatronBasicDataGetAsync)] = new(LiveCoverageClassification.ReadOnly),
            [nameof(IPapiClient.PatronCirculateBlocksGetAsync)] = new(LiveCoverageClassification.ReadOnly),
            [nameof(IPapiClient.PatronCodesGetAsync)] = new(LiveCoverageClassification.ReadOnly),
            [nameof(IPapiClient.PatronHoldRequestsGetAsync)] = new(LiveCoverageClassification.ReadOnly),
            [nameof(IPapiClient.PatronLanguagesGetAsync)] = new(LiveCoverageClassification.ReadOnly),
            [nameof(IPapiClient.PatronNotesGetAsync)] = new(LiveCoverageClassification.ReadOnly),
            [nameof(IPapiClient.PatronILLRequestsGetAsync)] = new(LiveCoverageClassification.ReadOnly),
            [nameof(IPapiClient.PatronItemsOutGetAsync)] = new(LiveCoverageClassification.ReadOnly),
            [nameof(IPapiClient.PatronMessageDeleteAsync)] = new(LiveCoverageClassification.Mutating),
            [nameof(IPapiClient.PatronMessagesGetAsync)] = new(LiveCoverageClassification.ReadOnly),
            [nameof(IPapiClient.PatronMessageUpdateStatusAsync)] = new(LiveCoverageClassification.Mutating),
            [nameof(IPapiClient.PatronPreferencesGetAsync)] = new(LiveCoverageClassification.ReadOnly),
            [nameof(IPapiClient.PatronReadingHistoryClearAsync)] = new(LiveCoverageClassification.Mutating),
            [nameof(IPapiClient.PatronReadingHistoryGetAsync)] = new(LiveCoverageClassification.ReadOnly),
            [nameof(IPapiClient.PatronRegistrationCreateAsync)] = new(LiveCoverageClassification.NotLiveTestedWithReason, "Creates patrons and requires environment-specific registration policy data; covered by validation/request-shape tests."),
            [nameof(IPapiClient.PatronRegistrationCreateV2Async)] = new(LiveCoverageClassification.NotLiveTestedWithReason, "Creates patrons and requires environment-specific registration policy data; covered by validation/request-shape tests."),
            [nameof(IPapiClient.PatronRegistrationUpdateV2Async)] = new(LiveCoverageClassification.NotLiveTestedWithReason, "Updates patrons and requires a known disposable patron plus environment-specific registration policy data; covered by validation/request-shape tests."),
            [nameof(IPapiClient.PatronRenewBlocksGetAsync)] = new(LiveCoverageClassification.StaffReadOnly),
            [nameof(IPapiClient.PatronSavedSearchesGetAsync)] = new(LiveCoverageClassification.ReadOnly),
            [nameof(IPapiClient.PatronSearchAsync)] = new(LiveCoverageClassification.StaffReadOnly),
            [nameof(IPapiClient.PatronStatisticalClassesGetAsync)] = new(LiveCoverageClassification.ReadOnly),
            [nameof(IPapiClient.PatronTitleListAddTitleAsync)] = new(LiveCoverageClassification.Mutating),
            [nameof(IPapiClient.PatronTitleListCopyAllTitlesAsync)] = new(LiveCoverageClassification.Mutating),
            [nameof(IPapiClient.PatronTitleListCopyTitleAsync)] = new(LiveCoverageClassification.Mutating),
            [nameof(IPapiClient.PatronTitleListDeleteAllTitlesAsync)] = new(LiveCoverageClassification.Mutating),
            [nameof(IPapiClient.PatronTitleListDeleteTitleAsync)] = new(LiveCoverageClassification.Mutating),
            [nameof(IPapiClient.PatronTitleListGetTitlesAsync)] = new(LiveCoverageClassification.ReadOnly),
            [nameof(IPapiClient.PatronTitleListMoveTitleAsync)] = new(LiveCoverageClassification.Mutating),
            [nameof(IPapiClient.PatronUpdateAsync)] = new(LiveCoverageClassification.Mutating),
            [nameof(IPapiClient.PatronUdfConfigsGetAsync)] = new(LiveCoverageClassification.ReadOnly),
            [nameof(IPapiClient.PatronUpdateUserNameAsync)] = new(LiveCoverageClassification.Mutating),
            [nameof(IPapiClient.PatronValidateAsync)] = new(LiveCoverageClassification.ReadOnly),
            [nameof(IPapiClient.Patron_GetBarcodeFromIdAsync)] = new(LiveCoverageClassification.StaffReadOnly),
            [nameof(IPapiClient.PickupAreasGetAsync)] = new(LiveCoverageClassification.ReadOnly),
            [nameof(IPapiClient.PickupBranchesGetAsync)] = new(LiveCoverageClassification.ReadOnly),
            [nameof(IPapiClient.RecordSetContentPutAsync)] = new(LiveCoverageClassification.StaffMutating),
            [nameof(IPapiClient.RecordSetRecordsGetAsync)] = new(LiveCoverageClassification.StaffReadOnly),
            [nameof(IPapiClient.RequestsUpdateStatusAsync)] = new(LiveCoverageClassification.Mutating),
            [nameof(IPapiClient.RemoteStorageItemsGetAsync)] = new(LiveCoverageClassification.NotLiveTestedWithReason, "Requires remote-storage configuration and date windows that are not generally available in disposable environments."),
            [nameof(IPapiClient.SAMobilePhoneCarriersGetAsync)] = new(LiveCoverageClassification.ReadOnly),
            [nameof(IPapiClient.SA_GetValueByOrgAsync)] = new(LiveCoverageClassification.StaffReadOnly),
            [nameof(IPapiClient.SortOptionsGetAsync)] = new(LiveCoverageClassification.ReadOnly),
            [nameof(IPapiClient.ShelfLocationsGetAsync)] = new(LiveCoverageClassification.ReadOnly),
            [nameof(IPapiClient.SysHoldStatusesGetAsync)] = new(LiveCoverageClassification.ReadOnly),
            [nameof(IPapiClient.Synch_BibsByIdGetAsync)] = new(LiveCoverageClassification.StaffReadOnly),
            [nameof(IPapiClient.UpdatePickupBranchIDAsync)] = new(LiveCoverageClassification.NotLiveTestedWithReason, "Requires a known active hold that is safe to retarget; hold lifecycle tests cover related hold mutation paths."),
            [nameof(IPapiClient.UpdatePatronNotesDataAsync)] = new(LiveCoverageClassification.Mutating),
        };

        [TestMethod]
        public void EveryPublicIPapiClientMethodHasLiveCoverageClassification()
        {
            var publicMethods = typeof(IPapiClient)
                .GetMethods(BindingFlags.Instance | BindingFlags.Public)
                .Where(method => method.ReturnType.IsGenericType && method.ReturnType.GetGenericTypeDefinition() == typeof(Task<>))
                .Select(method => method.Name)
                .OrderBy(name => name)
                .ToArray();

            var missingMethods = publicMethods.Except(Classifications.Keys).ToArray();
            var extraMethods = Classifications.Keys.Except(publicMethods).ToArray();
            var missingReasons = Classifications
                .Where(entry => entry.Value.Classification == LiveCoverageClassification.NotLiveTestedWithReason && string.IsNullOrWhiteSpace(entry.Value.Reason))
                .Select(entry => entry.Key)
                .ToArray();

            Assert.IsEmpty(missingMethods, $"Missing live coverage classification for IPapiClient methods: {string.Join(", ", missingMethods)}");
            Assert.IsEmpty(extraMethods, $"Classification contains methods that are no longer public IPapiClient methods: {string.Join(", ", extraMethods)}");
            Assert.IsEmpty(missingReasons, $"Methods classified as not live tested must include a reason: {string.Join(", ", missingReasons)}");
        }
    }
}
