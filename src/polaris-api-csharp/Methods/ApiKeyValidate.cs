using Clc.Rest;
using Clc.Polaris.Api.Models;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Clc.Polaris.Api
{
    public partial class PapiClient
    {
        public async Task<IRestResponse<PapiResponseCommon>> ApiKeyValidateAsync(CancellationToken cancellationToken = default)
        {
            var request = PapiRestRequest.Get($"/public/v1/1033/100/{OrganizationId}/apikeyvalidate");
            request.BlockStaffOverride = true;
            return await ExecutePapiAsync<PapiResponseCommon>(request, cancellationToken).ConfigureAwait(false);
        }
    }
}