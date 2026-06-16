
namespace Clc.Polaris.Api
{
    public partial class PapiClient
    {

        public async Task<IRestResponse<PatronPreferencesGetResult>> PatronPreferencesGetAsync(string barcode, string password = "", CancellationToken cancellationToken = default)
        {
            var url = $"/public/v1/1033/100/{OrganizationId}/patron/{EncodeBarcodePathSegment(barcode)}/preferences";
            var request = PapiRestRequest.Get(url, password: password);
            return await ExecutePapiAsync<PatronPreferencesGetResult>(request, cancellationToken).ConfigureAwait(false);
        }
    }
}