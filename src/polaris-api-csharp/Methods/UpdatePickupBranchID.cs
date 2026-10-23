namespace Clc.Polaris.Api
{
    public partial class PapiClient
    {
        public async Task<IRestResponse<PAPIResult>> UpdatePickupBranchIDAsync(string barcode, int requestId, int pickupBranchId, string password = "", int? userId = null, int? workstationId = null, CancellationToken cancellationToken = default)
        {
            Require.Positive(requestId);
            Require.Positive(pickupBranchId);
            Require.PositiveIfProvided(userId);
            Require.PositiveIfProvided(workstationId);

            var url = $"/public/v1/1033/100/{OrganizationId}/patron/{EncodeBarcodePathSegment(barcode)}/holdrequests/{requestId}/pickupbranch";
            var request = PapiRestRequest.Put(url, password: password);
            request.QueryParameters.Add("userid", userId ?? UserId);
            request.QueryParameters.Add("wsid", workstationId ?? WorkstationId);
            request.QueryParameters.Add("pickupbranchid", pickupBranchId);
            return await ExecutePapiAsync<PAPIResult>(request, cancellationToken).ConfigureAwait(false);
        }
    }
}
