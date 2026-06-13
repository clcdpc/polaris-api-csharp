using Clc.Polaris.Api.Models;
using Clc.Rest;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace Clc.Polaris.Api
{
    public partial class PapiClient
    {


        public async Task<IRestResponse<PapiResponseCommon>> PatronUpdateUserNameAsync(string barcode, string newUsername, string password = "", CancellationToken cancellationToken = default)
        {
            var url = $"/public/v1/1033/100/{OrganizationId}/patron/{EncodeBarcodePathSegment(barcode)}/username/{WebUtility.UrlEncode(newUsername)}";
            var request = PapiRestRequest.Put(url, password: password);
            return await ExecutePapiAsync<PapiResponseCommon>(request, cancellationToken).ConfigureAwait(false);
        }
    }
}