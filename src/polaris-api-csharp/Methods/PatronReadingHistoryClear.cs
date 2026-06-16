
namespace Clc.Polaris.Api
{
    public partial class PapiClient
    {
        public async Task<IRestResponse<PatronReadingHistoryClearResult>> PatronReadingHistoryClearAsync(string barcode, string password = "", IEnumerable<int>? ids = null, CancellationToken cancellationToken = default)
        {
            var idList = ids?.ToArray() ?? [];

            foreach (var id in idList)
            {
                Require.Positive(id);
            }

            var url = $"/public/v1/1033/100/{OrganizationId}/patron/{EncodeBarcodePathSegment(barcode)}/readinghistory";
            var request = PapiRestRequest.Delete(url, password: password ?? "");

            if (idList.Length > 0)
            {
                request.QueryParameters.Add("ids", string.Join(",", idList));
            }

            return await ExecutePapiAsync<PatronReadingHistoryClearResult>(request, cancellationToken).ConfigureAwait(false);
        }
    }
}