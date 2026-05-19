using Clc.Rest;
using Clc.Polaris.Api.Models;

namespace Clc.Polaris.Api
{
    public partial class PapiClient
    {
        public IRestResponse<BibIDListGetResult> Synch_GetMaxBibID()
        {
            var url = $"/protected/v1/1033/100/1/{Token.AccessToken}/synch/bibs/maxid";
            var request = new PapiRestRequest(url);
            return Execute<BibIDListGetResult>(request);
        }
    }
}
