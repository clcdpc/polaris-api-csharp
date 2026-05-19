using Clc.Rest;
using Clc.Polaris.Api.Models;
using Clc.Polaris.Models;
using System.Net;
using System.Net.Http;

namespace Clc.Polaris.Api
{
    public partial class PapiClient
    {
        public IRestResponse<PatronUpdateResult> PatronUpdateV2(string barcode, PatronRegistrationData data, string password = "")
        {
            var url = $"/public/v2/1033/100/{OrganizationId}/patron/{WebUtility.UrlEncode(barcode)}";
            var request = new PapiRestRequest(HttpMethod.Put, url) { Password = password, Body = data };
            return Execute<PatronUpdateResult>(request);
        }
    }
}
