using System;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;

using Clc.Polaris.Api.Validation;
using Clc.Rest;
using Clc.Polaris.Api.Models;
using System.Net.Http;
using System.Linq;

namespace Clc.Polaris.Api
{
    public partial class PapiClient
    {
        public async Task<IRestResponse<NotificationUpdateResult>> NotificationUpdateAsync(NotificationUpdateParams updateParams, CancellationToken cancellationToken = default)
        {            
            await EnsureProtectedTokenAsync(cancellationToken).ConfigureAwait(false);
            var url = $"/protected/v1/1033/100/1/{Token.AccessToken}/notification/{updateParams.NotificationTypeId}";
            var request = new PapiRestRequest(HttpMethod.Put, url) { Body = updateParams };
            return await ExecutePapiAsync<NotificationUpdateResult>(request, cancellationToken).ConfigureAwait(false);
        }
    }
}