using Clc.Rest;
using Clc.Polaris.Api.Models;
using System.Net;

namespace Clc.Polaris.Api
{
    public partial class PapiClient
    {
        public IRestResponse<GetBibResourceCountsByIDResult> Synch_GetBibResourceCountsByID(string bibids)
        {
            var url = $"/protected/v1/1033/100/1/{Token.AccessToken}/synch/bibs/resourcecounts?bibids={WebUtility.UrlEncode(bibids)}";
            var request = new PapiRestRequest(url);
            return Execute<GetBibResourceCountsByIDResult>(request);
        }
    }
}
