using Clc.Rest;
using Clc.Polaris.Api.Models;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;

namespace Clc.Polaris.Api
{
    public partial class PapiClient
    {


        public async Task<IRestResponse<PatronILLRequestsGetResult>> PatronILLRequestsGetAsync(string barcode, ILLStatus status = ILLStatus.All, string password = "", CancellationToken cancellationToken = default)
        {
            var url = $"/public/v1/1033/100/1/patron/{WebUtility.UrlEncode(barcode)}/illrequests/{status}";
            var request = new PapiRestRequest(url) { Password = password };
            return await ExecutePapiAsync<PatronILLRequestsGetResult>(request, cancellationToken).ConfigureAwait(false);
        }
    }
}
