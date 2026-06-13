using Clc.Polaris.Api.Models;
using Clc.Polaris.Api.Validation;
using Clc.Rest;
using System.Threading;
using System.Threading.Tasks;

namespace Clc.Polaris.Api
{
    public partial class PapiClient
    {
        public async Task<IRestResponse<PapiResponseCommon>> NotificationQueueGetAsync(int orgId = 1, CancellationToken cancellationToken = default)
        {
            Require.Positive(orgId);

            //"protected/{Version}/{LangID}/{AppID}/{OrgID}/{AccessToken}/notification
            var url = $"/protected/v1/1033/24/{orgId}/{ProtectedToken.Placeholder}/notification/";
            var request = PapiRestRequest.Get(url);
            return await ExecutePapiAsync<PapiResponseCommon>(request, cancellationToken).ConfigureAwait(false);
        }
    }
}
