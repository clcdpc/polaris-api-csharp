using Clc.Rest;
using Clc.Polaris.Api.Models;
using System.Threading;
using System.Threading.Tasks;
using System.Net;

namespace Clc.Polaris.Api
{
    public partial class PapiClient
    {


        public async Task<IRestResponse<PatronAccountGetResult>> PatronAccountGetAsync(string barcode, string password = "", CancellationToken cancellationToken = default)
        {
            var url = $"/public/v1/1033/100/1/patron/{WebUtility.UrlEncode(barcode)}/account/outstanding";
            var request = PapiRestRequest.Get(url, password: password);
            return await ExecutePapiAsync<PatronAccountGetResult>(request, cancellationToken).ConfigureAwait(false);
        }
    }
}
