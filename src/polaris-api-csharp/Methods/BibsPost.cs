using Clc.Rest;
using Clc.Polaris.Api.Models;
using System.Net;
using System.Net.Http;

namespace Clc.Polaris.Api
{
    public partial class PapiClient
    {
        public IRestResponse<BibsPostResult> BibsImport(string marcxml, string importProfileName = null, int? workstationId = null)
        {
            var url = $"/protected/v1/1033/100/1/{Token.AccessToken}/bibs?";
            if (!string.IsNullOrEmpty(importProfileName))
            {
                url += $"ImportProfileName={WebUtility.UrlEncode(importProfileName)}&";
            }
            url += $"wsid={workstationId ?? WorkstationId}";

            var request = new PapiRestRequest(HttpMethod.Post, url) { Body = marcxml };
            return Execute<BibsPostResult>(request);
        }
    }
}
