using System;
using System.Threading.Tasks;
using System.Xml.Linq;
using Clc.Polaris.Api.Validation;
using Clc.Rest;
using Clc.Polaris.Api.Models;
using System.Net.Http;

namespace Clc.Polaris.Api
{
    public partial class PapiClient
    {


        public async Task<IRestResponse<PatronRenewBlocksResult>> PatronRenewBlocksGetAsync(int patronId, int? branchId = null, CancellationToken cancellationToken = default)
        {
            var token = await GetTokenAsync(cancellationToken).ConfigureAwait(false);
            var url = $"/protected/v1/1033/100/{branchId ?? OrganizationId}/{token.AccessToken}/circulation/patron/{patronId}/renewblocks";
            var request = new PapiRestRequest(url);
            return await ExecutePapiAsync<PatronRenewBlocksResult>(request, cancellationToken).ConfigureAwait(false);
        }
    }
}