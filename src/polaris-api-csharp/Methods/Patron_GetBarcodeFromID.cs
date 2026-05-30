using Clc.Rest;
using Clc.Polaris.Api.Models;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Threading;
namespace Clc.Polaris.Api
{
	public partial class PapiClient
    {
        

        public async Task<IRestResponse<GetBarcodeAndPatronIDResult>> Patron_GetBarcodeFromIdAsync(int patronId, CancellationToken cancellationToken = default)
        {
            await EnsureProtectedTokenAsync(cancellationToken).ConfigureAwait(false);
            var url = $"/protected/v1/1033/100/1/{Token.AccessToken}/patron/barcode";
            var request = new PapiRestRequest(url);
            request.QueryParameters.Add("patronid", patronId);
            return await ExecutePapiAsync<GetBarcodeAndPatronIDResult>(request, cancellationToken).ConfigureAwait(false);
        }
    }
}
