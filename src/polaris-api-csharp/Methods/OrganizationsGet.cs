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
        public async Task<IRestResponse<OrganizationsGetResult>> OrganizationsGetAsync(OrganizationType type = OrganizationType.All, CancellationToken cancellationToken = default)
        {
            var url = $"/public/v1/1033/100/1/organizations/{type}";
            var request = new PapiRestRequest(url) { BlockStaffOverride = true };
            return await ExecutePapiAsync<OrganizationsGetResult>(request, cancellationToken).ConfigureAwait(false);
        }
    }
}
