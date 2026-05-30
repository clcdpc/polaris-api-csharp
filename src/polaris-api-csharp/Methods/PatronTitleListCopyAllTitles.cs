
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


        public async Task<IRestResponse<PapiResponseCommon>> PatronTitleListCopyAllTitlesAsync(string barcode, int fromRecordStoreId, int toRecordStoreId, string password = "", CancellationToken cancellationToken = default)
        {
            var url = $"/public/v1/1033/100/1/patron/{WebUtility.UrlEncode(barcode)}/patrontitlelistcopyalltitles/";
            var body = new PatronTitleListCopyAllTitlesData { FromRecordStoreId = fromRecordStoreId, ToRecordStoreId = toRecordStoreId };
            var request = new PapiRestRequest(HttpMethod.Post, url) { Password = password, Body = body };
            return await ExecutePapiAsync<PapiResponseCommon>(request, cancellationToken).ConfigureAwait(false);
        }
    }
}
