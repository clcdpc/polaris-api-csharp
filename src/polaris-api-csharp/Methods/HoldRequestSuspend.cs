namespace Clc.Polaris.Api
{
    public partial class PapiClient
    {
        public async Task<IRestResponse<HoldRequestActivationResult>> HoldRequestSuspendAsync(string barcode, int requestId, DateTime activationDate, string password = "", int? userId = null, CancellationToken cancellationToken = default)
        {
            Require.NonNegative(requestId);
            Require.PositiveIfProvided(userId);

            var url = $"/public/v1/1033/100/{OrganizationId}/patron/{EncodeBarcodePathSegment(barcode)}/holdrequests/{requestId}/inactive";
            var request = PapiRestRequest.Put(url, body: new HoldRequestActivationData(userId ?? UserId, activationDate), password: password);
            return await ExecutePapiAsync<HoldRequestActivationResult>(request, cancellationToken).ConfigureAwait(false);
        }
    }
}
