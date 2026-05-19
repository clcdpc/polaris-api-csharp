using Clc.Rest;
using Clc.Polaris.Api.Models;

namespace Clc.Polaris.Api
{
    public partial class PapiClient
    {
        public IRestResponse<ItemGetResult> Synch_ItemByIDGet(int itemId)
        {
            var url = $"/protected/v1/1033/100/1/{Token.AccessToken}/synch/item/{itemId}";
            var request = new PapiRestRequest(url);
            return Execute<ItemGetResult>(request);
        }
    }
}
