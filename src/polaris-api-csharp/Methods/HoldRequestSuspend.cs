
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
        public IRestResponse<HoldRequestActivationResult> HoldRequestSuspend(string barcode, int requestId, DateTime activationDate, string password = "", int? userId = null)
        {
            var url = $"/public/v1/1033/100/1/patron/{barcode}/holdrequests/{requestId}/inactive";
            var json = new { HoldRequestActivationData = new { UserId = userId ?? UserId, activationDate } };
            var request = new PapiRestRequest(HttpMethod.Put, url) { Password = password, Body = json };
            return Execute<HoldRequestActivationResult>(request);
        }
    }
}