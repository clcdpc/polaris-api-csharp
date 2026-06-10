
using Clc.Rest;
using Clc.Polaris.Api.Models;
using System.Net;
using System.Xml.Linq;
using System.Threading;
using System.Threading.Tasks;
namespace Clc.Polaris.Api
{
    public partial class PapiClient
    {
        public async Task<IRestResponse<ItemRenewResultWrapper>> ItemRenewAsync(string barcode, int itemId, string password = "", ItemRenewOptions? renewOptions = null, CancellationToken cancellationToken = default)
        {
            var url = $"/public/v1/1033/100/1/patron/{EncodeBarcodePathSegment(barcode)}/itemsout/{itemId}";
            var request = PapiRestRequest.Put(url, body: renewOptions ?? new ItemRenewOptions(), password: password);
            return await ExecutePapiAsync<ItemRenewResultWrapper>(request, cancellationToken).ConfigureAwait(false);
        }
    }
}
