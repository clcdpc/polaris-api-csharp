using Clc.Rest;
using Clc.Polaris.Api.Models;

namespace Clc.Polaris.Api
{
    public partial class PapiClient
    {
        public IRestResponse<ItemIDListGetResult> Synch_GetItemIDList(int startid, int? nrecs = null)
        {
            var url = $"/protected/v1/1033/100/1/{Token.AccessToken}/synch/items/idlist?startid={startid}";
            if (nrecs.HasValue)
            {
                url += $"&nrecs={nrecs}";
            }
            var request = new PapiRestRequest(url);
            return Execute<ItemIDListGetResult>(request);
        }
    }
}
