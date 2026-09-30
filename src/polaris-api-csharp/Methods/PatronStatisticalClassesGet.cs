namespace Clc.Polaris.Api
{
    public partial class PapiClient
    {
        public async Task<IRestResponse<PatronStatisticalClassesGetResult>> PatronStatisticalClassesGetAsync(int? organizationId = null, CancellationToken cancellationToken = default)
        {
            Require.PositiveIfProvided(organizationId);

            var url = $"/public/v1/1033/100/{organizationId ?? OrganizationId}/patronstatisticalclasses";
            var request = PapiRestRequest.Get(url);
            request.BlockStaffOverride = true;
            return await ExecutePapiAsync<PatronStatisticalClassesGetResult>(request, cancellationToken).ConfigureAwait(false);
        }
    }
}
