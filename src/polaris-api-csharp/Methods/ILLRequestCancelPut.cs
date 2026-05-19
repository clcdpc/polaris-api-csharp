using Clc.Rest;
using Clc.Polaris.Api.Models;
using System.Net;
using System.Net.Http;

namespace Clc.Polaris.Api
{
    public partial class PapiClient
    {
        public IRestResponse<ILLRequestCancelResult> ILLRequestCancel(string barcode, int requestId, int? userId = null, int? workstationId = null)
        {
            var url = $"/public/v1/1033/100/{OrganizationId}/patron/{WebUtility.UrlEncode(barcode)}/illrequests/{requestId}/cancelled?wsid={workstationId ?? WorkstationId}&userid={userId ?? UserId}";
            var request = new PapiRestRequest(HttpMethod.Put, url);
            return Execute<ILLRequestCancelResult>(request);
        }

        public IRestResponse<ILLRequestCancelResult> ILLRequestCancelAll(string barcode, int? userId = null, int? workstationId = null)
        {
            return ILLRequestCancel(barcode, 0, userId, workstationId);
        }
    }
}
