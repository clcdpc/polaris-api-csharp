using Clc.Rest;
using Clc.Polaris.Api.Models;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Xml.Linq;

namespace Clc.Polaris.Api
{
    public partial class PapiClient
    {
        

        public IRestResponse<PapiResponseCommon> PatronAccountVoid(string barcode, int paymentTxnId, int? workstationId = null, int? userId = null, string note = "")
        {
            var url = $"/protected/v1/1033/100/1/{Token.AccessToken}/patron/{WebUtility.UrlEncode(barcode)}/account/{paymentTxnId}/void/payment";
            var request = new PapiRestRequest(HttpMethod.Delete, url);
            request.QueryParameters.Add("wsid", workstationId ?? WorkstationId);
            request.QueryParameters.Add("userid", userId ?? UserId);
            return Execute<PapiResponseCommon>(request);
        }
    }
}
