using Clc.Polaris.Api.Models;
using Clc.Polaris.Api.Validation;
using Clc.Rest;
using System.Threading;
using System.Threading.Tasks;

namespace Clc.Polaris.Api
{
    public partial class PapiClient
    {


        public async Task<IRestResponse<MaterialTypesGetResult>> MaterialTypesGetAsync(int? branchId = null, CancellationToken cancellationToken = default)
        {
            Require.PositiveIfProvided(branchId);

            var url = $"/public/v1/1033/100/{branchId ?? OrganizationId}/materialtypes";
            var request = PapiRestRequest.Get(url);
            request.BlockStaffOverride = true;
            return await ExecutePapiAsync<MaterialTypesGetResult>(request, cancellationToken).ConfigureAwait(false);
        }
    }
}
