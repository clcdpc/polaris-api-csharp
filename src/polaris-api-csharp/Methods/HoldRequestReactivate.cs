using Clc.Rest;
using Clc.Polaris.Api.Models;
using System;
using System.Threading.Tasks;
using System.Net.Http;
using System.Xml.Linq;

namespace Clc.Polaris.Api
{
	public partial class PapiClient
    {
        public IRestResponse<HoldRequestActivationResult> HoldRequestReactivate(string barcode, string password, int requestId, DateTime activationDate, int? userId = null)
        {
            var url = $"/public/v1/1033/100/1/patron/{barcode}/holdrequests/{requestId}/active";
            var body = new { HoldRequestActivationData = new { UserId = userId ?? UserId, activationDate } };
            var request = new PapiRestRequest(HttpMethod.Put, url, password, body);
            return Execute<HoldRequestActivationResult>(request);
        }
    }
}