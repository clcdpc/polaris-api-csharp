using System.Threading;
using System.Threading.Tasks;
using Clc.Rest;
using Clc.Polaris.Api.Models;

namespace Clc.Polaris.Api
{
    public partial class PapiClient
    {


        public async Task<IRestResponse<RemoteStorageItemsGetResult>> RemoteStorageItemsGetAsync(int branchId, string startDate, string endDate, int maxItems, int listType, int? startItemRecordId = null, CancellationToken cancellationToken = default)
        {
            var url = $"/protected/v1/1033/100/1/{ProtectedToken.Placeholder}/cataloging/remotestorage/items";
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