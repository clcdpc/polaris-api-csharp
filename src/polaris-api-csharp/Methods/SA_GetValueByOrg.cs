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


        public async Task<IRestResponse<StringResult>> SA_GetValueByOrgAsync(string attribute, int? organizationId = null, CancellationToken cancellationToken = default)
        {
            var token = await GetTokenAsync(cancellationToken).ConfigureAwait(false);
            var url = $"/protected/v1/1033/100/1/{token.AccessToken}/organization/{organizationId ?? OrganizationId}/sysadmin/attribute/{attribute}";
            var request = new PapiRestRequest(url);
            return await ExecutePapiAsync<StringResult>(request, cancellationToken).ConfigureAwait(false);
        }
    }
}
