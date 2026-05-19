using Clc.Rest;
using Clc.Polaris.Api.Models;
using System.Net;
using System.Net.Http;

namespace Clc.Polaris.Api
{
    public partial class PapiClient
    {
        public IRestResponse<PapiResponseCommon> RequestsUpdateStatus(int requestId, string action, int? itemId = null, int? denyReasonId = null)
        {
            var url = $"/protected/v1/1033/100/{OrganizationId}/{Token.AccessToken}/circulation/requests/{requestId}/status?action={WebUtility.UrlEncode(action)}";
            if (itemId.HasValue)
            {
                url += $"&itemid={itemId.Value}";
            }
            if (denyReasonId.HasValue)
            {
                url += $"&denyreason={denyReasonId.Value}";
            }
            var request = new PapiRestRequest(HttpMethod.Put, url);
            return Execute<PapiResponseCommon>(request);
        }
    }
}
