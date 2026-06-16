using System.Threading;
using System.Threading.Tasks;
using Clc.Polaris.Api.Models;
using Clc.Polaris.Api.Validation;
using Clc.Rest;

namespace Clc.Polaris.Api
{
    public partial class PapiClient
    {
        public async Task<IRestResponse<ILLRequestCancelResult>> ILLRequestCancelAsync(string barcode, int illRequestId, string password = "", int? userId = null, int? workstationId = null, CancellationToken cancellationToken = default)
        {
            Require.Argument(barcode);
            Require.NonNegative(illRequestId);
            Require.PositiveIfProvided(userId);
            Require.PositiveIfProvided(workstationId);

            var url = $"/public/v1/1033/100/{OrganizationId}/patron/{EncodeBarcodePathSegment(barcode)}/illrequests/{illRequestId}/cancelled";
            var request = PapiRestRequest.Put(url, password: password);
            request.QueryParameters.Add("wsid", workstationId ?? WorkstationId);
            request.QueryParameters.Add("userid", userId ?? UserId);
            return await ExecutePapiAsync<ILLRequestCancelResult>(request, cancellationToken).ConfigureAwait(false);
        }
    }
}
