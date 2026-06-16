
namespace Clc.Polaris.Api
{
    public partial class PapiClient
    {
        public async Task<IRestResponse<HoldRequestActivationResult>> HoldRequestReactivateAsync(string barcode, string password, int requestId, DateTime activationDate, int? userId = null, CancellationToken cancellationToken = default)
        {
            Require.Positive(requestId);
            Require.PositiveIfProvided(userId);

            var url = $"/public/v1/1033/100/{OrganizationId}/patron/{EncodeBarcodePathSegment(barcode)}/holdrequests/{requestId}/active";
            var request = PapiRestRequest.Put(url, body: new HoldRequestActivationData(userId ?? UserId, activationDate), password: password);
            return await ExecutePapiAsync<HoldRequestActivationResult>(request, cancellationToken).ConfigureAwait(false);
        }
    }
}
