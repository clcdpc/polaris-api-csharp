
namespace Clc.Polaris.Api
{
    public partial class PapiClient
    {

        public async Task<IRestResponse<RemoteStorageItemsGetResult>> RemoteStorageItemsGetAsync(int branchId, string startDate, string endDate, int maxItems, int listType, int? startItemRecordId = null, CancellationToken cancellationToken = default)
        {
            Require.Positive(branchId);
            Require.Positive(maxItems);
            Require.PositiveIfProvided(startItemRecordId);

            var url = $"/protected/v1/1033/100/{OrganizationId}/{ProtectedToken.Placeholder}/cataloging/remotestorage/items";
            var request = PapiRestRequest.Get(url);
            request.QueryParameters.Add("branch", branchId);
            request.QueryParameters.Add("startdate", startDate);
            request.QueryParameters.Add("enddate", endDate);
            request.QueryParameters.Add("maxitems", maxItems);
            request.QueryParameters.Add("listtype", (int)listType);
            if (startItemRecordId.HasValue) { request.QueryParameters.Add("startitemrecordid", startItemRecordId.Value); }
            return await ExecutePapiAsync<RemoteStorageItemsGetResult>(request, cancellationToken).ConfigureAwait(false);
        }
    }
}