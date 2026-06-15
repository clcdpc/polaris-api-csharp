using Clc.Polaris.Api.Models;
using Clc.Polaris.Api.Validation;
using Clc.Rest;
using System.Threading;
using System.Threading.Tasks;
namespace Clc.Polaris.Api { public partial class PapiClient { public async Task<IRestResponse<ILLRequestCancelResult>> ILLRequestCancelAsync(string barcode, int illRequestId, string password = "", int? userId = null, int? workstationId = null, CancellationToken cancellationToken = default) { Require.Argument(barcode); Require.NonNegative(illRequestId); Require.PositiveIfProvided(userId); Require.PositiveIfProvided(workstationId); var request = PapiRestRequest.Put($"/public/v1/1033/100/{OrganizationId}/patron/{EncodeBarcodePathSegment(barcode)}/illrequests/{illRequestId}/cancelled", password: password); request.QueryParameters.Add("wsid", workstationId ?? WorkstationId); request.QueryParameters.Add("userid", userId ?? UserId); return await ExecutePapiAsync<ILLRequestCancelResult>(request, cancellationToken).ConfigureAwait(false); } } }
