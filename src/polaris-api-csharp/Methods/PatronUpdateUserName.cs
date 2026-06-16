
namespace Clc.Polaris.Api
{
    public partial class PapiClient
    {

        public async Task<IRestResponse<PatronUpdateResult>> PatronUpdateUserNameAsync(string barcode, string newUsername, string password = "", CancellationToken cancellationToken = default)
        {
            var url = $"/public/v1/1033/100/{OrganizationId}/patron/{EncodeBarcodePathSegment(barcode)}/username/{WebUtility.UrlEncode(newUsername)}";
            var request = PapiRestRequest.Put(url, password: password);
            return await ExecutePapiAsync<PatronUpdateResult>(request, cancellationToken).ConfigureAwait(false);
        }
    }
}