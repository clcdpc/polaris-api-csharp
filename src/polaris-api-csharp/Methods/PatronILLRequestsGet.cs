
namespace Clc.Polaris.Api
{
    public partial class PapiClient
    {

        public async Task<IRestResponse<PatronILLRequestsGetResult>> PatronILLRequestsGetAsync(string barcode, ILLStatus status = ILLStatus.All, string password = "", CancellationToken cancellationToken = default)
        {
            var url = $"/public/v1/1033/100/{OrganizationId}/patron/{EncodeBarcodePathSegment(barcode)}/illrequests/{status}";
            var request = PapiRestRequest.Get(url, password: password);
            return await ExecutePapiAsync<PatronILLRequestsGetResult>(request, cancellationToken).ConfigureAwait(false);
        }
    }
}
