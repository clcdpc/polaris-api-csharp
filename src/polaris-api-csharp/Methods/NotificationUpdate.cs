using System;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;

using Clc.Polaris.Api.Validation;
using Clc.Rest;
using Clc.Polaris.Api.Models;
using System.Collections.Generic;
using System.Linq;

namespace Clc.Polaris.Api
{
    public partial class PapiClient
    {
        public async Task<IRestResponse<NotificationUpdateResult>> NotificationUpdateAsync(NotificationUpdateParams updateParams, CancellationToken cancellationToken = default)
        {
            var url = $"/protected/v1/1033/100/1/{ProtectedToken.Placeholder}/notification/{updateParams.NotificationTypeId}";
            var request = PapiRestRequest.Put(url, body: updateParams);
            return await ExecutePapiCoreAsync<NotificationUpdateResult>(request, cancellationToken).ConfigureAwait(false);
        }
    }
}