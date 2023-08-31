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
        

        public IRestResponse<PapiResponseCommon> PatronAccountCreateTitleList(string barcode, string listName, string password = "")
        {
            var url = $"/public/v1/1033/100/1/patron/{barcode}/patronaccountcreatetitlelist";
            var body = new PatronAccountCreateTitleListData { RecordStoreName = listName };
            var request = new PapiRestRequest(HttpMethod.Post, url) { Password = password, Body = body };
            return Execute<PapiResponseCommon>(request);
        }
    }
}
