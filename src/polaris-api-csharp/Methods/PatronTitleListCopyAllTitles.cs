

namespace Clc.Polaris.Api
{
    public partial class PapiClient
    {

        public async Task<IRestResponse<PatronTitleListCopyAllTitlesResult>> PatronTitleListCopyAllTitlesAsync(string barcode, int fromRecordStoreId, int toRecordStoreId, string password = "", CancellationToken cancellationToken = default)
        {
            Require.Positive(fromRecordStoreId);
            Require.Positive(toRecordStoreId);

            var url = $"/public/v1/1033/100/{OrganizationId}/patron/{EncodeBarcodePathSegment(barcode)}/patrontitlelistcopyalltitles/";
            var body = new PatronTitleListCopyAllTitlesData { FromRecordStoreId = fromRecordStoreId, ToRecordStoreId = toRecordStoreId };
            var request = PapiRestRequest.Post(url, body: body, password: password);
            return await ExecutePapiAsync<PatronTitleListCopyAllTitlesResult>(request, cancellationToken).ConfigureAwait(false);
        }
    }
}
