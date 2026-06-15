using Clc.Polaris.Api.Models;
using Clc.Polaris.Api.Validation;
using Clc.Rest;
using System.Threading;
using System.Threading.Tasks;
namespace Clc.Polaris.Api { public partial class PapiClient { public async Task<IRestResponse<SysHoldStatusesGetResult>> SysHoldStatusesGetAsync(int? branchId = null, CancellationToken cancellationToken = default) { Require.PositiveIfProvided(branchId); var request = PapiRestRequest.Get($"/public/v1/1033/100/{branchId ?? OrganizationId}/sysholdstatuses"); request.BlockStaffOverride = true; return await ExecutePapiAsync<SysHoldStatusesGetResult>(request, cancellationToken).ConfigureAwait(false); } } }
