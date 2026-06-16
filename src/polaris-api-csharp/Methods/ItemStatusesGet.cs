
namespace Clc.Polaris.Api
{
    public partial class PapiClient
    {

        public async Task<IRestResponse<ItemStatusesGetResult>> ItemStatusesGetAsync(int? branchId = null, CancellationToken cancellationToken = default)
        {
            Require.PositiveIfProvided(branchId);

            var url = $"/public/v1/1033/100/{branchId ?? OrganizationId}/itemstatuses";
            var request = PapiRestRequest.Get(url);
            request.BlockStaffOverride = true;
            return await ExecutePapiAsync<ItemStatusesGetResult>(request, cancellationToken).ConfigureAwait(false);
        }
    }
}
