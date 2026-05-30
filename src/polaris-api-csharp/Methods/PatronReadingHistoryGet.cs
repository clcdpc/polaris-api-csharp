using System;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using Clc.Polaris.Api.Validation;
using Clc.Rest;
using Clc.Polaris.Api.Models;
using System.Net;

namespace Clc.Polaris.Api
{
    public partial class PapiClient
    {


        public async Task<IRestResponse<PatronReadingHistoryGetResult>> PatronReadingHistoryGetAsync(string barcode, int page = 1, int rowsPerPage = 50, string password = "", CancellationToken cancellationToken = default)
        {
            var url = $"/public/v1/1033/100/1/patron/{WebUtility.UrlEncode(barcode)}/readinghistory";
            var request = PapiRestRequest.Get(url, password: password);
            request.QueryParameters.Add("page", page);
            request.QueryParameters.Add("rowsperpage", rowsPerPage);
            return await ExecutePapiAsync<PatronReadingHistoryGetResult>(request, cancellationToken).ConfigureAwait(false);
        }
    }
}