using Clc.Rest;
using Clc.Polaris.Api.Models;
using System.Net;

namespace Clc.Polaris.Api
{
    public partial class PapiClient
    {
        public IRestResponse<GetSubscriptionsByIDResult> Synch_GetSubscriptionsByID(string bibids)
        {
            var url = $"/protected/v1/1033/100/1/{Token.AccessToken}/synch/subscriptions/SubscriptionXML?bibids={WebUtility.UrlEncode(bibids)}";
            var request = new PapiRestRequest(url);
            return Execute<GetSubscriptionsByIDResult>(request);
        }
    }
}
