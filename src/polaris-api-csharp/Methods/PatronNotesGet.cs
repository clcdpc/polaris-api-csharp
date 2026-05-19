using Clc.Rest;
using Clc.Polaris.Api.Models;
using System.Net;

namespace Clc.Polaris.Api
{
    public partial class PapiClient
    {
        public IRestResponse<PatronNotesGetResult> PatronNotesGet(string barcode, int? workstationId = null)
        {
            var url = $"/protected/v1/1033/100/1/{Token.AccessToken}/patron/{WebUtility.UrlEncode(barcode)}/notes?wsid={workstationId ?? WorkstationId}";
            var request = new PapiRestRequest(url);
            return Execute<PatronNotesGetResult>(request);
        }
    }
}
