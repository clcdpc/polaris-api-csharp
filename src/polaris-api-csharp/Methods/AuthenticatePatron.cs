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
        public async Task<IRestResponse<PatronAuthenticationResult>> AuthenticatePatronAsync(string barcode, string password, CancellationToken cancellationToken = default)
        {
            var url = "/public/v1/1033/100/1/authenticator/patron";
            var body = new { Barcode = barcode, Password = password };
            var request = PapiRestRequest.Post(url, body: body);
            request.BlockStaffOverride = true;
            return await ExecutePapiAsync<PatronAuthenticationResult>(request, cancellationToken).ConfigureAwait(false);
        }
    }
}
