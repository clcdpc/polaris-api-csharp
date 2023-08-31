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
        public IRestResponse<PapiResponseCommon> ApiKeyValidate()
        {
            var url = "/public/v1/1033/100/1/apikeyvalidate";
            var request = new PapiRestRequest(url) { BlockStaffOverride = true };// { AuthRequired = false };
            return Execute<PapiResponseCommon>(request);
        }
    }
}