
using Clc.Rest;
using Clc.Polaris.Api.Models;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
namespace Clc.Polaris.Api
{
	public partial class PapiClient
    {


        public async Task<IRestResponse<PatronHoldRequestsGetResult>> PatronHoldRequestsGetAsync(string barcode, PatronHoldStatus status = PatronHoldStatus.all, string password = "", CancellationToken cancellationToken = default)
        {
            var url = $"/public/v1/1033/100/{OrganizationId}/patron/{EncodeBarcodePathSegment(barcode)}/holdrequests/{status}";
            var request = PapiRestRequest.Get(url, password: password);
            return await ExecutePapiAsync<PatronHoldRequestsGetResult>(request, cancellationToken).ConfigureAwait(false);
        }
    }
}