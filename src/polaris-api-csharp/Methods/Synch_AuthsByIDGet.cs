using Clc.Rest;
using Clc.Polaris.Api.Models;

namespace Clc.Polaris.Api
{
    public partial class PapiClient
    {
        public IRestResponse<GetAuthsByIDResult> Synch_AuthsByIDGet(int[] authIds)
        {
            var url = $"/protected/v1/1033/100/1/{Token.AccessToken}/synch/auths/MARCXML?authids={string.Join(",", authIds)}";
            var request = new PapiRestRequest(url);
            return Execute<GetAuthsByIDResult>(request);
        }

        public IRestResponse<GetAuthsByIDResult> Synch_AuthsByIDGet(int authId) => Synch_AuthsByIDGet(new[] { authId });
    }
}
