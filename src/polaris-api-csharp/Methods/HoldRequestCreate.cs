
namespace Clc.Polaris.Api
{
    public partial class PapiClient
    {

        public async Task<IRestResponse<HoldRequestCreateResult>> HoldRequestCreateAsync(HoldRequestCreateParams holdParams, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(holdParams);
            Require.Positive(holdParams.PatronID);
            Require.Positive(holdParams.BibID);
            Require.NonNegative(holdParams.PickupOrgID);
            holdParams.WorkstationID = Require.PositiveIfProvidedOrDefault(holdParams.WorkstationID, WorkstationId);
            holdParams.UserID = Require.PositiveIfProvidedOrDefault(holdParams.UserID, UserId);
            holdParams.RequestingOrgID = Require.PositiveIfProvidedOrDefault(holdParams.RequestingOrgID, OrganizationId);

            var url = $"/public/v1/1033/100/{holdParams.RequestingOrgID}/holdrequest";
            var request = PapiRestRequest.Post(url, body: holdParams);
            return await ExecutePapiAsync<HoldRequestCreateResult>(request, cancellationToken).ConfigureAwait(false);
        }
    }
}
