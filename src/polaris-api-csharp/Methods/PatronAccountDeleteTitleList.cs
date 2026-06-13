using Clc.Polaris.Api.Models;
using Clc.Polaris.Api.Validation;
using Clc.Rest;
using System.Threading;
using System.Threading.Tasks;

namespace Clc.Polaris.Api
{
    public partial class PapiClient
    {


        public async Task<IRestResponse<PapiResponseCommon>> PatronAccountDeleteTitleListAsync(string barcode, int listId, string password = "", CancellationToken cancellationToken = default)
        {
            Require.Positive(listId);

            var url = $"/public/v1/1033/100/{OrganizationId}/patron/{EncodeBarcodePathSegment(barcode)}/patronaccountdeletetitlelist";
            var request = PapiRestRequest.Delete(url, password: password);
            request.QueryParameters.Add("list", listId);
            return await ExecutePapiAsync<PapiResponseCommon>(request, cancellationToken).ConfigureAwait(false);
        }
    }
}