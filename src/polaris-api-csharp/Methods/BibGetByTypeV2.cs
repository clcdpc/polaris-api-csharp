
namespace Clc.Polaris.Api
{
    public partial class PapiClient
    {
        public async Task<IRestResponse<BibGetByTypeResult>> BibGetByTypeV2Async(string key, BibGetByTypeKeyType type = BibGetByTypeKeyType.Barcode, int? branchId = null, CancellationToken cancellationToken = default)
        {
            Require.Argument(key);
            Require.PositiveIfProvided(branchId);

            var url = $"/public/v2/1033/100/{branchId ?? OrganizationId}/bib/{WebUtility.UrlEncode(key)}";
            var request = PapiRestRequest.Get(url);
            request.QueryParameters.Add("type", GetBibGetByTypeKeyTypeValue(type));
            return await ExecutePapiAsync<BibGetByTypeResult>(request, cancellationToken).ConfigureAwait(false);
        }

        private static string GetBibGetByTypeKeyTypeValue(BibGetByTypeKeyType type)
        {
            return type switch
            {
                BibGetByTypeKeyType.Barcode => "barcode",
                _ => throw new ArgumentOutOfRangeException(nameof(type), type, "Unsupported bib key type.")
            };
        }
    }
}
