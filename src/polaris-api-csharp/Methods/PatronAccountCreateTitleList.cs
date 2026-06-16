
namespace Clc.Polaris.Api
{
    public partial class PapiClient
    {

        public async Task<IRestResponse<PatronAccountCreateTitleListResult>> PatronAccountCreateTitleListAsync(string barcode, string listName, string password = "", CancellationToken cancellationToken = default)
        {
            var url = $"/public/v1/1033/100/{OrganizationId}/patron/{EncodeBarcodePathSegment(barcode)}/patronaccountcreatetitlelist";
            var body = new PatronAccountCreateTitleListData { RecordStoreName = listName };
            var request = PapiRestRequest.Post(url, body: body, password: password);
            return await ExecutePapiAsync<PatronAccountCreateTitleListResult>(request, cancellationToken).ConfigureAwait(false);
        }
    }
}
