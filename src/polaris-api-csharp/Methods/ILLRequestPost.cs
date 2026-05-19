using Clc.Rest;
using Clc.Polaris.Api.Models;
using System.Net.Http;

namespace Clc.Polaris.Api
{
    public partial class PapiClient
    {
        public IRestResponse<ILLRequestResult> ILLRequestCreate(ILLRequestCreateData data)
        {
            var url = $"/public/v1/1033/100/{OrganizationId}/illrequest";
            var request = new PapiRestRequest(HttpMethod.Post, url) { Body = data };
            return Execute<ILLRequestResult>(request);
        }
    }
}
