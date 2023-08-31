using System;
using System.Threading.Tasks;
using System.Xml.Linq;
using Clc.Polaris.Api.Validation;
using Clc.Rest;
using Clc.Polaris.Api.Models;
using System.Net.Http;

namespace Clc.Polaris.Api
{
    public partial class PapiClient
    {
        

        public IRestResponse<PatronReadingHistoryGetResult> PatronReadingHistoryGet(string barcode, int page = 1, int rowsPerPage = 50, string password = "")
        {
            var url = $"/public/v1/1033/100/1/patron/{barcode}/readinghistory?page={page}&rowsperpage={rowsPerPage}";
            var request = new PapiRestRequest(url) { Password = password };
            return Execute<PatronReadingHistoryGetResult>(request);
        }
    }
}