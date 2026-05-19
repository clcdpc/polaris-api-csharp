using Clc.Rest;
using Clc.Polaris.Api.Models;
using System.Net;
using System.Net.Http;

namespace Clc.Polaris.Api
{
    public partial class PapiClient
    {
        public IRestResponse<ItemCheckoutResult> ItemCheckout(string barcode, ItemCheckoutData data)
        {
            var url = $"/public/v1/1033/100/{OrganizationId}/patron/{WebUtility.UrlEncode(barcode)}/itemsout";
            var request = new PapiRestRequest(HttpMethod.Post, url) { Body = data };
            return Execute<ItemCheckoutResult>(request);
        }
    }
}
