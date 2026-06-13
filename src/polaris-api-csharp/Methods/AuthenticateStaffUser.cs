using Clc.Polaris.Api.Models;
using Clc.Rest;
using System.Threading;
using System.Threading.Tasks;

namespace Clc.Polaris.Api
{
    public partial class PapiClient
    {
        public async Task<IRestResponse<ProtectedToken>> AuthenticateStaffUserAsync(PolarisUser staffUser, CancellationToken cancellationToken = default)
        {
            var request = PapiRestRequest.Post($"/protected/v1/1033/100/{OrganizationId}/authenticator/staff", body: staffUser);

            return await ExecutePapiAsync<ProtectedToken>(request, cancellationToken).ConfigureAwait(false);
        }


    }
}
