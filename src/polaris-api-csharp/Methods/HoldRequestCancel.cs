using Clc.Rest;
using Clc.Polaris.Api.Models;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
namespace Clc.Polaris.Api
{
    public partial class PapiClient
    {
        public async Task<IRestResponse<HoldRequestCancelResult>> HoldRequestCancelAsync(string barcode, int requestId, string password = "", int? userId = null, int? workstationId = null, CancellationToken cancellationToken = default)
        {
            var url = $"/public/v1/1033/100/1/patron/{WebUtility.UrlEncode(barcode)}/holdrequests/{requestId}/cancelled";
            var request = PapiRestRequest.Put(url, password: password);
            request.QueryParameters.Add("wsid", workstationId ?? WorkstationId);
            request.QueryParameters.Add("userid", userId ?? UserId);
            return await ExecutePapiAsync<HoldRequestCancelResult>(request, cancellationToken).ConfigureAwait(false);
        }
    }
}