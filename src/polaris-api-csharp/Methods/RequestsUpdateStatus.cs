using System;
using System.Threading;
using System.Threading.Tasks;
using Clc.Polaris.Api.Models;
using Clc.Polaris.Api.Validation;
using Clc.Rest;

namespace Clc.Polaris.Api
{
    public partial class PapiClient
    {
        public async Task<IRestResponse<PAPIResult>> RequestsUpdateStatusAsync(int requestId, RequestStatusAction action, int? itemId = null, int? denyReason = null, int? organizationId = null, CancellationToken cancellationToken = default)
        {
            Require.Positive(requestId);
            Require.PositiveIfProvided(itemId);
            Require.PositiveIfProvided(denyReason);
            Require.PositiveIfProvided(organizationId);
            ValidateRequestStatusActionParameters(action, itemId, denyReason);

            var url = $"/protected/v1/1033/100/{organizationId ?? OrganizationId}/{ProtectedToken.Placeholder}/circulation/requests/{requestId}/status";
            var request = PapiRestRequest.Put(url);
            request.QueryParameters.Add("action", GetRequestStatusActionValue(action));
            if (itemId != null)
            {
                request.QueryParameters.Add("itemid", itemId.Value);
            }

            if (denyReason != null)
            {
                request.QueryParameters.Add("denyreason", denyReason.Value);
            }

            return await ExecutePapiAsync<PAPIResult>(request, cancellationToken).ConfigureAwait(false);
        }

        private static void ValidateRequestStatusActionParameters(RequestStatusAction action, int? itemId, int? denyReason)
        {
            if (action == RequestStatusAction.Deny && denyReason == null)
            {
                throw new ArgumentException("A deny reason is required when denying a request.", nameof(denyReason));
            }

            if (action != RequestStatusAction.Deny && itemId == null)
            {
                throw new ArgumentException("An item ID is required when locating, returning, or asking-me-later for a request.", nameof(itemId));
            }
        }

        private static string GetRequestStatusActionValue(RequestStatusAction action)
        {
            return action switch
            {
                RequestStatusAction.Deny => "deny",
                RequestStatusAction.Locate => "locate",
                RequestStatusAction.Return => "return",
                RequestStatusAction.AskMeLater => "askmelater",
                _ => throw new ArgumentOutOfRangeException(nameof(action), action, "Unsupported request status action.")
            };
        }
    }
}
