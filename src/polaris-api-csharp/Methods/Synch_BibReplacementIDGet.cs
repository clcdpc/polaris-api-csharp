using Clc.Rest;
using Clc.Polaris.Api.Models;

namespace Clc.Polaris.Api
{
    public partial class PapiClient
    {
        public IRestResponse<BibReplacementIDGetResult> Synch_BibReplacementIDGet(string startdate)
        {
            var url = $"/protected/v1/1033/100/1/{Token.AccessToken}/synch/bibs/replacementids?startdate={startdate}";
            var request = new PapiRestRequest(url);
            return Execute<BibReplacementIDGetResult>(request);
        }
    }
}
