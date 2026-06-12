
using Clc.Rest;
using Clc.Polaris.Api.Models;
using Clc.Polaris.Api.Validation;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Net;
using System.Xml.Linq;

namespace Clc.Polaris.Api
{
    public partial class PapiClient
    {
        public async Task<IRestResponse<HoldRequestActivationResult>> HoldRequestSuspendAsync(string barcode, int requestId, DateTime activationDate, string password = "", int? userId = null, CancellationToken cancellationToken = default)
        {
            Require.Positive(requestId);
            Require.PositiveIfProvided(userId);

            var url = $"/public/v1/1033/100/1/patron/{EncodeBarcodePathSegment(barcode)}/holdrequests/{requestId}/inactive";
            var json = new { HoldRequestActivationData = new { UserId = userId ?? UserId, activationDate } };
            var request = PapiRestRequest.Put(url, body: json, password: password);
            return await ExecutePapiAsync<HoldRequestActivationResult>(request, cancellationToken).ConfigureAwait(false);
        }
    }
}
