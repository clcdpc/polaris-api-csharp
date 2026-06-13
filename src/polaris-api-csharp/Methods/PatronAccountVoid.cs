using Clc.Polaris.Api.Models;
using Clc.Polaris.Api.Validation;
using Clc.Rest;
using System.Threading;
using System.Threading.Tasks;

namespace Clc.Polaris.Api
{
    public partial class PapiClient
    {


        public async Task<IRestResponse<PapiResponseCommon>> PatronAccountVoidAsync(string barcode, int paymentTxnId, int? workstationId = null, int? userId = null, string note = "", CancellationToken cancellationToken = default)
        {
            Require.Positive(paymentTxnId);
            Require.PositiveIfProvided(workstationId);
            Require.PositiveIfProvided(userId);

            var url = $"/protected/v1/1033/100/{OrganizationId}/{ProtectedToken.Placeholder}/patron/{EncodeBarcodePathSegment(barcode)}/account/{paymentTxnId}/void/payment";
            var request = PapiRestRequest.Delete(url);
            request.QueryParameters.Add("wsid", workstationId ?? WorkstationId);
            request.QueryParameters.Add("userid", userId ?? UserId);
            return await ExecutePapiAsync<PapiResponseCommon>(request, cancellationToken).ConfigureAwait(false);
        }
    }
}
