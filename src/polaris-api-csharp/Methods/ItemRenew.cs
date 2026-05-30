
using Clc.Rest;
using Clc.Polaris.Api.Models;
using System.Net;
using System.Net.Http;
using System.Xml.Linq;
using System.Threading.Tasks;
using System.Threading;
namespace Clc.Polaris.Api
{
    public partial class PapiClient
    {
        public async Task<IRestResponse<ItemRenewResultWrapper>> ItemRenewAsync(string barcode, int itemId, string password = "", ItemRenewOptions renewOptions = null, CancellationToken cancellationToken = default)
        {
            var url = $"/public/v1/1033/100/1/patron/{WebUtility.UrlEncode(barcode)}/itemsout/{itemId}";
            var request = new PapiRestRequest(HttpMethod.Put, url) { Password = password, Body = renewOptions ?? new ItemRenewOptions() };
            return await ExecutePapiAsync<ItemRenewResultWrapper>(request, cancellationToken).ConfigureAwait(false);
        }
        public async Task<IRestResponse<ItemRenewResultWrapper>> ItemRenewAllForPatronAsync(string barcode, string password = "", ItemRenewOptions renewOptions = null, CancellationToken cancellationToken = default)
            => await ItemRenewAsync(barcode, 0, password, renewOptions, cancellationToken).ConfigureAwait(false);
    }
}
