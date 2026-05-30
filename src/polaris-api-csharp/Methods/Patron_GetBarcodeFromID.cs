using Clc.Rest;
using Clc.Polaris.Api.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
namespace Clc.Polaris.Api
{
    public partial class PapiClient
    {


        public async Task<IRestResponse<GetBarcodeAndPatronIDResult>> Patron_GetBarcodeFromIdAsync(int patronId, CancellationToken cancellationToken = default)
        {
            var url = $"/protected/v1/1033/100/1/{ProtectedToken.Placeholder}/patron/barcode";
            var request = PapiRestRequest.Get(url);
            request.QueryParameters.Add("patronid", patronId);
            return await ExecutePapiAsync<GetBarcodeAndPatronIDResult>(request, cancellationToken).ConfigureAwait(false);
        }
    }
}