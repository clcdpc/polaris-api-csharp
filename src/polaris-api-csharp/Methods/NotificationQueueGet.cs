using Clc.Rest;
using Clc.Polaris.Api.Models;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Text;

namespace Clc.Polaris.Api
{
    public partial class PapiClient
    {
        public async Task<IRestResponse<PapiResponseCommon>> NotificationQueueGetAsync(int orgId = 1, CancellationToken cancellationToken = default)
        {
            //"protected/{Version}/{LangID}/{AppID}/{OrgID}/{AccessToken}/notification
            var token = await GetTokenAsync(cancellationToken).ConfigureAwait(false);
            var url = $"/protected/v1/1033/24/{orgId}/{token.AccessToken}/notification/";
            var request = new PapiRestRequest(url);
            return await ExecutePapiAsync<PapiResponseCommon>(request, cancellationToken).ConfigureAwait(false);
        }
    }
}
