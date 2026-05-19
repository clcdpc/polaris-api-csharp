using Clc.Rest;
using Clc.Polaris.Api.Models;
using System.Net;
using System.Net.Http;

namespace Clc.Polaris.Api
{
    public partial class PapiClient
    {
        public IRestResponse<ItemCheckInResult> ItemCheckin(string barcode, ItemCheckInData data)
        {
            var url = $"/protected/v1/1033/100/1/{Token.AccessToken}/item/{WebUtility.UrlEncode(barcode)}/checkin";
            var request = new PapiRestRequest(HttpMethod.Post, url) { Body = data };
            return Execute<ItemCheckInResult>(request);
        }
    }
}
