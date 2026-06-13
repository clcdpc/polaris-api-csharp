using Clc.Polaris.Api.Models;
using Clc.Rest;
using System.Threading;
using System.Threading.Tasks;

namespace Clc.Polaris.Api
{
    public partial class PapiClient
    {
        public async Task<IRestResponse<OrganizationsGetResult>> OrganizationsGetAsync(OrganizationType type = OrganizationType.All, CancellationToken cancellationToken = default)
        {
            var url = $"/public/v1/1033/100/{OrganizationId}/organizations/{type}";
            var request = PapiRestRequest.Get(url);
            request.BlockStaffOverride = true;
            return await ExecutePapiAsync<OrganizationsGetResult>(request, cancellationToken).ConfigureAwait(false);
        }
    }
}
