using Clc.Rest;
using Clc.Polaris.Api.Models;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
namespace Clc.Polaris.Api
{
    public partial class PapiClient
    {


        public async Task<IRestResponse<PatronValidateResult>> PatronValidateAsync(string barcode, string password = "", CancellationToken cancellationToken = default)
        {
            var url = $"/public/v1/1033/100/1/patron/{WebUtility.UrlEncode(barcode)}";
            var request = PapiRestRequest.Get(url, password: password);
            return await ExecutePapiCoreAsync<PatronValidateResult>(request, cancellationToken).ConfigureAwait(false);
        }
    }
}