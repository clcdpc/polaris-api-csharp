namespace Clc.Polaris.Api
{
    public partial class PapiClient
    {
        /// <summary>
        ///
        /// </summary>
        /// <param name="barcode"></param>
        /// <param name="fromRecordStoreId"></param>
        /// <param name="fromPosition">starts at 1, not 0</param>
        /// <param name="toRecordStoreId"></param>
        /// <param name="password"></param>
        /// <returns></returns>

        public async Task<IRestResponse<PatronTitleListMoveTitleResult>> PatronTitleListMoveTitleAsync(string barcode, int fromRecordStoreId, int fromPosition, int toRecordStoreId, string password = "", CancellationToken cancellationToken = default)
        {
            Require.Positive(fromRecordStoreId);
            Require.Positive(fromPosition);
            Require.Positive(toRecordStoreId);

            var url = $"/public/v1/1033/100/{OrganizationId}/patron/{EncodeBarcodePathSegment(barcode)}/patrontitlelistmovetitle/";
            var body = new PatronTitleListMoveTitleData { FromRecordStoreId = fromRecordStoreId, FromPosition = fromPosition, ToRecordStoreId = toRecordStoreId };
            var request = PapiRestRequest.Post(url, body: body, password: password);
            return await ExecutePapiAsync<PatronTitleListMoveTitleResult>(request, cancellationToken).ConfigureAwait(false);
        }
    }
}
