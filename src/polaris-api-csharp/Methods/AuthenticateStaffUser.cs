using Clc.Rest;
using Clc.Polaris.Api.Models;

using System;
using System.Threading.Tasks;
using System.Threading;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Xml.Linq;

namespace Clc.Polaris.Api
{
    public partial class PapiClient
    {
        public async Task<IRestResponse<ProtectedToken>> AuthenticateStaffUserAsync(PolarisUser staffUser, CancellationToken cancellationToken = default)
        {
            var url = "/protected/v1/1033/100/1/authenticator/staff";
            return await PostAsync<ProtectedToken>(url, body: staffUser, cancellationToken: cancellationToken).ConfigureAwait(false);
        }

        
    }
}
