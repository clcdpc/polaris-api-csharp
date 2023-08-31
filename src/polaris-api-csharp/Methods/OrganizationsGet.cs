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
        public IRestResponse<OrganizationsGetResult> OrganizationsGet(OrganizationType type = OrganizationType.All)
        {
            var url = $"/public/v1/1033/100/1/organizations/{type}";
            var request = new PapiRestRequest(url) { BlockStaffOverride = true };
            return Execute<OrganizationsGetResult>(request);
        }
    }
}
