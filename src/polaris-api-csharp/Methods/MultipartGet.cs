
namespace Clc.Polaris.Api
{
    public partial class PapiClient
    {
        public async Task<IRestResponse<MultipartGetResult>> MultipartGetAsync(int bibId, int patronId, int? pickupLocationId = null, int? branchId = null, CancellationToken cancellationToken = default)
        {
            Require.Positive(bibId);
            Require.Positive(patronId);
            Require.PositiveIfProvided(pickupLocationId);
            Require.PositiveIfProvided(branchId);

            var url = $"/public/v1/1033/100/{branchId ?? OrganizationId}/bib/{bibId}/multiparts";
            var request = PapiRestRequest.Get(url);
            request.BlockStaffOverride = true;
            request.QueryParameters.Add("PatronID", patronId);
            if (pickupLocationId != null)
            {
                request.QueryParameters.Add("PickupLocID", pickupLocationId.Value);
            }

            return await ExecutePapiAsync<MultipartGetResult>(request, cancellationToken).ConfigureAwait(false);
        }
    }
}
