
using Clc.Polaris.Api.Models;
using Clc.Polaris.Api.Validation;
using Clc.Rest;
using System.Threading;
using System.Threading.Tasks;

namespace Clc.Polaris.Api
{
    public partial class PapiClient
    {


        public async Task<IRestResponse<PatronTitleListDeleteAllTitlesResult>> PatronTitleListDeleteAllTitlesAsync(string barcode, int listId, string password = "", CancellationToken cancellationToken = default)
        {
            Require.Positive(listId);

            var url = $"/public/v1/1033/100/{OrganizationId}/patron/{EncodeBarcodePathSegment(barcode)}/patrontitlelistdeletealltitles";
            var request = PapiRestRequest.Delete(url, password: password);
            request.QueryParameters.Add("list", listId);
            return await ExecutePapiAsync<PatronTitleListDeleteAllTitlesResult>(request, cancellationToken).ConfigureAwait(false);
        }
    }
}
