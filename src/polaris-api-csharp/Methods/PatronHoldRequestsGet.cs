
using Clc.Rest;
using Clc.Polaris.Api.Models;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
namespace Clc.Polaris.Api
{
	public partial class PapiClient
    {


        public async Task<IRestResponse<PatronHoldRequestsGetResult>> PatronHoldRequestsGetAsync(string barcode, PatronHoldStatus status = PatronHoldStatus.all, string password = "", CancellationToken cancellationToken = default)
        {
            var url = $"/public/v1/1033/100/1/patron/{WebUtility.UrlEncode(barcode)}/holdrequests/{status}";
            var request = new PapiRestRequest(url) { Password = password };
            return await ExecutePapiAsync<PatronHoldRequestsGetResult>(request, cancellationToken).ConfigureAwait(false);
        }
    }
}