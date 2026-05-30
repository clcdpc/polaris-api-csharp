using Clc.Rest;
using Clc.Polaris.Api.Models;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
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
            var url = $"/protected/v1/1033/100/1/{ProtectedToken.Placeholder}/organization/{organizationId ?? OrganizationId}/sysadmin/attribute/{attribute}";
            var request = PapiRestRequest.Get(url);
            return await ExecutePapiAsync<StringResult>(request, cancellationToken).ConfigureAwait(false);
        }
    }
}
