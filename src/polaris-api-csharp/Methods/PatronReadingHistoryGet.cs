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
        

        public IRestResponse<PatronReadingHistoryGetResult> PatronReadingHistoryGet(string barcode, int page = 1, int rowsPerPage = 50, string password = "")
        {
            var url = $"/public/v1/1033/100/1/patron/{WebUtility.UrlEncode(barcode)}/readinghistory";
            var request = new PapiRestRequest(url) { Password = password };
            request.QueryParameters.Add("page", page);
            request.QueryParameters.Add("rowsperpage", rowsPerPage);
            return Execute<PatronReadingHistoryGetResult>(request);
        }
    }
}