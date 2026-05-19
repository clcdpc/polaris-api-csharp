using Clc.Rest;
using Clc.Polaris.Api.Models;

namespace Clc.Polaris.Api
{
    public partial class PapiClient
    {
        public IRestResponse<AuthIDListGetResult> Synch_GetMaxAuthID()
        {
            var url = $"/protected/v1/1033/100/1/{Token.AccessToken}/synch/auths/maxid";
            var request = new PapiRestRequest(url);
            return Execute<AuthIDListGetResult>(request);
        }
    }
}
