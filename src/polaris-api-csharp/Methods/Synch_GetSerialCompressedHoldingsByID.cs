using Clc.Rest;
using Clc.Polaris.Api.Models;
using System.Net;

namespace Clc.Polaris.Api
{
    public partial class PapiClient
    {
        public IRestResponse<GetSerialCompressedHoldingsByIDResult> Synch_GetSerialCompressedHoldingsByID(string ids)
        {
            var url = $"/protected/v1/1033/100/1/{Token.AccessToken}/synch/serialholdings/textual/compressed?ids={WebUtility.UrlEncode(ids)}";
            var request = new PapiRestRequest(url);
            return Execute<GetSerialCompressedHoldingsByIDResult>(request);
        }
    }
}
