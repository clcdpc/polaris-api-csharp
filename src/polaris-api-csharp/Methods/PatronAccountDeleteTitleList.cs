using System.Threading;
using System.Threading.Tasks;
using Clc.Rest;
using Clc.Polaris.Api.Models;
using System.Net;

namespace Clc.Polaris.Api
{
    public partial class PapiClient
    {


        public async Task<IRestResponse<PapiResponseCommon>> PatronAccountDeleteTitleListAsync(string barcode, int listId, string password = "", CancellationToken cancellationToken = default)
        {
            var url = $"/public/v1/1033/100/1/patron/{WebUtility.UrlEncode(barcode)}/patronaccountdeletetitlelist";
            var request = PapiRestRequest.Delete(url, password: password);
            request.QueryParameters.Add("list", listId);
            return await ExecutePapiAsync<PapiResponseCommon>(request, cancellationToken).ConfigureAwait(false);
        }
    }
}