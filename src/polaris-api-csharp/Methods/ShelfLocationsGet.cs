using Clc.Rest;
using Clc.Polaris.Api.Models;
using System.Threading;
using System.Threading.Tasks;

namespace Clc.Polaris.Api
{
    public partial class PapiClient
    {


        public async Task<IRestResponse<ShelfLocationsGetResult>> ShelfLocationsGetAsync(int? branchId = null, CancellationToken cancellationToken = default)
        {
            var url = $"/public/v1/1033/100/{branchId ?? OrganizationId}/shelflocations";
            var request = PapiRestRequest.Get(url);

            request.BlockStaffOverride = true;
            return await ExecutePapiAsync<ShelfLocationsGetResult>(request, cancellationToken).ConfigureAwait(false);
        }
    }
}
