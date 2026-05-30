using System;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using Clc.Polaris.Api.Validation;
using Clc.Rest;
using Clc.Polaris.Api.Models;
using System.Net;
using System.Net.Http;
using System.Collections.Generic;
using System.Linq;

namespace Clc.Polaris.Api
{
    public partial class PapiClient
    {
        public Task<IRestResponse<PapiResponseCommon>> PatronReadingHistoryClearAsync(string barcode, IEnumerable<int> ids, CancellationToken cancellationToken = default)
            => PatronReadingHistoryClearAsync(barcode, null, ids, cancellationToken);

        public async Task<IRestResponse<PapiResponseCommon>> PatronReadingHistoryClearAsync(string barcode, string password, IEnumerable<int> ids, CancellationToken cancellationToken = default)
        {
            var url = $"/public/v1/1033/100/1/patron/{WebUtility.UrlEncode(barcode)}/readinghistory";
            var request = new PapiRestRequest(HttpMethod.Delete, url) { Password = password };
            if (ids.Any()) { request.QueryParameters.Add("ids", string.Join(",", ids)); }
            return await ExecutePapiAsync<PapiResponseCommon>(request, cancellationToken).ConfigureAwait(false);
        }
    }
}