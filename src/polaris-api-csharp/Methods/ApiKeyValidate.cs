using Clc.Rest;
using Clc.Polaris.Api.Models;
using System.Threading;
using System.Threading.Tasks;

namespace Clc.Polaris.Api
{
    public partial class PapiClient
    {
        public async Task<IRestResponse<PapiResponseCommon>> ApiKeyValidateAsync(CancellationToken cancellationToken = default)
        {
            var url = "/public/v1/1033/100/1/apikeyvalidate";
            var request = PapiRestRequest.Get(url);

            request.BlockStaffOverride = true;
            return await ExecutePapiAsync<PapiResponseCommon>(request, cancellationToken).ConfigureAwait(false);
        }
    }
}