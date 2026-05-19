using Clc.Rest;
using Clc.Polaris.Api.Models;

namespace Clc.Polaris.Api
{
    public partial class PapiClient
    {
        public IRestResponse<SysHoldStatusesGetResult> SysHoldStatusesGet(int? branchId = null)
        {
            var url = $"/public/v1/1033/100/{branchId ?? OrganizationId}/sysholdstatuses";
            var request = new PapiRestRequest(url) { BlockStaffOverride = true };
            return Execute<SysHoldStatusesGetResult>(request);
        }
    }
}
