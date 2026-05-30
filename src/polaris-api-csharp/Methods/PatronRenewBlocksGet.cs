using System;
using System.Threading;
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
            await EnsureProtectedTokenAsync(cancellationToken).ConfigureAwait(false);
            var url = $"/protected/v1/1033/100/{branchId ?? OrganizationId}/{Token.AccessToken}/circulation/patron/{patronId}/renewblocks";
            var request = new PapiRestRequest(url);
            return await ExecutePapiAsync<PatronRenewBlocksResult>(request, cancellationToken).ConfigureAwait(false);
        }
    }
}