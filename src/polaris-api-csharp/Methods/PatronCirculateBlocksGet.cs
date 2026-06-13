using Clc.Polaris.Api.Models;
using Clc.Rest;
using System.Threading;
using System.Threading.Tasks;

namespace Clc.Polaris.Api
{
    public partial class PapiClient
    {


        public async Task<IRestResponse<PatronCirculateBlocksResult>> PatronCirculateBlocksGetAsync(string barcode, string password = "", CancellationToken cancellationToken = default)
        {
            var url = $"/public/v1/1033/100/{OrganizationId}/patron/{EncodeBarcodePathSegment(barcode)}/circulationblocks";
            var request = PapiRestRequest.Get(url, password: password);
            return await ExecutePapiAsync<PatronCirculateBlocksResult>(request, cancellationToken).ConfigureAwait(false);
        }
    }
}
