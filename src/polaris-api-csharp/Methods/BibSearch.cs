namespace Clc.Polaris.Api
{
    public partial class PapiClient
    {
        public async Task<IRestResponse<BibSearchResult>> BibSearchAsync(BibSearchOptions options, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(options);
            Require.Argument(options.Term);
            var branchId = Require.PositiveIfProvidedOrDefault(options.Branch, OrganizationId);
            Require.Positive(options.Page);
            Require.Positive(options.PageSize);

            var url = $"/public/v1/1033/100/{branchId}/search/bibs/{options.SearchType}";
            if (options.SearchType == BibSearchTypes.keyword) { url += $"/{options.Qualifier}"; }

            var request = PapiRestRequest.Get(url);
            request.BlockStaffOverride = true;
            request.QueryParameters.Add("q", options.Term ?? string.Empty);
            request.QueryParameters.Add("sort", options.SortOption);
            request.QueryParameters.Add("page", options.Page);
            request.QueryParameters.Add("bibsperpage", options.PageSize);
            request.QueryParameters.Add("limit", options.Limit ?? string.Empty);

            return await ExecutePapiAsync<BibSearchResult>(request, cancellationToken).ConfigureAwait(false);
        }
    }
}
