namespace Clc.Polaris.Api
{
    public partial class PapiClient
    {
        public async Task<IRestResponse<PickupAreasGetResult>> PickupAreasGetAsync(int? organizationId = null, CancellationToken cancellationToken = default)
        {
            Require.PositiveIfProvided(organizationId);

            var url = $"/public/v1/1033/100/{organizationId ?? OrganizationId}/pickupareas";
            var request = PapiRestRequest.Get(url);
            request.BlockStaffOverride = true;
            return await ExecutePapiAsync<PickupAreasGetResult>(request, cancellationToken).ConfigureAwait(false);
        }
    }
}
