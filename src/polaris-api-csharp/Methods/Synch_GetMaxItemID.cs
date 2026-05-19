using Clc.Rest;
using Clc.Polaris.Api.Models;

namespace Clc.Polaris.Api
{
    public partial class PapiClient
    {
        public IRestResponse<ItemIDListGetResult> Synch_GetMaxItemID()
        {
            var url = $"/protected/v1/1033/100/1/{Token.AccessToken}/synch/items/maxid";
            var request = new PapiRestRequest(url);
            return Execute<ItemIDListGetResult>(request);
        }
    }
}
