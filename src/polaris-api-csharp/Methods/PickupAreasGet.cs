using Clc.Rest;
using Clc.Polaris.Api.Models;
using System.Net.Http;

namespace Clc.Polaris.Api
{
    public partial class PapiClient
    {
        public IRestResponse<PickupAreasGetResult> PickupAreasGet(int? orgId = null)
        {
            var url = $"/public/v1/1033/100/{orgId ?? OrganizationId}/pickupareas";
            var request = new PapiRestRequest(HttpMethod.Get, url);
            return Execute<PickupAreasGetResult>(request);
        }
    }
}
