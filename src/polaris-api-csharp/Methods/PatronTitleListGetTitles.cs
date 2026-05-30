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


        public async Task<IRestResponse<PatronTitleListGetTitlesResult>> PatronTitleListGetTitlesAsync(string barcode, int listId, int startPosition = 1, int endPosition = 100, string password = "", CancellationToken cancellationToken = default)
        {
            var url = $"/public/v1/1033/100/1/patron/{WebUtility.UrlEncode(barcode)}/patrontitlelistgettitles";
            var request = new PapiRestRequest(url) { Password = password };
            request.QueryParameters.Add("list", listId);
            request.QueryParameters.Add("startPosition", startPosition);
            request.QueryParameters.Add("endPosition", endPosition);
            return await ExecutePapiAsync<PatronTitleListGetTitlesResult>(request, cancellationToken).ConfigureAwait(false);
        }
    }
}