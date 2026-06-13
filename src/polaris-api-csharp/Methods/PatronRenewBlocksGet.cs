using Clc.Polaris.Api.Models;
using Clc.Polaris.Api.Validation;
using Clc.Rest;
using System.Threading;
using System.Threading.Tasks;

namespace Clc.Polaris.Api
{
    public partial class PapiClient
    {


        public async Task<IRestResponse<PatronRenewBlocksResult>> PatronRenewBlocksGetAsync(int patronId, int? branchId = null, CancellationToken cancellationToken = default)
        {
            Require.Positive(patronId);
            Require.PositiveIfProvided(branchId);

            var url = $"/protected/v1/1033/100/{branchId ?? OrganizationId}/{ProtectedToken.Placeholder}/circulation/patron/{patronId}/renewblocks";
            var request = PapiRestRequest.Get(url);
            return await ExecutePapiAsync<PatronRenewBlocksResult>(request, cancellationToken).ConfigureAwait(false);
        }
    }
}