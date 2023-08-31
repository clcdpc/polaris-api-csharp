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
        

        public IRestResponse<PatronMessagesGetResult> PatronMessagesGet(string barcode, bool unreadOnly = false, string password = "")
        {
            var url = $"/public/v1/1033/100/1/patron/{barcode}/messages?unreadonly={(unreadOnly ? 1 : 0)}";
            var request = new PapiRestRequest(url) { Password = password };
            return Execute<PatronMessagesGetResult>(request);
        }
    }
}