namespace Clc.Polaris.Api
{
    public partial class PapiClient
    {
        public async Task<IRestResponse<LimitFiltersGetResult>> LimitFiltersGetAsync(int? branchId = null, CancellationToken cancellationToken = default)
        {
            Require.PositiveIfProvided(branchId);

            var url = $"/public/v1/1033/100/{branchId ?? OrganizationId}/limitfilters";
            var request = PapiRestRequest.Get(url);
            request.BlockStaffOverride = true;
            return await ExecutePapiAsync<LimitFiltersGetResult>(request, cancellationToken).ConfigureAwait(false);
        }
    }
}
