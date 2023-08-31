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
        /// <summary>
        /// Get an SA value
        /// </summary>
        /// <param name="orgId"></param>
        /// <param name="attribute"></param>
        /// <returns></returns>
        

        public IRestResponse<StringResult> SA_GetValueByOrg(string attribute, int? organizationId = null)
        {
            var url = $"/protected/v1/1033/100/1/{Token.AccessToken}/organization/{organizationId ?? OrganizationId}/sysadmin/attribute/{attribute}";
            var request = new PapiRestRequest(url);
            return Execute<StringResult>(request);
        }
    }
}
