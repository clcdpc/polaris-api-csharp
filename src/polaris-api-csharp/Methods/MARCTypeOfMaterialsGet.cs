
namespace Clc.Polaris.Api
{
    public partial class PapiClient
    {

        public async Task<IRestResponse<MARCTypeOfMaterialsGetResult>> MARCTypeOfMaterialsGetAsync(int? branchId = null, CancellationToken cancellationToken = default)
        {
            Require.PositiveIfProvided(branchId);

            var url = $"/public/v1/1033/100/{branchId ?? OrganizationId}/marctypeofmaterials";
            var request = PapiRestRequest.Get(url);
            request.BlockStaffOverride = true;
            return await ExecutePapiAsync<MARCTypeOfMaterialsGetResult>(request, cancellationToken).ConfigureAwait(false);
        }
    }
}
