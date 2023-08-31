
using Clc.Rest;
using Clc.Polaris.Api.Models;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;

namespace Clc.Polaris.Api
{
    public partial class PapiClient
    {
        

        public IRestResponse<PapiResponseCommon> PatronTitleListDeleteAllTitles(string barcode, int listId, string password = "")
        {
            var url = $"/public/v1/1033/100/1/patron/{barcode}/patrontitlelistdeletealltitles?list={listId}";
            var request = new PapiRestRequest(HttpMethod.Delete, url) { Password = password };
            return Execute<PapiResponseCommon>(request);
        }
    }
}
