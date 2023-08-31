
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
        

        public IRestResponse<PapiResponseCommon> PatronTitleListCopyAllTitles(string barcode, int fromRecordStoreId, int toRecordStoreId, string password = "")
        {
            var url = $"/public/v1/1033/100/1/patron/{barcode}/patrontitlelistcopyalltitles/";
            var body = new PatronTitleListCopyAllTitlesData { FromRecordStoreId = fromRecordStoreId, ToRecordStoreId = toRecordStoreId };
            var request = new PapiRestRequest(HttpMethod.Post, url) { Password = password, Body = body };
            return Execute<PapiResponseCommon>(request);
        }
    }
}
