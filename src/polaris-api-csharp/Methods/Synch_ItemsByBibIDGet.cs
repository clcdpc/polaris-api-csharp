using Clc.Rest;
using Clc.Polaris.Api.Models;

namespace Clc.Polaris.Api
{
    public partial class PapiClient
    {
        public IRestResponse<ItemGetResult> Synch_ItemsByBibIDGet(int bibId, bool excludeecontent = false)
        {
            var url = $"/protected/v1/1033/100/1/{Token.AccessToken}/synch/items/bibid/{bibId}?excludeecontent={excludeecontent.ToString().ToLower()}";
            var request = new PapiRestRequest(url);
            return Execute<ItemGetResult>(request);
        }
    }
}
