using Clc.Polaris.Api.Models;
using Clc.Polaris.Api.Validation;
using Clc.Rest;
using System.Threading;
using System.Threading.Tasks;

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
        public async Task<IRestResponse<PapiResponseCommon>> PatronTitleListCopyTitleAsync(string barcode, int fromRecordStoreId, int fromPosition, int toRecordStoreId, string password = "", CancellationToken cancellationToken = default)
        {
            Require.Positive(fromRecordStoreId);
            Require.Positive(fromPosition);
            Require.Positive(toRecordStoreId);

            var url = $"/public/v1/1033/100/{OrganizationId}/patron/{EncodeBarcodePathSegment(barcode)}/patrontitlelistcopytitle/";
            var body = new PatronTitleListCopyTitleData { FromRecordStoreId = fromRecordStoreId, FromPosition = fromPosition, ToRecordStoreId = toRecordStoreId };
            var request = PapiRestRequest.Post(url, body: body, password: password);
            return await ExecutePapiAsync<PapiResponseCommon>(request, cancellationToken).ConfigureAwait(false);
        }
    }
}
