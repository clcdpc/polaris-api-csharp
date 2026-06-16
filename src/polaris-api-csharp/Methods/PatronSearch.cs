namespace Clc.Polaris.Api
{
    public partial class PapiClient
    {

        public async Task<IRestResponse<PatronSearchResult>> PatronSearchAsync(string query, int page = 1, int pageSize = 10, PatronSortKeys sortBy = PatronSortKeys.PATN, int? orgId = null, CancellationToken cancellationToken = default)
        {
            Require.Positive(page);
            Require.Positive(pageSize);
            Require.PositiveIfProvided(orgId);

            var url = $"/protected/v1/1033/100/{orgId ?? OrganizationId}/{ProtectedToken.Placeholder}/search/patrons/Boolean";
            var request = PapiRestRequest.Get(url);
            request.QueryParameters.Add("q", query);
            request.QueryParameters.Add("patronsperpage", pageSize);
            request.QueryParameters.Add("page", page);
            request.QueryParameters.Add("sort", sortBy);
            return await ExecutePapiAsync<PatronSearchResult>(request, cancellationToken).ConfigureAwait(false);
        }
    }
}
