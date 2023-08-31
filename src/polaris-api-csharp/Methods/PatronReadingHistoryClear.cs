using System;
using System.Threading.Tasks;
using System.Xml.Linq;
using Clc.Polaris.Api.Validation;
using Clc.Rest;
using Clc.Polaris.Api.Models;
using System.Net.Http;
using System.Linq;

namespace Clc.Polaris.Api
{
    public partial class PapiClient
    {        
        public IRestResponse<PapiResponseCommon> PatronReadingHistoryClear(string barcode, params int[] ids)
            => PatronReadingHistoryClear(barcode, null, ids);

        public IRestResponse<PapiResponseCommon> PatronReadingHistoryClear(string barcode, string password, params int[] ids)
        {
            var url = $"/public/v1/1033/100/1/patron/{barcode}/readinghistory";
            if (ids.Any()) { url += $"?ids={string.Join(",", ids)}"; }
            var request = new PapiRestRequest(HttpMethod.Delete, url) { Password = password };
            return Execute<PapiResponseCommon>(request);
        }
    }
}