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
        public async Task<IRestResponse<HeadingsSearchResult>> HeadingsSearchAsync(SearchQualifiers qualifierName, int numberOfTerms, int preferredPosition, string? startPoint = null, int? noTransaction = null, int? branchId = null, CancellationToken cancellationToken = default)
        {
            if (qualifierName != SearchQualifiers.AU && qualifierName != SearchQualifiers.SE && qualifierName != SearchQualifiers.SU && qualifierName != SearchQualifiers.TI) throw new ArgumentOutOfRangeException(nameof(qualifierName), qualifierName, "Heading qualifier must be AU, SE, SU, or TI.");
            Require.Positive(numberOfTerms); Require.PositiveIfProvided(noTransaction); Require.PositiveIfProvided(branchId);
            var request = PapiRestRequest.Get($"/public/v1/1033/100/{branchId ?? OrganizationId}/search/headings/{qualifierName}");
            if (startPoint != null) request.QueryParameters.Add("startpoint", startPoint);
            request.QueryParameters.Add("numterms", numberOfTerms); request.QueryParameters.Add("preferredpos", preferredPosition);
            if (noTransaction != null) request.QueryParameters.Add("notran", noTransaction.Value);
            return await ExecutePapiAsync<HeadingsSearchResult>(request, cancellationToken).ConfigureAwait(false);
        }
    }
}
