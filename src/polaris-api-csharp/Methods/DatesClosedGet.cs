using Clc.Rest;
using Clc.Polaris.Api.Models;
using Clc.Polaris.Api.Validation;
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
        public async Task<IRestResponse<DatesClosedGetResult>> DatesClosedGetAsync(int organizationId, CancellationToken cancellationToken = default)
        {
            Require.Positive(organizationId);

            var url = $"/public/v1/1033/100/{organizationId}/datesclosed";
            var request = PapiRestRequest.Get(url);
            request.BlockStaffOverride = true;
            return await ExecutePapiAsync<DatesClosedGetResult>(request, cancellationToken).ConfigureAwait(false);
        }
    }
}
