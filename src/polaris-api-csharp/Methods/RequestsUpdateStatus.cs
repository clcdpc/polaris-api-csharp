using Clc.Polaris.Api.Models;
using Clc.Polaris.Api.Validation;
using Clc.Rest;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Clc.Polaris.Api
{
    public partial class PapiClient
    {
        public async Task<IRestResponse<PapiResponseCommon>> RequestsUpdateStatusAsync(int requestId, RequestStatusAction action, int? itemId = null, int? denyReason = null, int? organizationId = null, CancellationToken cancellationToken = default)
        {
            Require.Positive(requestId); Require.PositiveIfProvided(itemId); Require.PositiveIfProvided(denyReason); Require.PositiveIfProvided(organizationId);
            if (action == RequestStatusAction.Deny && denyReason == null) throw new ArgumentException("denyreason is required for deny.", nameof(denyReason));
            if ((action == RequestStatusAction.Locate || action == RequestStatusAction.Return || action == RequestStatusAction.AskMeLater) && itemId == null) throw new ArgumentException("itemid is required for this action.", nameof(itemId));
            var request = PapiRestRequest.Put($"/protected/v1/1033/100/{organizationId ?? OrganizationId}/{ProtectedToken.Placeholder}/circulation/requests/{requestId}/status");
            request.QueryParameters.Add("action", ToRequestStatusActionValue(action));
            if (itemId != null) request.QueryParameters.Add("itemid", itemId.Value);
            if (denyReason != null) request.QueryParameters.Add("denyreason", denyReason.Value);
            return await ExecutePapiAsync<PapiResponseCommon>(request, cancellationToken).ConfigureAwait(false);
        }

        private static string ToRequestStatusActionValue(RequestStatusAction action) => action switch
        {
            RequestStatusAction.Deny => "deny",
            RequestStatusAction.Locate => "locate",
            RequestStatusAction.Return => "return",
            RequestStatusAction.AskMeLater => "askmelater",
            _ => throw new ArgumentOutOfRangeException(nameof(action), action, null)
        };
    }
}
