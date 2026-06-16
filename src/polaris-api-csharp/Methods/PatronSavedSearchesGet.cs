namespace Clc.Polaris.Api
{
    public partial class PapiClient
    {
        public async Task<IRestResponse<PatronSavedSearchesGetResult>> PatronSavedSearchesGetAsync(string barcode, string password = "", CancellationToken cancellationToken = default)
        {
            var url = $"/public/v1/1033/100/{OrganizationId}/patron/{EncodeBarcodePathSegment(barcode)}/savedsearches";
            var request = PapiRestRequest.Get(url, password: password);
            return await ExecutePapiAsync<PatronSavedSearchesGetResult>(request, cancellationToken).ConfigureAwait(false);
        }
    }
}