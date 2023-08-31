using Clc.Rest;
using Clc.Polaris.Api.Models;

using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Xml.Linq;

namespace Clc.Polaris.Api
{
    public partial class PapiClient
    {
        public IRestResponse<ProtectedToken> AuthenticateStaffUser(PolarisUser staffUser)
        {
            var url = "/protected/v1/1033/100/1/authenticator/staff";
            var request = new PapiRestRequest(HttpMethod.Post, url) { Body = staffUser };
            return Post<ProtectedToken>(url, body: staffUser);
            //return Execute<ProtectedToken>(request);
        }

        
    }
}
