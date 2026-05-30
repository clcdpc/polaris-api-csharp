using Clc.Rest;
using Clc.Polaris.Api.Models;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Net;
using System.Linq;
using System.Text;

namespace Clc.Polaris.Api
{
    public partial class PapiClient
    {


        public async Task<IRestResponse<PatronAccountGetTitleListsResult>> PatronAccountGetTitleListsAsync(string barcode, string password = "", CancellationToken cancellationToken = default)
        {
            var url = $"/public/v1/1033/100/1/patron/{WebUtility.UrlEncode(barcode)}/patronaccountgettitlelists";
            var request = new PapiRestRequest(url) { Password = password };
            return await ExecutePapiAsync<PatronAccountGetTitleListsResult>(request, cancellationToken).ConfigureAwait(false);
        }
    }
}
