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
        

        public IRestResponse<PatronILLRequestsGetResult> PatronILLRequestsGet(string barcode, ILLStatus status = ILLStatus.All, string password = "")
        {
            var url = $"/public/v1/1033/100/1/patron/{barcode}/illrequests/{status}";
            var request = new PapiRestRequest(url) { Password = password };
            return Execute<PatronILLRequestsGetResult>(request);
        }
    }    
}
