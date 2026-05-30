using Clc.Rest;
using Clc.Polaris.Api.Models;

using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Clc.Polaris.Api
{
    public partial class PapiClient
    {
        public async Task<IRestResponse<ProtectedToken>> AuthenticateStaffUserAsync(PolarisUser staffUser, CancellationToken cancellationToken = default)
        {
            var request = PapiRestRequest.Post("/protected/v1/1033/100/1/authenticator/staff", body: staffUser);

            return await ExecutePapiAsync<ProtectedToken>(request, cancellationToken, ProtectedTokenPreloadMode.Skip).ConfigureAwait(false);
        }


    }
}
