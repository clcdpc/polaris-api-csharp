using Clc.Polaris.Api.Models;
using Clc.Polaris.Api.Validation;
using Clc.Rest;
using System.Threading;
using System.Threading.Tasks;

namespace Clc.Polaris.Api
{
    public partial class PapiClient
    {
        public async Task<IRestResponse<BibGetByTypeResult>> BibGetByTypeV2Async(string key, string type = "barcode", int? branchId = null, CancellationToken cancellationToken = default)
        {
            Require.Argument(key);
            Require.Argument(type);
            Require.PositiveIfProvided(branchId);
            var request = PapiRestRequest.Get($"/public/v2/1033/100/{branchId ?? OrganizationId}/bib/{key}");
            request.QueryParameters.Add("type", type);
            return await ExecutePapiAsync<BibGetByTypeResult>(request, cancellationToken).ConfigureAwait(false);
        }
    }
}
