using Clc.Rest;
using Clc.Polaris.Api.Models;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Clc.Polaris.Api
{
    public partial class PapiClient
    {
        

        public IRestResponse<PickupBranchesGetResult> PickupBranchesGet(int? organizationId = null)
        {
            var url = $"/public/v1/1033/100/{organizationId ?? OrganizationId}/pickupbranches";
            var request = new PapiRestRequest(url) { BlockStaffOverride = true };
            return Execute<PickupBranchesGetResult>(request);
        }
    }
}
