
using Clc.Rest;
using Clc.Polaris.Api.Models;
using System;
using System.Threading.Tasks;
using System.Threading;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;

namespace Clc.Polaris.Api
{
    public partial class PapiClient
    {
        

        public async Task<IRestResponse<PatronTitleListAddTitleResult>> PatronTitleListAddTitleAsync(string barcode, int recordStoreId, int localControlNumber, string password = "", CancellationToken cancellationToken = default)
        {
            var url = $"/public/v1/1033/100/1/patron/{WebUtility.UrlEncode(barcode)}/patrontitlelistaddtitle/";
            var body = new PatronTitleListAddTitleData { RecordStoreId = recordStoreId, LocalControlNumber = localControlNumber };
            var request = new PapiRestRequest(HttpMethod.Post, url) { Password = password, Body = body };
            return await ExecutePapiAsync<PatronTitleListAddTitleResult>(request, cancellationToken).ConfigureAwait(false);
        }
    }
}
