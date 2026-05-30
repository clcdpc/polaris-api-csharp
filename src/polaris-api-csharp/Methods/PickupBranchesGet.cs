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
        

        public async Task<IRestResponse<PickupBranchesGetResult>> PickupBranchesGetAsync(int? organizationId = null, CancellationToken cancellationToken = default)
        {
            var url = $"/public/v1/1033/100/{organizationId ?? OrganizationId}/pickupbranches";
            var request = new PapiRestRequest(url) { BlockStaffOverride = true };
            return await ExecutePapiAsync<PickupBranchesGetResult>(request, cancellationToken).ConfigureAwait(false);
        }
    }
}
