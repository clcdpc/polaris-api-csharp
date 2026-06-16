namespace Clc.Polaris.Api
{
    public partial class PapiClient
    {
        public async Task<IRestResponse<ILLRequestResult>> ILLRequestPostAsync(ILLRequestCreateData requestData, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(requestData);
            Require.Positive(requestData.PatronID);
            Require.Argument(requestData.Title);
            Require.NonNegative(requestData.PickupOrgID);
            Require.PositiveIfProvided(requestData.WorkstationID);
            Require.PositiveIfProvided(requestData.UserID);
            Require.PositiveIfProvided(requestData.HoldPickupAreaID);

            var url = $"/public/v1/1033/100/{OrganizationId}/illrequest";
            var request = PapiRestRequest.Post(url, body: requestData);
            return await ExecutePapiAsync<ILLRequestResult>(request, cancellationToken).ConfigureAwait(false);
        }
    }
}
