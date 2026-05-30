using System.Threading;
using System.Threading.Tasks;
using Clc.Rest;
using Clc.Polaris.Api.Models;
using System.Net;

namespace Clc.Polaris.Api
{
    public partial class PapiClient
    {


        public async Task<IRestResponse<PatronSavedSearchesGetResult>> PatronSavedSearchesGetAsync(string barcode, string password = "", CancellationToken cancellationToken = default)
        {
            var url = $"/public/v1/1033/100/1/patron/{WebUtility.UrlEncode(barcode)}/savedsearches";
            var request = PapiRestRequest.Get(url, password: password);
            return await ExecutePapiAsync<PatronSavedSearchesGetResult>(request, cancellationToken).ConfigureAwait(false);
        }
    }
}