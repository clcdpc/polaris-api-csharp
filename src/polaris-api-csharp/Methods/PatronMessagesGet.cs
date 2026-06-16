
namespace Clc.Polaris.Api
{
    public partial class PapiClient
    {

        public async Task<IRestResponse<PatronMessagesGetResult>> PatronMessagesGetAsync(string barcode, bool unreadOnly = false, string password = "", CancellationToken cancellationToken = default)
        {
            var url = $"/public/v1/1033/100/{OrganizationId}/patron/{EncodeBarcodePathSegment(barcode)}/messages";
            var request = PapiRestRequest.Get(url, password: password);
            request.QueryParameters.Add("unreadonly", unreadOnly ? 1 : 0);
            return await ExecutePapiAsync<PatronMessagesGetResult>(request, cancellationToken).ConfigureAwait(false);
        }
    }
}