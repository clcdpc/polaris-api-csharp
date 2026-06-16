
namespace Clc.Polaris.Api
{
    public partial class PapiClient
    {
        public async Task<IRestResponse<PatronAuthenticationResult>> AuthenticatePatronAsync(string barcode, string password, CancellationToken cancellationToken = default)
        {
            var url = $"/public/v1/1033/100/{OrganizationId}/authenticator/patron";
            var body = new { Barcode = barcode, Password = password };
            var request = PapiRestRequest.Post(url, body: body);
            request.BlockStaffOverride = true;
            return await ExecutePapiAsync<PatronAuthenticationResult>(request, cancellationToken).ConfigureAwait(false);
        }
    }
}
