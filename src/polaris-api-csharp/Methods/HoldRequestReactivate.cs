using Clc.Rest;
using Clc.Polaris.Api.Models;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Net;

namespace Clc.Polaris.Api
{
	public partial class PapiClient
    {
        public async Task<IRestResponse<HoldRequestActivationResult>> HoldRequestReactivateAsync(string barcode, string password, int requestId, DateTime activationDate, int? userId = null, CancellationToken cancellationToken = default)
        {
            var url = $"/public/v1/1033/100/1/patron/{WebUtility.UrlEncode(barcode)}/holdrequests/{requestId}/active";
            var body = new { HoldRequestActivationData = new { UserId = userId ?? UserId, activationDate } };
            var request = PapiRestRequest.Put(url, body: body, password: password);
            return await ExecutePapiAsync<HoldRequestActivationResult>(request, cancellationToken).ConfigureAwait(false);
        }
    }
}