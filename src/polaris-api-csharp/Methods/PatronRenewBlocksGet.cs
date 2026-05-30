using System.Threading;
using System.Threading.Tasks;
using Clc.Rest;
using Clc.Polaris.Api.Models;

namespace Clc.Polaris.Api
{
    public partial class PapiClient
    {


        public async Task<IRestResponse<PatronRenewBlocksResult>> PatronRenewBlocksGetAsync(int patronId, int? branchId = null, CancellationToken cancellationToken = default)
        {
            var url = $"/protected/v1/1033/100/{branchId ?? OrganizationId}/{ProtectedToken.Placeholder}/circulation/patron/{patronId}/renewblocks";
            var request = PapiRestRequest.Get(url);
            return await ExecutePapiAsync<PatronRenewBlocksResult>(request, cancellationToken).ConfigureAwait(false);
        }
    }
}