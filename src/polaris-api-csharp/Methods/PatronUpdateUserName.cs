using System;
using System.Threading.Tasks;
using System.Xml.Linq;
using Clc.Polaris.Api.Validation;
using Clc.Rest;
using Clc.Polaris.Api.Models;
using System.Net.Http;

namespace Clc.Polaris.Api
{
    public partial class PapiClient
    {
        

        public IRestResponse<PapiResponseCommon> PatronUpdateUserName(string barcode, string newUsername, string password = "")
        {
            var url = $"/public/v1/1033/100/1/patron/{barcode}/username/{newUsername}";
            var request = new PapiRestRequest(HttpMethod.Put, url) { Password = password };
            return Execute<PapiResponseCommon>(request);
        }
    }
}