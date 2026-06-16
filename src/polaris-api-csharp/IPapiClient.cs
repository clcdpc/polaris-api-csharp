using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Clc.Polaris.Api.Models;
using Clc.Rest;

namespace Clc.Polaris.Api
{
    public interface IPapiClient
    {
        string AccessID { get; set; }

        /// <summary>
        /// Your PAPI Access Key
        /// </summary>
        string AccessKey { get; set; }

        /// <summary>
        /// The base URL of your PAPI service
        /// </summary>
        string Hostname { get; set; }

        int UserId { get; set; }
        int WorkstationId { get; set; }
        int OrganizationId { get; set; }

        bool AllowStaffOverrideRequests { get; set; }

        /// <summary>
        /// The staff credentials used for protected methods and public method overrides
        /// </summary>
        PolarisUser? StaffOverrideAccount { get; set; }

        Task<IRestResponse<PapiResponseCommon>> ApiKeyValidateAsync(CancellationToken cancellationToken = default);
        Task<IRestResponse<ApiResult>> ApiVersionGetAsync(CancellationToken cancellationToken = default);
        Task<IRestResponse<PatronAuthenticationResult>> AuthenticatePatronAsync(string barcode, string password, CancellationToken cancellationToken = default);
        Task<IRestResponse<ProtectedToken>> AuthenticateStaffUserAsync(PolarisUser staffUser, CancellationToken cancellationToken = default);
        Task<IRestResponse<BibGetResult>> BibGetAsync(int bibId, int? branchId = null, CancellationToken cancellationToken = default);
        Task<IRestResponse<BibGetByTypeResult>> BibGetByTypeV2Async(string key, BibGetByTypeKeyType type = BibGetByTypeKeyType.Barcode, int? branchId = null, CancellationToken cancellationToken = default);
        Task<IRestResponse<BibSearchResult>> BibSearchAsync(BibSearchOptions options, CancellationToken cancellationToken = default);
        Task<IRestResponse<CollectionsGetResult>> CollectionsGetAsync(int? branchId = null, CancellationToken cancellationToken = default);
        Task<IRestResponse<CreatePatronBlocksResult>> CreatePatronBlocksAsync(string barcode, BlockType blockType, string blockValue, int? userId = null, int? workstationId = null, CancellationToken cancellationToken = default);
        Task<IRestResponse<DatesClosedGetResult>> DatesClosedGetAsync(int organizationId, CancellationToken cancellationToken = default);
        Task<IRestResponse<BibHoldingsGetResult>> HoldingsGetAsync(int bibId, CancellationToken cancellationToken = default);
        Task<IRestResponse<HeadingsSearchResult>> HeadingsSearchAsync(HeadingSearchQualifier qualifierName, int numberOfTerms, int preferredPosition, string? startPoint = null, int? noTransaction = null, int? branchId = null, CancellationToken cancellationToken = default);
        Task<IRestResponse<HoldRequestCancelResult>> HoldRequestCancelAsync(string barcode, int requestId, string password = "", int? userId = null, int? workstationId = null, CancellationToken cancellationToken = default);
        Task<IRestResponse<HoldRequestCreateResult>> HoldRequestCreateAsync(HoldRequestCreateParams holdParams, CancellationToken cancellationToken = default);
        Task<IRestResponse<HoldRequestGetListResult>> HoldRequestGetListAsync(int branchId, RequestListBranchType branchType = RequestListBranchType.PickupBranch, HoldStatus status = HoldStatus.Held, CancellationToken cancellationToken = default);
        Task<IRestResponse<HoldRequestActivationResult>> HoldRequestReactivateAsync(string barcode, string password, int requestId, DateTime activationDate, int? userId = null, CancellationToken cancellationToken = default);
        Task<IRestResponse<HoldRequestReplyResult>> HoldRequestReplyAsync(HoldRequestCreateResult holdCreateResult, int requestingOrgId, HoldRequestReplyAnswer answer, HoldRequestReplyState state, CancellationToken cancellationToken = default);
        Task<IRestResponse<HoldRequestActivationResult>> HoldRequestSuspendAsync(string barcode, int requestId, DateTime activationDate, string password = "", int? userId = null, CancellationToken cancellationToken = default);
        Task<IRestResponse<ItemCheckInResult>> ItemCheckInPostAsync(string itemBarcode, int? logonBranchId = null, int? logonUserId = null, int? logonWorkstationId = null, CancellationToken cancellationToken = default);
        Task<IRestResponse<ItemCheckOutResult>> ItemCheckOutPostAsync(string patronBarcode, string itemBarcode, string password = "", int? logonBranchId = null, int? logonUserId = null, int? logonWorkstationId = null, CancellationToken cancellationToken = default);
        Task<IRestResponse<ILLRequestCancelResult>> ILLRequestCancelAsync(string barcode, int illRequestId, string password = "", int? userId = null, int? workstationId = null, CancellationToken cancellationToken = default);
        Task<IRestResponse<ItemRenewResultWrapper>> ItemRenewAsync(string barcode, int itemId, string password = "", ItemRenewOptions? renewOptions = null, CancellationToken cancellationToken = default);
        Task<IRestResponse<ItemStatusesGetResult>> ItemStatusesGetAsync(int? branchId = null, CancellationToken cancellationToken = default);
        Task<IRestResponse<PAPIResult>> ItemUpdateBarcodeAsync(string newBarcode, int? itemRecordId = null, int? transactionBranchId = null, string oldBarcode = "", CancellationToken cancellationToken = default);
        Task<IRestResponse<LimitFiltersGetResult>> LimitFiltersGetAsync(int? branchId = null, CancellationToken cancellationToken = default);
        Task<IRestResponse<MARCTypeOfMaterialsGetResult>> MARCTypeOfMaterialsGetAsync(int? branchId = null, CancellationToken cancellationToken = default);
        Task<IRestResponse<MaterialTypesGetResult>> MaterialTypesGetAsync(int? branchId = null, CancellationToken cancellationToken = default);
        Task<IRestResponse<MultipartGetResult>> MultipartGetAsync(int bibId, int patronId, int? pickupLocationId = null, int? branchId = null, CancellationToken cancellationToken = default);
        Task<IRestResponse<NotificationUpdateResult>> NotificationUpdateAsync(NotificationUpdateParams updateParams, CancellationToken cancellationToken = default);
        Task<IRestResponse<PapiResponseCommon>> NotificationQueueGetAsync(int orgId = 1, CancellationToken cancellationToken = default);
        Task<IRestResponse<OrganizationsGetResult>> OrganizationsGetAsync(OrganizationType type = OrganizationType.All, CancellationToken cancellationToken = default);
        Task<IRestResponse<PatronAccountCreateCreditResult>> PatronAccountCreateCreditAsync(string barcode, double txnAmount, PaymentMethod paymentMethod, int? workstationId = null, int? userId = null, string note = "", CancellationToken cancellationToken = default);
        Task<IRestResponse<PatronAccountCreateTitleListResult>> PatronAccountCreateTitleListAsync(string barcode, string listName, string password = "", CancellationToken cancellationToken = default);
        Task<IRestResponse<PatronAccountDeleteTitleListResult>> PatronAccountDeleteTitleListAsync(string barcode, int listId, string password = "", CancellationToken cancellationToken = default);
        Task<IRestResponse<PatronAccountDepositCreditResult>> PatronAccountDepositCreditAsync(string barcode, double txnAmount, int? workstationId = null, int? userId = null, string note = "", CancellationToken cancellationToken = default);
        Task<IRestResponse<PatronAccountGetResult>> PatronAccountGetAsync(string barcode, string password = "", CancellationToken cancellationToken = default);
        Task<IRestResponse<PatronAccountGetTitleListsResult>> PatronAccountGetTitleListsAsync(string barcode, string password = "", CancellationToken cancellationToken = default);
        Task<IRestResponse<PatronAccountPayResult>> PatronAccountPayAsync(string barcode, int txnId, double txnAmount, PaymentMethod paymentMethod, int? workstationId = null, int? userId = null, string note = "", CancellationToken cancellationToken = default);
        Task<IRestResponse<PatronAccountPayResult>> PatronAccountPayAllAsync(string barcode, double txnAmount, PaymentMethod paymentMethod, int? workstationId = null, int? userId = null, string note = "", CancellationToken cancellationToken = default);
        Task<IRestResponse<PatronAccountRefundCreditResult>> PatronAccountRefundCreditAsync(string barcode, double txnAmount, int? workstationId = null, int? userId = null, string note = "", CancellationToken cancellationToken = default);
        Task<IRestResponse<PatronAccountVoidPaymentResult>> PatronAccountVoidAsync(string barcode, int paymentTxnId, int? workstationId = null, int? userId = null, string note = "", CancellationToken cancellationToken = default);
        Task<IRestResponse<PatronBasicDataGetResult>> PatronBasicDataGetAsync(string barcode, string password = "", bool addresses = false, bool notes = false, CancellationToken cancellationToken = default);
        Task<IRestResponse<PatronCirculateBlocksResult>> PatronCirculateBlocksGetAsync(string barcode, string password = "", CancellationToken cancellationToken = default);
        Task<IRestResponse<PatronCodesGetResult>> PatronCodesGetAsync(int? branchId = null, CancellationToken cancellationToken = default);
        Task<IRestResponse<PatronNotesResult>> PatronNotesGetAsync(string barcode, string password = "", CancellationToken cancellationToken = default);
        Task<IRestResponse<PatronHoldRequestsGetResult>> PatronHoldRequestsGetAsync(string barcode, PatronHoldStatus status = PatronHoldStatus.all, string password = "", CancellationToken cancellationToken = default);
        Task<IRestResponse<PatronILLRequestsGetResult>> PatronILLRequestsGetAsync(string barcode, ILLStatus status = ILLStatus.All, string password = "", CancellationToken cancellationToken = default);
        Task<IRestResponse<PatronItemsOutGetResult>> PatronItemsOutGetAsync(string barcode, PatronItemsOutGetStatus status = PatronItemsOutGetStatus.All, string password = "", CancellationToken cancellationToken = default);
        Task<IRestResponse<PAPIResult>> PatronMessageDeleteAsync(string barcode, PatronMessageType messageType, int messageId, string password = "", CancellationToken cancellationToken = default);
        Task<IRestResponse<PatronMessagesGetResult>> PatronMessagesGetAsync(string barcode, bool unreadOnly = false, string password = "", CancellationToken cancellationToken = default);
        Task<IRestResponse<PAPIResult>> PatronMessageUpdateStatusAsync(string barcode, PatronMessageType messageType, int messageId, string password = "", CancellationToken cancellationToken = default);
        Task<IRestResponse<PatronLanguagesGetResult>> PatronLanguagesGetAsync(int? organizationId = null, CancellationToken cancellationToken = default);
        Task<IRestResponse<PatronPreferencesGetResult>> PatronPreferencesGetAsync(string barcode, string password = "", CancellationToken cancellationToken = default);
        Task<IRestResponse<PatronReadingHistoryClearResult>> PatronReadingHistoryClearAsync(string barcode, string password = "", IEnumerable<int>? ids = null, CancellationToken cancellationToken = default);
        Task<IRestResponse<PatronReadingHistoryGetResult>> PatronReadingHistoryGetAsync(string barcode, int page = 1, int rowsPerPage = 50, string password = "", CancellationToken cancellationToken = default);
        Task<IRestResponse<PatronRegistrationCreateResult>> PatronRegistrationCreateAsync(PatronRegistrationParams _params, CancellationToken cancellationToken = default);
        Task<IRestResponse<PatronRegistrationCreateResult>> PatronRegistrationCreateV2Async(PatronRegistrationData _params, CancellationToken cancellationToken = default);
        Task<IRestResponse<PatronRegistrationUpdateResult>> PatronRegistrationUpdateV2Async(string barcode, PatronRegistrationData registrationData, string password = "", bool ignoresa = true, CancellationToken cancellationToken = default);
        Task<IRestResponse<PatronRenewBlocksResult>> PatronRenewBlocksGetAsync(int patronId, int? branchId = null, CancellationToken cancellationToken = default);
        Task<IRestResponse<PatronSavedSearchesGetResult>> PatronSavedSearchesGetAsync(string barcode, string password = "", CancellationToken cancellationToken = default);
        Task<IRestResponse<PatronSearchResult>> PatronSearchAsync(string query, int page = 1, int pageSize = 10, PatronSortKeys sortBy = PatronSortKeys.PATN, int? orgId = null, CancellationToken cancellationToken = default);
        Task<IRestResponse<PatronStatisticalClassesGetResult>> PatronStatisticalClassesGetAsync(int? organizationId = null, CancellationToken cancellationToken = default);
        Task<IRestResponse<PatronTitleListAddTitleResult>> PatronTitleListAddTitleAsync(string barcode, int recordStoreId, int localControlNumber, string password = "", CancellationToken cancellationToken = default);
        Task<IRestResponse<PatronTitleListCopyAllTitlesResult>> PatronTitleListCopyAllTitlesAsync(string barcode, int fromRecordStoreId, int toRecordStoreId, string password = "", CancellationToken cancellationToken = default);
        Task<IRestResponse<PatronTitleListCopyTitleResult>> PatronTitleListCopyTitleAsync(string barcode, int fromRecordStoreId, int fromPosition, int toRecordStoreId, string password = "", CancellationToken cancellationToken = default);
        Task<IRestResponse<PatronTitleListDeleteAllTitlesResult>> PatronTitleListDeleteAllTitlesAsync(string barcode, int listId, string password = "", CancellationToken cancellationToken = default);
        Task<IRestResponse<PatronTitleListDeleteTitleResult>> PatronTitleListDeleteTitleAsync(string barcode, int listId, int position, string password = "", CancellationToken cancellationToken = default);
        Task<IRestResponse<PatronTitleListGetTitlesResult>> PatronTitleListGetTitlesAsync(string barcode, int listId, int startPosition = 1, int endPosition = 100, string password = "", CancellationToken cancellationToken = default);
        Task<IRestResponse<PatronTitleListMoveTitleResult>> PatronTitleListMoveTitleAsync(string barcode, int fromRecordStoreId, int fromPosition, int toRecordStoreId, string password = "", CancellationToken cancellationToken = default);
        Task<IRestResponse<PatronUpdateResult>> PatronUpdateAsync(string barcode, PatronUpdateParams updateParams, string password = "", bool ignoresa = true, CancellationToken cancellationToken = default);
        Task<IRestResponse<PatronUpdateResult>> PatronUpdateUserNameAsync(string barcode, string newUsername, string password = "", CancellationToken cancellationToken = default);
        Task<IRestResponse<PatronUdfConfigsGetResult>> PatronUdfConfigsGetAsync(int? organizationId = null, CancellationToken cancellationToken = default);
        Task<IRestResponse<PatronValidateResult>> PatronValidateAsync(string barcode, string password = "", CancellationToken cancellationToken = default);
        Task<IRestResponse<GetBarcodeAndPatronIDResult>> Patron_GetBarcodeFromIdAsync(int patronId, CancellationToken cancellationToken = default);
        Task<IRestResponse<PickupAreasGetResult>> PickupAreasGetAsync(int? organizationId = null, CancellationToken cancellationToken = default);
        Task<IRestResponse<PickupBranchesGetResult>> PickupBranchesGetAsync(int? organizationId = null, CancellationToken cancellationToken = default);
        Task<IRestResponse<PAPIResult>> RecordSetContentPutAsync(int recordSetId, IEnumerable<int> records, RecordSetContentPutActions action, int? userId = null, int? workstationId = null, CancellationToken cancellationToken = default);
        Task<IRestResponse<RecordSetRecordsGetResult>> RecordSetRecordsGetAsync(int recordSetId, int? userId = null, int? workstationId = null, int startIndex = 0, int numRecords = 1000, CancellationToken cancellationToken = default);
        Task<IRestResponse<PAPIResult>> RequestsUpdateStatusAsync(int requestId, RequestStatusAction action, int? itemId = null, int? denyReason = null, int? organizationId = null, CancellationToken cancellationToken = default);
        Task<IRestResponse<RemoteStorageItemsGetResult>> RemoteStorageItemsGetAsync(int branchId, string startDate, string endDate, int maxItems, int listType, int? startItemRecordId = null, CancellationToken cancellationToken = default);
        Task<IRestResponse<SAMobilePhoneCarriersGetResult>> SAMobilePhoneCarriersGetAsync(int? organizationId = null, CancellationToken cancellationToken = default);
        Task<IRestResponse<StringResult>> SA_GetValueByOrgAsync(string attribute, int? organizationId = null, CancellationToken cancellationToken = default);
        Task<IRestResponse<SortOptionsGetResult>> SortOptionsGetAsync(int? branchId = null, CancellationToken cancellationToken = default);
        Task<IRestResponse<ShelfLocationsGetResult>> ShelfLocationsGetAsync(int? branchId = null, CancellationToken cancellationToken = default);
        Task<IRestResponse<SysHoldStatusesGetResult>> SysHoldStatusesGetAsync(int? branchId = null, CancellationToken cancellationToken = default);
        Task<IRestResponse<Sync_BibsByIdGetResult>> Synch_BibsByIdGetAsync(int[] bibIds, bool includeItems = false, CancellationToken cancellationToken = default);
        Task<IRestResponse<PAPIResult>> UpdatePickupBranchIDAsync(string barcode, int requestId, int pickupBranchId, string password = "", int? userId = null, int? workstationId = null, CancellationToken cancellationToken = default);
        Task<IRestResponse<UpdatePatronNotesResult>> UpdatePatronNotesDataAsync(string barcode, string? nonBlockingNote = null, string? blockingNote = null, UpdateNoteMode updateMode = UpdateNoteMode.Prepend, int? workstationId = null, CancellationToken cancellationToken = default);
    }
}
