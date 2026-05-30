using Clc.Rest;
using Clc.Polaris.Api.Models;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
namespace Clc.Polaris.Api
{
	public partial class PapiClient
    {


        public async Task<IRestResponse<GetBarcodeAndPatronIDResult>> Patron_GetBarcodeFromIdAsync(int patronId, CancellationToken cancellationToken = default)
        {
            var token = await GetTokenAsync(cancellationToken).ConfigureAwait(false);
            var url = $"/protected/v1/1033/100/1/{token.AccessToken}/patron/barcode";
            var request = new PapiRestRequest(url);
            request.QueryParameters.Add("patronid", patronId);
            return await ExecutePapiAsync<GetBarcodeAndPatronIDResult>(request, cancellationToken).ConfigureAwait(false);
        }
    }
}