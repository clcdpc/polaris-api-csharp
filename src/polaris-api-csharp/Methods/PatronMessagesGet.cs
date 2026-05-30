using System;
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


        public async Task<IRestResponse<PatronMessagesGetResult>> PatronMessagesGetAsync(string barcode, bool unreadOnly = false, string password = "", CancellationToken cancellationToken = default)
        {
            var url = $"/public/v1/1033/100/1/patron/{WebUtility.UrlEncode(barcode)}/messages";
            var request = new PapiRestRequest(url) { Password = password };
            request.QueryParameters.Add("unreadonly", unreadOnly ? 1 : 0);
            return await ExecutePapiAsync<PatronMessagesGetResult>(request, cancellationToken).ConfigureAwait(false);
        }
    }
}