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
        

        public async Task<IRestResponse<PapiResponseCommon>> PatronMessageUpdateStatusAsync(string barcode, PatronMessageType messageType, int messageId, string password = "", CancellationToken cancellationToken = default)
        {
            var url = $"/public/v1/1033/100/1/patron/{WebUtility.UrlEncode(barcode)}/messages/{messageType}/{messageId}";
            var request = new PapiRestRequest(HttpMethod.Put, url) { Password = password };
            return await ExecutePapiAsync<PapiResponseCommon>(request, cancellationToken).ConfigureAwait(false);
        }
    } 
}