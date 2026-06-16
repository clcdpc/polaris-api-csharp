using Clc.Polaris.Api.Models;
using Clc.Polaris.Api.Validation;
using Clc.Rest;
using System.Threading;
using System.Threading.Tasks;

namespace Clc.Polaris.Api
{
    public partial class PapiClient
    {


        public async Task<IRestResponse<PAPIResult>> PatronMessageDeleteAsync(string barcode, PatronMessageType messageType, int messageId, string password = "", CancellationToken cancellationToken = default)
        {
            Require.Positive(messageId);

            var url = $"/public/v1/1033/100/{OrganizationId}/patron/{EncodeBarcodePathSegment(barcode)}/messages/{messageType}/{messageId}";
            var request = PapiRestRequest.Delete(url, password: password);
            return await ExecutePapiAsync<PAPIResult>(request, cancellationToken).ConfigureAwait(false);
        }
    }
}