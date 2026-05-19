using Clc.Rest;
using Clc.Polaris.Api.Models;
using System.Net.Http;

namespace Clc.Polaris.Api
{
    public partial class PapiClient
    {
        public IRestResponse<SAMobilePhoneCarriersGetResult> SAMobilePhoneCarriersGet(int? orgId = null)
        {
            var url = $"/protected/v1/1033/100/{orgId ?? OrganizationId}/{Token.AccessToken}/sysadmin/mobilephonecarriers";
            var request = new PapiRestRequest(HttpMethod.Get, url);
            return Execute<SAMobilePhoneCarriersGetResult>(request);
        }
    }
}
