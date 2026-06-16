
namespace Clc.Polaris.Api
{
    public partial class PapiClient
    {
        public async Task<IRestResponse<HeadingsSearchResult>> HeadingsSearchAsync(HeadingSearchQualifier qualifierName, int numberOfTerms, int preferredPosition, string? startPoint = null, int? noTransaction = null, int? branchId = null, CancellationToken cancellationToken = default)
        {
            Require.Positive(numberOfTerms);
            Require.PositiveIfProvided(noTransaction);
            Require.PositiveIfProvided(branchId);

            var url = $"/public/v1/1033/100/{branchId ?? OrganizationId}/search/headings/{qualifierName}";
            var request = PapiRestRequest.Get(url);
            request.BlockStaffOverride = true;
            if (startPoint != null)
            {
                request.QueryParameters.Add("startpoint", startPoint);
            }

            request.QueryParameters.Add("numterms", numberOfTerms);
            request.QueryParameters.Add("preferredpos", preferredPosition);
            if (noTransaction != null)
            {
                request.QueryParameters.Add("notran", noTransaction.Value);
            }

            return await ExecutePapiAsync<HeadingsSearchResult>(request, cancellationToken).ConfigureAwait(false);
        }
    }
}
