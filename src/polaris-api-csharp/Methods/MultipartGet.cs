using Clc.Rest;
using Clc.Polaris.Api.Models;
using System.Net.Http;

namespace Clc.Polaris.Api
{
    public partial class PapiClient
    {
        public IRestResponse<MultipartGetResult> MultipartGet(int bibId, int patronId, int? pickupLocId = null)
        {
            var url = $"/public/v1/1033/100/{OrganizationId}/bib/{bibId}/multiparts?PatronID={patronId}";
            if (pickupLocId.HasValue)
            {
                url += $"&PickupLocID={pickupLocId.Value}";
            }
            var request = new PapiRestRequest(HttpMethod.Get, url);
            return Execute<MultipartGetResult>(request);
        }
    }
}
