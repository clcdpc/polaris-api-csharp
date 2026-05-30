using Clc.Rest;
using Clc.Polaris.Api.Models;
using System.Threading;
using System.Threading.Tasks;

namespace Clc.Polaris.Api
{
    public partial class PapiClient
    {
        public async Task<IRestResponse<HoldRequestGetListResult>> HoldRequestGetListAsync(int branchId, RequestListBranchType branchType = RequestListBranchType.PickupBranch, HoldStatus status = HoldStatus.Held, CancellationToken cancellationToken = default)
        {
            var url = $"/protected/v1/1033/100/1/{ProtectedToken.Placeholder}/circulation/requests/list";
            var request = PapiRestRequest.Get(url);
            request.QueryParameters.Add("branch", branchId);
            request.QueryParameters.Add("branchtype", (int)branchType);
            request.QueryParameters.Add("requeststatus", (int)status);
            return await ExecutePapiAsync<HoldRequestGetListResult>(request, cancellationToken).ConfigureAwait(false);
        }
    }
}
