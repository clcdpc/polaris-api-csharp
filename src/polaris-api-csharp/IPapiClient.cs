using Clc.Polaris.Api.Models;
using Clc.Polaris.Models;
using Clc.Rest;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

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
        PolarisUser StaffOverrideAccount { get; set; }

        IRestResponse<PapiResponseCommon> ApiKeyValidate();
        IRestResponse<ApiResult> ApiVersionGet();
        IRestResponse<PatronAuthenticationResult> AuthenticatePatron(string barcode, string password);
        IRestResponse<ProtectedToken> AuthenticateStaffUser(PolarisUser staffUser);
        IRestResponse<BibGetResult> BibGet(int bibId, int? branchId = null);
        IRestResponse<BibSearchResult> BibSearch(BibSearchOptions options);
        IRestResponse<CollectionsGetResult> CollectionsGet(int? branchId = null);
        IRestResponse<CreatePatronBlocksResult> CreatePatronBlocks(string barcode, BlockType blockType, string blockValue, int? userId = null, int? workstationId = null);
        IRestResponse<DatesClosedGetResult> DatesClosedGet(int organizationId);
        IRestResponse<BibHoldingsGetResult> HeadingsSearch(int bibId);
        IRestResponse<BibHoldingsGetResult> HoldingsGet(int bibId);
        IRestResponse<HoldRequestCancelResult> HoldRequestCancel(string barcode, int requestId, string password = "", int? userId = null, int? workstationId = null);
        IRestResponse<HoldRequestCreateResult> HoldRequestCreate(HoldRequestCreateParams holdParams);
        IRestResponse<HoldRequestGetListResult> HoldRequestGetList(int branchId, RequestListBranchType branchType = RequestListBranchType.PickupBranch, HoldStatus status = HoldStatus.Held);
        IRestResponse<HoldRequestActivationResult> HoldRequestReactivate(string barcode, string password, int requestId, DateTime activationDate, int? userId = null);
        IRestResponse<HoldRequestReplyResult> HoldRequestReply(HoldRequestCreateResult holdCreateResult, int requestingOrgId, HoldRequestReplyAnswer answer, HoldRequestReplyState state);
        IRestResponse<HoldRequestActivationResult> HoldRequestSuspend(string barcode, int requestId, DateTime activationDate, string password = "", int? userId = null);
        IRestResponse<ItemRenewResultWrapper> ItemRenew(string barcode, int itemId, string password = "", ItemRenewOptions renewOptions = null);
        IRestResponse<ItemRenewResultWrapper> ItemRenewAllForPatron(string barcode, string password = "", ItemRenewOptions renewOptions = null);
        IRestResponse<ItemStatusesGetResult> ItemStatusesGet(int? branchId = null);
        IRestResponse<PapiResponseCommon> ItemUpdateBarcode(string newBarcode, int? itemRecordId = null, int? transactionBranchId = null, string oldBarcode = "");
        IRestResponse<LimitFiltersGetResult> LimitFiltersGet(int? branchId = null);
        IRestResponse<MARCTypeOfMaterialsGetResult> MARCTypeOfMaterialsGet(int? branchId = null);
        IRestResponse<MaterialTypesGetResult> MaterialTypesGet(int? branchId = null);
        IRestResponse<NotificationUpdateResult> NotificationUpdate(NotificationUpdateParams updateParams);
        IRestResponse<OrganizationsGetResult> OrganizationsGet(OrganizationType type = OrganizationType.All);
        IRestResponse<PapiResponseCommon> PatronAccountCreateCredit(string barcode, double txnAmount, PaymentMethod paymentMethod, int? workstationId = null, int? userId = null, string note = "");
        IRestResponse<PapiResponseCommon> PatronAccountCreateTitleList(string barcode, string listName, string password = "");
        IRestResponse<PapiResponseCommon> PatronAccountDeleteTitleList(string barcode, int listId, string password = "");
        IRestResponse<PapiResponseCommon> PatronAccountDepositCredit(string barcode, double txnAmount, int? workstationId = null, int? userId = null, string note = "");
        IRestResponse<PatronAccountGetResult> PatronAccountGet(string barcode, string password = "");
        IRestResponse<PatronAccountGetTitleListsResult> PatronAccountGetTitleLists(string barcode, string password = "");
        IRestResponse<PapiResponseCommon> PatronAccountPay(string barcode, int txnId, double txnAmount, PaymentMethod paymentMethod, int? workstationId = null, int? userId = null, string note = "");
        IRestResponse<PapiResponseCommon> PatronAccountPayAll(string barcode, double txnAmount, PaymentMethod paymentMethod, int? workstationId = 1, int? userId = 1, string note = "");
        IRestResponse<PapiResponseCommon> PatronAccountRefundCredit(string barcode, double txnAmount, int? workstationId = null, int? userId = null, string note = "");
        IRestResponse<PapiResponseCommon> PatronAccountVoid(string barcode, int paymentTxnId, int? workstationId = null, int? userId = null, string note = "");
        IRestResponse<PatronBasicDataGetResult> PatronBasicDataGet(string barcode, string password = "", bool addresses = false);
        IRestResponse<PatronCirculateBlocksResult> PatronCirculateBlocksGet(string barcode, string password = "");
        IRestResponse<PatronCodesGetResult> PatronCodesGet(int? branchId = null);
        IRestResponse<PatronHoldRequestsGetResult> PatronHoldRequestsGet(string barcode, PatronHoldStatus status = PatronHoldStatus.all, string password = "");
        IRestResponse<PatronILLRequestsGetResult> PatronILLRequestsGet(string barcode, ILLStatus status = ILLStatus.All, string password = "");
        IRestResponse<PatronItemsOutGetResult> PatronItemsOutGet(string barcode, PatronItemsOutGetStatus status = PatronItemsOutGetStatus.All, string password = "");
        IRestResponse<PapiResponseCommon> PatronMessageDelete(string barcode, PatronMessageType messageType, int messageId, string password = "");
        IRestResponse<PatronMessagesGetResult> PatronMessagesGet(string barcode, bool unreadOnly = false, string password = "");
        IRestResponse<PapiResponseCommon> PatronMessageUpdateStatus(string barcode, PatronMessageType messageType, int messageId, string password = "");
        IRestResponse<PatronPreferencesGetResult> PatronPreferencesGet(string barcode, string password = "");
        IRestResponse<PapiResponseCommon> PatronReadingHistoryClear(string barcode, string password, params int[] ids);
        IRestResponse<PapiResponseCommon> PatronReadingHistoryClear(string barcode, params int[] ids);
        IRestResponse<PatronReadingHistoryGetResult> PatronReadingHistoryGet(string barcode, int page = 1, int rowsPerPage = 50, string password = "");
        IRestResponse<PatronRegistrationCreateResult> PatronRegistrationCreate(PatronRegistrationParams _params);
        IRestResponse<PatronRegistrationCreateResult> PatronRegistrationCreateV2(PatronRegistrationData _params);
        IRestResponse<PatronRenewBlocksResult> PatronRenewBlocksGet(int patronId, int? branchId = null);
        IRestResponse<PatronSavedSearchesGetResult> PatronSavedSearchesGet(string barcode, string password = "");
        IRestResponse<PatronSearchResult> PatronSearch(string query, int page = 1, int pageSize = 10, PatronSortKeys sortBy = PatronSortKeys.PATN, int? orgId = null);
        IRestResponse<PatronTitleListAddTitleResult> PatronTitleListAddTitle(string barcode, int recordStoreId, int localControlNumber, string password = "");
        IRestResponse<PapiResponseCommon> PatronTitleListCopyAllTitles(string barcode, int fromRecordStoreId, int toRecordStoreId, string password = "");
        IRestResponse<PapiResponseCommon> PatronTitleListCopyTitle(string barcode, int fromRecordStoreId, int fromPosition, int toRecordStoreId, string password = "");
        IRestResponse<PapiResponseCommon> PatronTitleListDeleteAllTitles(string barcode, int listId, string password = "");
        IRestResponse<PapiResponseCommon> PatronTitleListDeleteTitle(string barcode, int listId, int position, string password = "");
        IRestResponse<PatronTitleListGetTitlesResult> PatronTitleListGetTitles(string barcode, int listId, int startPosition = 1, int endPosition = 100, string password = "");
        IRestResponse<PapiResponseCommon> PatronTitleListMoveTitle(string barcode, int fromRecordStoreId, int fromPosition, int toRecordStoreId, string password = "");
        IRestResponse<PatronUpdateResult> PatronUpdate(string barcode, PatronUpdateParams updateParams, string password = "", bool ignoresa = true);
        IRestResponse<PapiResponseCommon> PatronUpdateUserName(string barcode, string newUsername, string password = "");
        IRestResponse<PatronValidateResult> PatronValidate(string barcode, string password = "");
        IRestResponse<GetBarcodeAndPatronIDResult> Patron_GetBarcodeFromId(int patronId);
        IRestResponse<PickupBranchesGetResult> PickupBranchesGet(int? organizationId = null);
        IRestResponse<PapiResponseCommon> RecordSetContentPut(int recordSetId, IEnumerable<int> records, RecordSetContentPutActions action, int? userId = null, int? workstationId = null);
        IRestResponse<PapiResponseCommon> RecordSetContentAdd(int recordSetId, int recordId, int userId = 1, int workstationId = 1);
        IRestResponse<PapiResponseCommon> RecordSetContentAdd(int recordSetId, IEnumerable<int> records, int userId = 1, int workstationId = 1);
        IRestResponse<PapiResponseCommon> RecordSetContentRemove(int recordSetId, int recordId, int userId = 1, int workstationId = 1);
        IRestResponse<PapiResponseCommon> RecordSetContentRemove(int recordSetId, IEnumerable<int> records, int userId = 1, int workstationId = 1);
        IRestResponse<RecordSetRecordsGetResult> RecordSetRecordsGet(int recordSetId, int userId = 1, int workstationId = 1, int startIndex = 0, int numRecords = 1000);
        IRestResponse<RemoteStorageItemsGetResult> RemoteStorageItemsGet(int branchId, string startDate, string endDate, int maxItems, int listType, int? startItemRecordId = null);
        IRestResponse<StringResult> SA_GetValueByOrg(string attribute, int? organizationId = null);
        IRestResponse<ShelfLocationsGetResult> ShelfLocationsGet(int? branchId = null);
        IRestResponse<Sync_BibsByIdGetResult> Synch_BibsByIdGet(int[] bibIds, bool includeItems);
        IRestResponse<Sync_BibsByIdGetResult> Synch_BibsByIdGet(int bibId, bool includeItems);
        IRestResponse<PapiResponseCommon> UpdatePickupBranchID(string barcode, int requestId, int pickupBranchId, string password = "", int? userId = null, int? workstationId = null);
        IRestResponse<PapiResponseCommon> UpdatePatronNotesData(string barcode, string nonBlockingNote = null, string blockingNote = null, UpdateNoteMode updateMode = UpdateNoteMode.Prepend, int? workstationId = null);
    }
}
