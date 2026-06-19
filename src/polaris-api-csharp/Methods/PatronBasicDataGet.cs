namespace Clc.Polaris.Api
{
    public partial class PapiClient
    {
        public async Task<IRestResponse<PatronBasicDataGetResult>> PatronBasicDataGetAsync(string barcode, string password = "", bool addresses = false, bool notes = false, CancellationToken cancellationToken = default)
        {
            var url = $"/public/v1/1033/100/{OrganizationId}/patron/{EncodeBarcodePathSegment(barcode)}/basicdata";
            var request = PapiRestRequest.Get(url, password: password);
            request.QueryParameters.Add("addresses", Convert.ToInt32(addresses));
            request.QueryParameters.Add("notes", Convert.ToInt32(notes));
            return await ExecutePapiAsync<PatronBasicDataGetResult>(request, cancellationToken).ConfigureAwait(false);
        }
    }
}
