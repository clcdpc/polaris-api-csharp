
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
        

        public IRestResponse<PatronTitleListAddTitleResult> PatronTitleListAddTitle(string barcode, int recordStoreId, int localControlNumber, string password = "")
        {
            var url = $"/public/v1/1033/100/1/patron/{barcode}/patrontitlelistaddtitle/";
            var body = new PatronTitleListAddTitleData { RecordStoreId = recordStoreId, LocalControlNumber = localControlNumber };
            var request = new PapiRestRequest(HttpMethod.Post, url) { Password = password, Body = body };
            return Execute<PatronTitleListAddTitleResult>(request);
        }
    }
}
