using Clc.Rest;
using Clc.Polaris.Api.Models;
using System.Net;
using System.Net.Http;

namespace Clc.Polaris.Api
{
    public partial class PapiClient
    {
        public IRestResponse<BibGetByTypeResult> BibGetByTypeV2(string key, string type, int? orgId = null)
        {
            var url = $"/public/v2/1033/100/{orgId ?? OrganizationId}/bib/{WebUtility.UrlEncode(key)}?type={WebUtility.UrlEncode(type)}";
            var request = new PapiRestRequest(HttpMethod.Get, url);
            return Execute<BibGetByTypeResult>(request);
        }
    }
}
