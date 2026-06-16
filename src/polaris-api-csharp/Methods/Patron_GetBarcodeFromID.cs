namespace Clc.Polaris.Api
{
    public partial class PapiClient
    {

        public async Task<IRestResponse<GetBarcodeAndPatronIDResult>> Patron_GetBarcodeFromIdAsync(int patronId, CancellationToken cancellationToken = default)
        {
            Require.Positive(patronId);

            var url = $"/protected/v1/1033/100/{OrganizationId}/{ProtectedToken.Placeholder}/patron/barcode";
            var request = PapiRestRequest.Get(url);
            request.QueryParameters.Add("patronid", patronId);
            return await ExecutePapiAsync<GetBarcodeAndPatronIDResult>(request, cancellationToken).ConfigureAwait(false);
        }
    }
}
