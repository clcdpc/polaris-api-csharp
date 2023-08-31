using Clc.Rest;
using Clc.Polaris.Api.Models;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;

namespace Clc.Polaris.Api
{
    public partial class PapiClient
    {
        public IRestResponse<PatronAuthenticationResult> AuthenticatePatron(string barcode, string password)
        {
            var url = "/public/v1/1033/100/1/authenticator/patron";
            var body = $"<PatronAuthenticationData><Barcode>{barcode}</Barcode><Password>{password}</Password></PatronAuthenticationData>";
            var body2 = new { Barcode = barcode, Password = password };
            var request = new PapiRestRequest(HttpMethod.Post, url) { Body = body2, BlockStaffOverride = true };
            return Execute<PatronAuthenticationResult>(request);
        }
    }
}
