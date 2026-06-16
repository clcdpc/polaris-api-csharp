

namespace Clc.Polaris.Api
{
    public partial class PapiClient
    {

        public async Task<IRestResponse<PatronTitleListAddTitleResult>> PatronTitleListAddTitleAsync(string barcode, int recordStoreId, int localControlNumber, string password = "", CancellationToken cancellationToken = default)
        {
            Require.Positive(recordStoreId);
            Require.Positive(localControlNumber);

            var url = $"/public/v1/1033/100/{OrganizationId}/patron/{EncodeBarcodePathSegment(barcode)}/patrontitlelistaddtitle/";
            var body = new PatronTitleListAddTitleData { RecordStoreId = recordStoreId, LocalControlNumber = localControlNumber };
            var request = PapiRestRequest.Post(url, body: body, password: password);
            return await ExecutePapiAsync<PatronTitleListAddTitleResult>(request, cancellationToken).ConfigureAwait(false);
        }
    }
}
