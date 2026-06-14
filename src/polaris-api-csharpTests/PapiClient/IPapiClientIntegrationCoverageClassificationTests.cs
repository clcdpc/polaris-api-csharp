using System.Reflection;

namespace Clc.Polaris.Api.Tests
{
    [TestClass]
    [UnitTest]
    public sealed class IPapiClientIntegrationCoverageClassificationTests
    {
        private enum LiveCoverageClassification
        {
            UnitOnly,
            ReadOnlyIntegration,
            ProtectedReadOnlyIntegration,
            MutatingIntegration,
            ProtectedMutatingIntegration,
            NotLiveTestedWithReason,
        }

        private sealed record MethodCoverage(LiveCoverageClassification Classification, string? Reason = null);

        private static readonly Dictionary<string, MethodCoverage> Classifications = new()
        {
            [nameof(IPapiClient.ApiKeyValidateAsync)] = new(LiveCoverageClassification.ReadOnlyIntegration),
            [nameof(IPapiClient.ApiVersionGetAsync)] = new(LiveCoverageClassification.ReadOnlyIntegration),
            [nameof(IPapiClient.AuthenticatePatronAsync)] = new(LiveCoverageClassification.ReadOnlyIntegration),
            [nameof(IPapiClient.AuthenticateStaffUserAsync)] = new(LiveCoverageClassification.ProtectedReadOnlyIntegration),
            [nameof(IPapiClient.BibGetAsync)] = new(LiveCoverageClassification.ReadOnlyIntegration),
            [nameof(IPapiClient.BibSearchAsync)] = new(LiveCoverageClassification.ReadOnlyIntegration),
            [nameof(IPapiClient.CollectionsGetAsync)] = new(LiveCoverageClassification.ReadOnlyIntegration),
            [nameof(IPapiClient.CreatePatronBlocksAsync)] = new(LiveCoverageClassification.ProtectedMutatingIntegration),
            [nameof(IPapiClient.DatesClosedGetAsync)] = new(LiveCoverageClassification.ReadOnlyIntegration),
            [nameof(IPapiClient.HoldingsGetAsync)] = new(LiveCoverageClassification.ReadOnlyIntegration),
            [nameof(IPapiClient.HoldRequestCancelAsync)] = new(LiveCoverageClassification.MutatingIntegration),
            [nameof(IPapiClient.HoldRequestCreateAsync)] = new(LiveCoverageClassification.MutatingIntegration),
            [nameof(IPapiClient.HoldRequestGetListAsync)] = new(LiveCoverageClassification.ProtectedReadOnlyIntegration),
            [nameof(IPapiClient.HoldRequestReactivateAsync)] = new(LiveCoverageClassification.MutatingIntegration),
            [nameof(IPapiClient.HoldRequestReplyAsync)] = new(LiveCoverageClassification.MutatingIntegration),
            [nameof(IPapiClient.HoldRequestSuspendAsync)] = new(LiveCoverageClassification.MutatingIntegration),
            [nameof(IPapiClient.ItemCheckInPostAsync)] = new(LiveCoverageClassification.ProtectedMutatingIntegration),
            [nameof(IPapiClient.ItemCheckOutPostAsync)] = new(LiveCoverageClassification.MutatingIntegration),
            [nameof(IPapiClient.ItemRenewAsync)] = new(LiveCoverageClassification.MutatingIntegration),
            [nameof(IPapiClient.ItemStatusesGetAsync)] = new(LiveCoverageClassification.ReadOnlyIntegration),
            [nameof(IPapiClient.ItemUpdateBarcodeAsync)] = new(LiveCoverageClassification.MutatingIntegration),
            [nameof(IPapiClient.LimitFiltersGetAsync)] = new(LiveCoverageClassification.ReadOnlyIntegration),
            [nameof(IPapiClient.MARCTypeOfMaterialsGetAsync)] = new(LiveCoverageClassification.ReadOnlyIntegration),
            [nameof(IPapiClient.MaterialTypesGetAsync)] = new(LiveCoverageClassification.ReadOnlyIntegration),
            [nameof(IPapiClient.NotificationUpdateAsync)] = new(LiveCoverageClassification.ProtectedMutatingIntegration),
            [nameof(IPapiClient.NotificationQueueGetAsync)] = new(LiveCoverageClassification.UnitOnly, "Request-shape coverage only; live queue contents depend on environment-specific notification setup."),
            [nameof(IPapiClient.OrganizationsGetAsync)] = new(LiveCoverageClassification.ReadOnlyIntegration),
            [nameof(IPapiClient.PatronAccountCreateCreditAsync)] = new(LiveCoverageClassification.ProtectedMutatingIntegration),
            [nameof(IPapiClient.PatronAccountCreateTitleListAsync)] = new(LiveCoverageClassification.MutatingIntegration),
            [nameof(IPapiClient.PatronAccountDeleteTitleListAsync)] = new(LiveCoverageClassification.MutatingIntegration),
            [nameof(IPapiClient.PatronAccountDepositCreditAsync)] = new(LiveCoverageClassification.ProtectedMutatingIntegration),
            [nameof(IPapiClient.PatronAccountGetAsync)] = new(LiveCoverageClassification.ReadOnlyIntegration),
            [nameof(IPapiClient.PatronAccountGetTitleListsAsync)] = new(LiveCoverageClassification.ReadOnlyIntegration),
            [nameof(IPapiClient.PatronAccountPayAsync)] = new(LiveCoverageClassification.ProtectedMutatingIntegration),
            [nameof(IPapiClient.PatronAccountPayAllAsync)] = new(LiveCoverageClassification.ProtectedMutatingIntegration),
            [nameof(IPapiClient.PatronAccountRefundCreditAsync)] = new(LiveCoverageClassification.ProtectedMutatingIntegration),
            [nameof(IPapiClient.PatronAccountVoidAsync)] = new(LiveCoverageClassification.ProtectedMutatingIntegration),
            [nameof(IPapiClient.PatronBasicDataGetAsync)] = new(LiveCoverageClassification.ReadOnlyIntegration),
            [nameof(IPapiClient.PatronCirculateBlocksGetAsync)] = new(LiveCoverageClassification.ReadOnlyIntegration),
            [nameof(IPapiClient.PatronCodesGetAsync)] = new(LiveCoverageClassification.ReadOnlyIntegration),
            [nameof(IPapiClient.PatronHoldRequestsGetAsync)] = new(LiveCoverageClassification.ReadOnlyIntegration),
            [nameof(IPapiClient.PatronILLRequestsGetAsync)] = new(LiveCoverageClassification.ReadOnlyIntegration),
            [nameof(IPapiClient.PatronItemsOutGetAsync)] = new(LiveCoverageClassification.ReadOnlyIntegration),
            [nameof(IPapiClient.PatronMessageDeleteAsync)] = new(LiveCoverageClassification.MutatingIntegration),
            [nameof(IPapiClient.PatronMessagesGetAsync)] = new(LiveCoverageClassification.ReadOnlyIntegration),
            [nameof(IPapiClient.PatronMessageUpdateStatusAsync)] = new(LiveCoverageClassification.MutatingIntegration),
            [nameof(IPapiClient.PatronPreferencesGetAsync)] = new(LiveCoverageClassification.ReadOnlyIntegration),
            [nameof(IPapiClient.PatronReadingHistoryClearAsync)] = new(LiveCoverageClassification.MutatingIntegration),
            [nameof(IPapiClient.PatronReadingHistoryGetAsync)] = new(LiveCoverageClassification.ReadOnlyIntegration),
            [nameof(IPapiClient.PatronRegistrationCreateAsync)] = new(LiveCoverageClassification.NotLiveTestedWithReason, "Creates patrons and requires environment-specific registration policy data; covered by validation/request-shape tests."),
            [nameof(IPapiClient.PatronRegistrationCreateV2Async)] = new(LiveCoverageClassification.NotLiveTestedWithReason, "Creates patrons and requires environment-specific registration policy data; covered by validation/request-shape tests."),
            [nameof(IPapiClient.PatronRenewBlocksGetAsync)] = new(LiveCoverageClassification.ProtectedReadOnlyIntegration),
            [nameof(IPapiClient.PatronSavedSearchesGetAsync)] = new(LiveCoverageClassification.ReadOnlyIntegration),
            [nameof(IPapiClient.PatronSearchAsync)] = new(LiveCoverageClassification.ProtectedReadOnlyIntegration),
            [nameof(IPapiClient.PatronTitleListAddTitleAsync)] = new(LiveCoverageClassification.MutatingIntegration),
            [nameof(IPapiClient.PatronTitleListCopyAllTitlesAsync)] = new(LiveCoverageClassification.MutatingIntegration),
            [nameof(IPapiClient.PatronTitleListCopyTitleAsync)] = new(LiveCoverageClassification.MutatingIntegration),
            [nameof(IPapiClient.PatronTitleListDeleteAllTitlesAsync)] = new(LiveCoverageClassification.MutatingIntegration),
            [nameof(IPapiClient.PatronTitleListDeleteTitleAsync)] = new(LiveCoverageClassification.MutatingIntegration),
            [nameof(IPapiClient.PatronTitleListGetTitlesAsync)] = new(LiveCoverageClassification.ReadOnlyIntegration),
            [nameof(IPapiClient.PatronTitleListMoveTitleAsync)] = new(LiveCoverageClassification.MutatingIntegration),
            [nameof(IPapiClient.PatronUpdateAsync)] = new(LiveCoverageClassification.MutatingIntegration),
            [nameof(IPapiClient.PatronUpdateUserNameAsync)] = new(LiveCoverageClassification.MutatingIntegration),
            [nameof(IPapiClient.PatronValidateAsync)] = new(LiveCoverageClassification.ReadOnlyIntegration),
            [nameof(IPapiClient.Patron_GetBarcodeFromIdAsync)] = new(LiveCoverageClassification.ProtectedReadOnlyIntegration),
            [nameof(IPapiClient.PickupBranchesGetAsync)] = new(LiveCoverageClassification.ReadOnlyIntegration),
            [nameof(IPapiClient.RecordSetContentPutAsync)] = new(LiveCoverageClassification.ProtectedMutatingIntegration),
            [nameof(IPapiClient.RecordSetRecordsGetAsync)] = new(LiveCoverageClassification.ProtectedReadOnlyIntegration),
            [nameof(IPapiClient.RemoteStorageItemsGetAsync)] = new(LiveCoverageClassification.NotLiveTestedWithReason, "Requires remote-storage configuration and date windows that are not generally available in disposable environments."),
            [nameof(IPapiClient.SA_GetValueByOrgAsync)] = new(LiveCoverageClassification.ProtectedReadOnlyIntegration),
            [nameof(IPapiClient.ShelfLocationsGetAsync)] = new(LiveCoverageClassification.ReadOnlyIntegration),
            [nameof(IPapiClient.Synch_BibsByIdGetAsync)] = new(LiveCoverageClassification.ProtectedReadOnlyIntegration),
            [nameof(IPapiClient.UpdatePickupBranchIDAsync)] = new(LiveCoverageClassification.NotLiveTestedWithReason, "Requires a known active hold that is safe to retarget; hold lifecycle tests cover related hold mutation paths."),
            [nameof(IPapiClient.UpdatePatronNotesDataAsync)] = new(LiveCoverageClassification.MutatingIntegration),
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
