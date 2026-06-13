using Clc.Polaris.Api.Models;
using Clc.Rest;
using System.Threading;
using System.Threading.Tasks;
namespace Clc.Polaris.Api
{
    public partial class PapiClient
    {


        public async Task<IRestResponse<PatronItemsOutGetResult>> PatronItemsOutGetAsync(string barcode, PatronItemsOutGetStatus status = PatronItemsOutGetStatus.All, string password = "", CancellationToken cancellationToken = default)
        {
            var url = $"/public/v1/1033/100/{OrganizationId}/patron/{EncodeBarcodePathSegment(barcode)}/itemsout/{status}";
            var request = PapiRestRequest.Get(url, password: password);
            return await ExecutePapiAsync<PatronItemsOutGetResult>(request, cancellationToken).ConfigureAwait(false);
        }
    }
}