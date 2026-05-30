
using Clc.Rest;
using Clc.Polaris.Api.Models;
using System;
using System.Threading.Tasks;
using System.Net;
using System.Net.Http;
using System.Xml.Linq;

namespace Clc.Polaris.Api
{
    public partial class PapiClient
    {
        public async Task<IRestResponse<HoldRequestActivationResult>> HoldRequestSuspendAsync(string barcode, int requestId, DateTime activationDate, string password = "", int? userId = null, CancellationToken cancellationToken = default)
        {
            var url = $"/public/v1/1033/100/1/patron/{WebUtility.UrlEncode(barcode)}/holdrequests/{requestId}/inactive";
            var json = new { HoldRequestActivationData = new { UserId = userId ?? UserId, activationDate } };
            var request = new PapiRestRequest(HttpMethod.Put, url) { Password = password, Body = json };
            return await ExecutePapiAsync<HoldRequestActivationResult>(request, cancellationToken).ConfigureAwait(false);
        }
    }
}