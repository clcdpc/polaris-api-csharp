using Clc.Rest;
using Clc.Polaris.Api.Models;

namespace Clc.Polaris.Api
{
    public partial class PapiClient
    {
        public IRestResponse<PatronStatisticalClassesGetResult> PatronStatisticalClassesGet(int? branchId = null)
        {
            var url = $"/public/v1/1033/100/{branchId ?? OrganizationId}/patron/statisticalclasses";
            var request = new PapiRestRequest(url) { BlockStaffOverride = true };
            return Execute<PatronStatisticalClassesGetResult>(request);
        }
    }
}
