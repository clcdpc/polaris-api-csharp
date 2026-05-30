using Clc.Rest;
using Clc.Polaris.Api.Models;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
namespace Clc.Polaris.Api
{
	public partial class PapiClient
    {


        public async Task<IRestResponse<PatronItemsOutGetResult>> PatronItemsOutGetAsync(string barcode, PatronItemsOutGetStatus status = PatronItemsOutGetStatus.All, string password = "", CancellationToken cancellationToken = default)
        {
            var url = $"/public/v1/1033/100/1/patron/{WebUtility.UrlEncode(barcode)}/itemsout/{status}";
            var request = new PapiRestRequest(url) { Password = password };
            return await ExecutePapiAsync<PatronItemsOutGetResult>(request, cancellationToken).ConfigureAwait(false);
        }
    }
}