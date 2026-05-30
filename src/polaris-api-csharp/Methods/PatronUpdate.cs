
using Clc.Rest;
using Clc.Polaris.Api.Models;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Xml.Linq;

namespace Clc.Polaris.Api
{
    public partial class PapiClient
    {


        public async Task<IRestResponse<PatronUpdateResult>> PatronUpdateAsync(string barcode, PatronUpdateParams updateParams, string password = "", bool ignoresa = true, CancellationToken cancellationToken = default)
        {
            var url = $"/public/v1/1033/100/1/patron/{WebUtility.UrlEncode(barcode)}";
            var request = PapiRestRequest.Put(url, body: updateParams, password: password);
            request.QueryParameters.Add("ignoresa", ignoresa);
            return await ExecutePapiAsync<PatronUpdateResult>(request, cancellationToken).ConfigureAwait(false);
        }
    }
}
