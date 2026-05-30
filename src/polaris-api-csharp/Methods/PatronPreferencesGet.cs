using System;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using Clc.Polaris.Api.Validation;
using Clc.Rest;
using Clc.Polaris.Api.Models;
using System.Net;
using System.Net.Http;

namespace Clc.Polaris.Api
{
    public partial class PapiClient
    {


        public async Task<IRestResponse<PatronPreferencesGetResult>> PatronPreferencesGetAsync(string barcode, string password = "", CancellationToken cancellationToken = default)
        {
            var url = $"/public/v1/1033/100/1/patron/{WebUtility.UrlEncode(barcode)}/preferences";
            var request = new PapiRestRequest(url) { Password = password };
            return await ExecutePapiAsync<PatronPreferencesGetResult>(request, cancellationToken).ConfigureAwait(false);
        }
    }
}