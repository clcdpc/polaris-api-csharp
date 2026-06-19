namespace Clc.Polaris.Api
{
    public partial class PapiClient
    {
        public async Task<IRestResponse<PatronNotesResult>> PatronNotesGetAsync(string barcode, string password = "", CancellationToken cancellationToken = default)
        {
            var url = $"/public/v1/1033/100/{OrganizationId}/patron/{EncodeBarcodePathSegment(barcode)}/notes";
            var request = PapiRestRequest.Get(url, password: password);
            return await ExecutePapiAsync<PatronNotesResult>(request, cancellationToken).ConfigureAwait(false);
        }
    }
}
