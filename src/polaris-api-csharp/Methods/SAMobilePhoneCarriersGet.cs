using Clc.Polaris.Api.Models;
using Clc.Polaris.Api.Validation;
using Clc.Rest;
using System.Threading;
using System.Threading.Tasks;
namespace Clc.Polaris.Api { public partial class PapiClient { public async Task<IRestResponse<SAMobilePhoneCarriersGetResult>> SAMobilePhoneCarriersGetAsync(int? organizationId = null, CancellationToken cancellationToken = default) { Require.PositiveIfProvided(organizationId); var request = PapiRestRequest.Get($"/protected/v1/1033/100/{organizationId ?? OrganizationId}/{ProtectedToken.Placeholder}/sysadmin/mobilephonecarriers"); return await ExecutePapiAsync<SAMobilePhoneCarriersGetResult>(request, cancellationToken).ConfigureAwait(false); } } }
